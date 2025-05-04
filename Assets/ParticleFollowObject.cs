using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowObject : EnemyLocator
{
    //EnemyLocator継承
    private Transform _target;
    public float _speed;
    public float _maxAngle = 45f;
    int particleCount;

    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    // Start is called before the first frame update
    void Start()
    {
        _target = _characterLocator.transform;
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_target != null)
        {
            particleCount = _particleSystem.particleCount;
        }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        _particleSystem.GetParticles(particles);

        for(int i=0; i<particleCount; i++)
        {
            Vector3 directionToTarget = (_target.position - particles[i].position).normalized;

            Vector3 currentDirection = particles[i].velocity.normalized;

            float angleBetween = Vector3.Angle(currentDirection, directionToTarget);

            if(angleBetween <= _maxAngle)
            {
                particles[i].velocity += directionToTarget * _speed * Time.deltaTime;
            }
        }

        _particleSystem.SetParticles(particles, particleCount);
    }
}
