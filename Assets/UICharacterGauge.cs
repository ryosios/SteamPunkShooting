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
    // Start is called before the first frame update

    public int _getSpecialPointValue { get; set; } = 40; //Specialゲージが増える値
    private void Awake()
    {
        _initSpecialGaugeArrowRotate = _specialGaugeArrow.localRotation.eulerAngles;

        GaugeImage.fillAmount = _characterLocator._characterSpecialLevel.Value;
        _characterLocator._characterSpecialLevel
            .DistinctUntilChanged()//同じ値なら無視
            .Subscribe(specialLevel => //値が引数で自動で入る
            {
                // GaugeValueSet(specialLevel);
                GaugeValueSet2(specialLevel);
            });
    }
    public void GaugeValueSet(float specialLevel)
    {
        if(GaugeImage.fillAmount <= 1)
        {
            GaugeImage.fillAmount = specialLevel;
        }
        
    }

    public void GaugeValueSet2(int specialLevel)
    {
        //_specialGaugeArrow.localRotation = Quaternion.Euler(_initSpecialGaugeArrowRotate);

        _specialGaugeArrow.DORotate(new Vector3(0, 0, 120 - specialLevel), 0.5f,RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
        Debug.Log(specialLevel);

    }

}
