using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocator : MonoBehaviour
{
    public CharacterLocator _characterLocator { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        _characterLocator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterLocator>();
    }

    // Update is called once per frame
   
}
