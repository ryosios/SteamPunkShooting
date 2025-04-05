using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICharacterHp : MonoBehaviour
{
   
    [SerializeField] private RectTransform[] _hPStarsUpperRect;
    [SerializeField] private RectTransform[] _hPStarsUnderRect;


    private int _initHpStarInt = 5;


    private void Awake()
    {
        SetHpValue(_initHpStarInt);
    }

    public void SetHpValue(int nowHp)
    {
        for (int i = 0; i < 10; i++)
        {
            _hPStarsUpperRect[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < nowHp; i++)
        {
            _hPStarsUpperRect[i].gameObject.SetActive(true);
        }
    }

}
