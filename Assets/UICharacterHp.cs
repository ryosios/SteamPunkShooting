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

    private Image[] _hPStarsUpperImage;
    private ParticleSystem[] _hPStarsUpperParticle;
    private void Awake()
    {
        _hPStarsUpperImage = new Image[_hPStarsUpperRect.Length];
        _hPStarsUpperParticle = new ParticleSystem[_hPStarsUpperRect.Length];
        for (int i = 0; i< _hPStarsUpperRect.Length; i++)
        {
            Debug.Log(_hPStarsUpperRect[i]);
            _hPStarsUpperImage[i] = _hPStarsUpperRect[i].gameObject.GetComponent<Image>();
            _hPStarsUpperParticle[i] = _hPStarsUpperRect[i].GetComponentInChildren<ParticleSystem>();
        }
        //èâä˙HPê›íË
        SetHpValue(_characterLocator._characterHP.Value);
    }

    public void SetHpValue(int nowHp)//UIÇ…HPÇÉZÉbÉg
    {
        for (int i = 0; i < 10; i++)
        {
            //_hPStarsUpperRect[i].gameObject.SetActive(false);
            _hPStarsUpperImage[i].color = new Color(1f, 1f, 1f, 0f);
        }
        for (int i = 0; i < nowHp; i++)
        {
            //_hPStarsUpperRect[i].gameObject.SetActive(true);
            _hPStarsUpperImage[i].color = new Color(1f, 1f, 1f, 1f);           
        }
        if(nowHp >= 0 && nowHp < 10)
        {
            _hPStarsUpperParticle[nowHp].Play();
        }
        
    }

}
