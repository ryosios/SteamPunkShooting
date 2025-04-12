using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UniRx;

public class ParticleController : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterLocator _characterLocator;
    
    private int _getDamagePointValue = 1;
    private float _getSpecialPointValue = 0.1f;

    void Awake()
    {
        _characterLocator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterLocator>();     
    }
   
    void OnParticleCollision(GameObject obj)
    {
        if(obj.tag == "Player")
        {
            Debug.Log("Hit");
            _characterLocator._getDamageSubject.OnNext(_getDamagePointValue);
            _characterLocator._getSpecialLevelSubject.OnNext(_getSpecialPointValue);

        }
        if (obj.tag == "CharacterSpecial")
        {
            Debug.Log("CharacterSpecial");
           
        }

    }
}
