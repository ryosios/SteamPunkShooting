using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


public class UICharacterHp : MonoBehaviour
{
   
    [SerializeField] private RectTransform[] _hPStarsUpperRect;
    [SerializeField] private RectTransform[] _hPStarsUnderRect;
    [SerializeField] private CharacterLocator _characterLocator;


    private void Awake()
    {
        //èâä˙HPê›íË
        SetHpValue(_characterLocator._characterHP.Value);
    }

    public void SetHpValue(int nowHp)//UIÇ…HPÇÉZÉbÉg
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
