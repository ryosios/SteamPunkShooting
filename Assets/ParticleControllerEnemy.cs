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

    private Subject<Unit> _onGrazeTriggered = new Subject<Unit>();

    void Awake()
    {
        _attackParticle = GetComponent<ParticleSystem>();
        _attackParticle.trigger.SetCollider(0, _enemyLocator._characterLocator.graiseCollision);
    }

    private void Start()
    {
        _onGrazeTriggered
          .ThrottleFirst(System.TimeSpan.FromSeconds(0.2f))  // 0.2秒の間隔を置く
          .Subscribe(_ => TriggerGrazeCheck());  // 0.2秒ごとにグレイズ判定処理を呼び出す
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
            _onGrazeTriggered.OnNext(Unit.Default);  // グレイズ判定発生を通知
        }
    }
    private void TriggerGrazeCheck()
    {
        // グレイズカウント加算やSE再生など、実際の処理
        UIPoint._instance.AddPoint(1);
    }
}
