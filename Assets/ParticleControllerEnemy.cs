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
    
    private int _getDamagePointValue = 1;
    

    void Awake()
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
}
