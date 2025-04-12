using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private BackGroundMaker _backGroundMaker;

    // Start is called before the first frame update
    void Awake()
    {
 
       
    }

    // Update is called once per frame
    void Update()
    {  
        BackGround();
    }

    private void BackGround()
    {
        _backGroundMaker.CreateBgObject();
    }
}
