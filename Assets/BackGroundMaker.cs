using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;

public class BackGroundMaker : MonoBehaviour
{

    [SerializeField] private Transform[] _bgObjectsTra;
    [SerializeField] private Transform _bgObjectsSetterObjRTrans;

    [SerializeField] private float _bgSpeed;
    [SerializeField] private float _bgDistance;
    [SerializeField] private float _bgRangeTime;
    private bool _isBgCreate = false;


    private void Awake()
    {
        _isBgCreate = true;
    }


    public void CreateBgObject()
    {

        for(int i = 0; i < _bgObjectsTra.Length; i++)
        {
            if (_bgObjectsTra[i].transform.localPosition.z >= 63)
            {
                _bgObjectsTra[i].transform.localPosition = new Vector3(0f, 0f, -4f);
            }
            _bgObjectsTra[i].transform.localPosition += new Vector3(0f, 0f, _bgSpeed * Time.deltaTime);
           
        }   
    }

    /*
    public async void CreateBgObjectSet()
    {
       if(_isBgCreate)
        {
            _isBgCreate = false;
            CreateBgObject();
            await UniTask.Delay(TimeSpan.FromSeconds(_bgRangeTime));
            _isBgCreate = true;
        }
        
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
