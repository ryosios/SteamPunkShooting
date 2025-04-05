using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class ParticleController : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterLocator _characterLocator;


    private Action _particleHit;
    private float _mutekiTime = 1f;
  

    void Awake()
    {
        _particleHit = CharacterHpSet;
        _characterLocator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterLocator>();
       
    }
    private async void CharacterHpSet()
    {
        _characterLocator._characterHP -= 1; //HP1Œ¸‚ç‚·
        _characterLocator.gameObject.layer = 6;

        await UniTask.Delay(TimeSpan.FromSeconds(_mutekiTime));
        _characterLocator.gameObject.layer = 3;
    }

    void OnParticleCollision(GameObject obj)
    {
        Debug.Log("Hit");
        _particleHit.Invoke();
    }
}
