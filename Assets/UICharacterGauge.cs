using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class UICharacterGauge : MonoBehaviour
{

    [SerializeField] private Image GaugeImage;
    [SerializeField] private CharacterLocator _characterLocator;

    [SerializeField] private RectTransform _specialGaugeArrow;
    private Vector3 _initSpecialGaugeArrowRotate;

    //effect
    [SerializeField] private ParticleSystem _effectSpecialGaugeSmoke;

    // Start is called before the first frame update

    public int _getSpecialPointValue { get; set; } = 40; //SpecialÉQÅ[ÉWÇ™ëùÇ¶ÇÈíl
    private void Awake()
    {
        _initSpecialGaugeArrowRotate = _specialGaugeArrow.localRotation.eulerAngles;
    }
 

    public void SpecialGaugeValueSet(int specialLevel)
    {
        if (specialLevel <= 6 && specialLevel >= 1)
        {
            _specialGaugeArrow.DOLocalRotate(new Vector3(0, 0, specialLevel * 40), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
            _effectSpecialGaugeSmoke.Play();
            Debug.Log(specialLevel);
        }
        else
        {
            _specialGaugeArrow.DOLocalRotate(new Vector3(0, 0, specialLevel * 40), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
            Debug.Log(specialLevel);
        }
   

    }

}
