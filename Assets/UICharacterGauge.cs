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

    /// <summary>スペシャルUI矢印の最小回転角度、初期値</summary>
    private const int UI_SPECIAL_ROTATE_MIN_DEGREE = -120;

    /// <summary>スペシャルUI矢印の最大回転角度</summary>
    private const int UI_SPECIAL_ROTATE_MAX_DEGREE = 120;;

    private void Awake()
    {
        _initSpecialGaugeArrowRotate = _specialGaugeArrow.localRotation.eulerAngles;

        GaugeImage.fillAmount = _characterLocator._characterSpecialLevel.Value;
        _characterLocator._characterSpecialLevel
            .DistinctUntilChanged()//同じ値なら無視
            .Subscribe(specialLevel => //値が引数で自動で入る
            {
                SetUISpecialGauge(specialLevel);
            });
    }

    public void SetUISpecialGauge(int specialLevel)
    {
        int gap = UI_SPECIAL_ROTATE_MAX_DEGREE - UI_SPECIAL_ROTATE_MIN_DEGREE;
        int maxLevel = CharacterSpecialLevel.MAX_SPECIAL_LEVEL;
        int rotate = 1.0 * gap * specialLevel / maxLevel
        int targetRotate = UI_SPECIAL_ROTATE_MIN_DEGREE + rotate;
        
        _specialGaugeArrow.DORotate(new Vector3(0, 0, -targetRotate), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
        Debug.Log(specialLevel);
    }

}
