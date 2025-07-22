using Mirror;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceChatPlayer : NetworkBehaviour
{
    [Header("Settings")]
    public float proximityRange = 15f;

    private AudioSource _audioSource;
    private AudioClip _microphoneClip;
    private const int sampleRate = 44100;
    private const int micLengthSec = 1;

    private int _lastSamplePosition;
    private float[] _audioData = new float[sampleRate];

    private string _micDevice;
    private bool _micReady = false;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = AudioClip.Create("PlayerVoice", 44100, 1, 44100, false);
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f; // 3D audio

        _audioSource.Play();
        Debug.Log("[Voice] AudioSource clip created and playback started.");
    }

    public override void OnStartLocalPlayer()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("[Voice] No microphones found.");
            return;
        }

        _micDevice = Microphone.devices[0];  // Pick the first available mic
        Debug.Log($"[Voice] Using mic device: {_micDevice}");

        _microphoneClip = Microphone.Start(_micDevice, true, 1, 44100);

        if (_microphoneClip == null)
        {
            Debug.LogError("[Voice] Failed to start microphone.");
        }
        else
        {
            Debug.Log("[Voice] Microphone started successfully.");
        }
    }

    private const int sendRate = 441; // 441 samples = 10ms at 44.1kHz

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!_micReady)
        {
            int pos = Microphone.GetPosition(_micDevice);
            Debug.Log($"[Voice] Waiting for mic... pos = {pos}");
            if (pos > 0)
            {
                _micReady = true;
                Debug.Log("[Voice] Mic is ready.");
            }
            return;
        }

        if (!Microphone.IsRecording(_micDevice))
        {
            Debug.LogWarning("[Voice] Mic stopped recording.");
            return;
        }

        int micPosition = Microphone.GetPosition(_micDevice);
        int samplesToSend = sendRate;

        if (micPosition < _lastSamplePosition)
            _lastSamplePosition = 0; // Loop wrap

        if (micPosition - _lastSamplePosition >= samplesToSend)
        {
            float[] samples = new float[samplesToSend];
            _microphoneClip?.GetData(samples, _lastSamplePosition);
            _lastSamplePosition += samplesToSend;

            byte[] data = FloatArrayToByteArray(samples);

            Debug.Log($"[Voice] Sending audio data: {data.Length} bytes");

            CmdSendVoice(data);
        }
    }

    [Command(channel = Channels.Unreliable)]
    void CmdSendVoice(byte[] data)
    {
        Debug.Log($"[VoiceChat] Received voice data from client, size: {data.Length}");
        RpcReceiveVoice(data);
    }

    [ClientRpc(channel = Channels.Unreliable)]
    void RpcReceiveVoice(byte[] data)
    {
        Debug.Log($"[RPC] Received audio data: {data?.Length ?? 0} bytes");

        if (isLocalPlayer)
            return;

        if (data == null || data.Length == 0)
        {
            Debug.LogWarning("[RPC] Received empty or null audio data");
            return;
        }

        if (_audioSource == null)
        {
            Debug.LogError("[RPC] AudioSource is null on " + gameObject.name);
            return;
        }

        if (_audioSource.clip == null)
        {
            Debug.LogError("[RPC] AudioSource.clip is null on " + gameObject.name);
            return;
        }

        float dist = Vector3.Distance(transform.position, NetworkClient.localPlayer.transform.position);
        if (dist > proximityRange)
        {
            Debug.Log("[RPC] Ignored voice due to distance: " + dist);
            return;
        }

        float[] floatData = ByteArrayToFloatArray(data);
        if (floatData == null || floatData.Length == 0)
        {
            Debug.LogWarning("[RPC] Received invalid audio data");
            return;
        }

        try
        {
            _audioSource.clip.SetData(floatData, 0);
        }
        catch (Exception e)
        {
            Debug.LogError("[RPC] Exception setting audio data: " + e);
            return;
        }

        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
            Debug.Log("[RPC] Playing audio source at distance: " + dist);
        }
    }


    public static byte[] FloatArrayToByteArray(float[] floatArray)
    {
        byte[] byteArray = new byte[floatArray.Length * 2]; // 2 bytes per sample (Int16)
        for (int i = 0; i < floatArray.Length; i++)
        {
            short val = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * short.MaxValue);
            byte[] bytes = System.BitConverter.GetBytes(val);
            byteArray[i * 2] = bytes[0];
            byteArray[i * 2 + 1] = bytes[1];
        }
        return byteArray;
    }

    public static float[] ByteArrayToFloatArray(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 2];
        for (int i = 0; i < floatArray.Length; i++)
        {
            short val = System.BitConverter.ToInt16(byteArray, i * 2);
            floatArray[i] = val / (float)short.MaxValue;
        }
        return floatArray;
    }
}
