using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProximityChat : NetworkBehaviour
{
    [SerializeField] private float _voiceRange = 10f;

    public void StartRecording()
    {
        SteamUser.StartVoiceRecording();
    }

    public byte[] GetCompressedVoiceData()
    {
        uint compressedBytes;
        if (SteamUser.GetAvailableVoice(out compressedBytes) == EVoiceResult.k_EVoiceResultOK && compressedBytes > 0)
        {
            byte[] compressedBuffer = new byte[compressedBytes];
            uint bytesWritten;
            SteamUser.GetVoice(true, compressedBuffer, compressedBytes, out bytesWritten);
            Debug.Log($"[VoiceChat] Captured {bytesWritten} bytes of voice data.");
            return compressedBuffer;
        }
        return null;
    }

    public float[] DecompressVoiceData(byte[] compressedBuffer)
    {
        byte[] uncompressedAudio = new byte[22000];
        uint bytesWritten;
        SteamUser.DecompressVoice(compressedBuffer, (uint)compressedBuffer.Length, uncompressedAudio, (uint)uncompressedAudio.Length, out bytesWritten, 11025);

        float[] audioData = new float[bytesWritten / 2];
        for (int i = 0; i < audioData.Length; i++)
        {
            short sample = BitConverter.ToInt16(uncompressedAudio, i * 2);
            audioData[i] = sample / (float)short.MaxValue;
        }
        return audioData;
    }

    public void PlayVoice(float[] audioData, AudioSource audioSource)
    {
        AudioClip clip = AudioClip.Create("Voice", audioData.Length, 1, 11025, false);
        clip.SetData(audioData, 0);
        audioSource.clip = clip;
        audioSource.Play();
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdSendVoice(byte[] voiceData)
    {
        Debug.Log($"[VoiceChat] Sending voice chat: {voiceData}");
        RpcReceiveVoice(voiceData);
    }

    [ClientRpc(channel = Channels.Unreliable)]
    public void RpcReceiveVoice(byte[] voiceData)
    {
        float[] audioData = DecompressVoiceData(voiceData);
        Debug.Log($"[VoiceChat] Receive voice chat: {audioData}");
        PlayVoice(audioData, GetComponent<AudioSource>());
    }

    public bool IsWithinProximity(Transform otherPlayer, float range)
    {
        return Vector3.Distance(transform.position, otherPlayer.position) <= range;
    }

    private void Start()
    {
        StartCoroutine(TransmitVoiceRoutine());
    }

    private IEnumerator TransmitVoiceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // Transmit every 100ms

            byte[] compressedVoice = GetCompressedVoiceData();
            if (compressedVoice != null)
            {
                List<NetworkIdentity> nearbyPlayers = GetPlayersInRange(_voiceRange);
                CmdSendVoiceToPlayers(compressedVoice, nearbyPlayers.Select(p => p.netId).ToList());
            }
        }
    }

    public List<NetworkIdentity> GetPlayersInRange(float range)
    {
        List<NetworkIdentity> playersInRange = new List<NetworkIdentity>();

        foreach (NetworkIdentity player in NetworkServer.spawned.Values)
        {
            if (/*player != GameManager.Instance.LocalPlayer.GetComponent<NetworkIdentity>() &&*/ Vector3.Distance(transform.position, player.transform.position) <= range)
            {
                playersInRange.Add(player);
            }
        }

        return playersInRange;
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdSendVoiceToPlayers(byte[] voiceData, List<uint> targetNetIds)
    {
        foreach (uint netId in targetNetIds)
        {
            NetworkIdentity target = NetworkServer.spawned[netId];
            TargetReceiveVoice(target.connectionToClient, voiceData);
        }
    }

    [TargetRpc]
    public void TargetReceiveVoice(NetworkConnection conn, byte[] voiceData)
    {
        float[] audioData = DecompressVoiceData(voiceData);
        PlayVoice(audioData, GetComponent<AudioSource>());
    }
}
