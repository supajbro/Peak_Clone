using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePooler : MonoBehaviour
{
    [System.Serializable]
    public enum ParticleType
    {
        GroundHit = 0,
        GroundHitText,
        ENDOFTYPES
    }

    public static ParticlePooler Instance;
    private void SetInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Parent")]
    [SerializeField] private Transform _parent;
    public Transform Parent => _parent;

    [Header("Pooled Particles")]
    [SerializeField] private List<Particle> _particles;
    private void SpawnParticles()
    {
        foreach (var particle in _particles)
        {
            particle.Spawn();
        }
    }

    private void Awake()
    {
        SetInstance();
        SpawnParticles();
    }

    public void PlayParticle(ParticleType type, Vector3 position)
    {
        foreach (var particle in _particles)
        {
            if(particle.Type == type)
            {
                particle.Play(position);
            }
        }
    }
}

[System.Serializable]
public class Particle
{
    public string Name;
    public ParticlePooler.ParticleType Type;
    public ParticleSystem ParticlePrefab;
    public List<ParticleSystem> Particles = new();
    public int Amount = 10;

    public void Spawn()
    {
        for (int i = 0; i < Amount; i++)
        {
            var particle = ParticlePooler.Instantiate(ParticlePrefab, ParticlePooler.Instance.Parent);
            Particles.Add(particle);
        }
    }

    public void Play(Vector3 position)
    {
        foreach (var particle in Particles)
        {
            if (!particle.isPlaying)
            {
                particle.transform.position = position;
                particle.Play();
                return;
            }
        }

        // If all are playing, spawn a new one and play it
        Debug.Log($"[Particle Pooler] Ran out of available particles of type {Type}. Spawning a new one.");
        var newParticle = ParticlePooler.Instantiate(ParticlePrefab, ParticlePooler.Instance.Parent);
        Particles.Add(newParticle);
        newParticle.Play();
    }
}
