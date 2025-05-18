using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UniRx;

public class ParticleControllerEnemy : MonoBehaviour
{
    //エネミー側の弾がキャラクターにダメージを与えるスクリプト
    // Start is called before the first frame update

    [SerializeField] EnemyLocator _enemyLocator;
    private ParticleSystem _attackParticle;

    private int _getDamagePointValue = 1;

   

    void Awake()
    {
        _attackParticle = GetComponent<ParticleSystem>();
        _attackParticle.trigger.SetCollider(0, _enemyLocator._characterLocator.graiseCollision);
        Debug.Log("グレイズコリジョン"+_enemyLocator._characterLocator.graiseCollision);
    }

    private void Start()
    {
      
    }
    void OnParticleCollision(GameObject obj)
    {
        if(obj.tag == "Player")
        {
            Debug.Log("Hit");
            _enemyLocator._characterLocator._getDamageSubject.OnNext(_getDamagePointValue);

        }
        if (obj.tag == "CharacterSpecial")
        {
            Debug.Log("CharacterSpecial");
           
        }

    }
    void OnParticleTrigger()
    {
        var particles = new List<ParticleSystem.Particle>();
        int count = _attackParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, particles);

        if (count > 0)
        {
            //Debug.Log("グレイズ！");
            UIPoint._instance._onGrazeTriggered.OnNext(Unit.Default);  // グレイズ判定発生を通知
        }
    }
   
}
