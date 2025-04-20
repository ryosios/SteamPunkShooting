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

    private const float _specialGaugeInitialAngle = 120.0f;
    private const float _specialGaugeUnitAngle = -40.0f; //1Specialゲージあたりのメータの角度
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
        var currentAngle = Mathf.DeltaAngle(0.0f, _specialGaugeArrow.rotation.eulerAngles.z);
        var targetAngle = _specialGaugeInitialAngle + _specialGaugeUnitAngle * specialLevel;
        _specialGaugeArrow.DOLocalRotate(new Vector3(0, 0, targetAngle - currentAngle), 0.5f, RotateMode.LocalAxisAdd);
        Debug.Log(specialLevel);

    }

}
