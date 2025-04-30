using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterHp : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _characterLocatorCC2D;
    [SerializeField] private UICharacterHp _uICharacterHp;

    private Action _characterHit;

    void Awake()
    {
       
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        _characterHit.Invoke();
    }
  
}
