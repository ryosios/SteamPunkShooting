using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChasePlayer : MonoBehaviour
{
    public ParticleSystem ps;
    ParticleSystem.Particle[] m_Particles;

    public float threshold = 100f;
    public float intensity = 1f;

    // ターゲットをセットする
    public Transform target;

    void Update()
    {
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticlesAlive = ps.GetParticles(m_Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            var velocity = ps.transform.TransformPoint(m_Particles[i].velocity);
            var position = ps.transform.TransformPoint(m_Particles[i].position);

            var period = m_Particles[i].remainingLifetime * 0.9f;

            //ターゲットと自分自身の差
            var diff = target.TransformPoint(target.position) - position;
            Vector3 accel = (diff - velocity * period) * 2f / (period * period);

            //加速度が一定以上だと追尾を弱くする
            if (accel.magnitude > threshold)
            {
                accel = accel.normalized * threshold;
            }

            // 速度の計算
            velocity += accel * Time.deltaTime * intensity;
            m_Particles[i].velocity = ps.transform.InverseTransformPoint(velocity);
        }
        ps.SetParticles(m_Particles, numParticlesAlive);
    }
}
