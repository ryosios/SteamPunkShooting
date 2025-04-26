using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenHaguruma : MonoBehaviour
{
    [SerializeField] private RectTransform _hagurumaRect;
    [SerializeField] private float _rotateTime;
    [SerializeField] private bool _isRotateReverse;
    public bool _destroyMotion = true;


    // Start is called before the first frame update
    private void Start()
    {
        Vector3 thisRotate = this.GetComponent<RectTransform>().localRotation.eulerAngles;
        if (!_isRotateReverse)
        {
            _hagurumaRect.DORotate(new Vector3(0f, 0f, thisRotate.z + 360f), _rotateTime, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
        else
        {
            _hagurumaRect.DORotate(new Vector3(0f, 0f, thisRotate.z - 360f), _rotateTime, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
        
    }

    private void OnDestroy()
    {
        _hagurumaRect.DOKill();
    }

}
