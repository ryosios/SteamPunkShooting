using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UniRx;

public class ParticleController : MonoBehaviour
{
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
           // _enemyLocator._characterLocator._getSpecialLevelSubject.OnNext(_getSpecialPointValue);

        }
        if (obj.tag == "CharacterSpecial")
        {
            Debug.Log("CharacterSpecial");
           
        }

    }
}
