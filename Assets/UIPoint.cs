using System.Drawing;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class UIPoint : MonoBehaviour
{
    public static UIPoint _instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _pointCountText;
    [SerializeField] private CharacterLocator _characterLocator;


    //現在のポイント
    public ReactiveProperty<int> _nowPoint = new ReactiveProperty<int>(0);

    //グレイズイベント
    public Subject<Unit> _onGrazeTriggered = new Subject<Unit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _pointCountText.text = _nowPoint.Value.ToString("D9");
    }

    private void Start()
    {
        _onGrazeTriggered
             .ThrottleFirst(System.TimeSpan.FromSeconds(0.2f))  // 0.2秒の間隔を置く
             .Subscribe(_ => TriggerGrazeCheck());  // 0.2秒ごとにグレイズ判定処理を呼び出す
    }

    // Update is called once per frame
    public void AddPoint(int point)
    {
        _nowPoint.Value += point;
        _pointCountText.text = _nowPoint.Value.ToString("D9");
       
    }

    public void TriggerGrazeCheck()
    {
        // グレイズカウント加算やSE再生など、実際の処理
        AddPoint(1);
        _characterLocator.GrazeEffectPlay();

    }
}
