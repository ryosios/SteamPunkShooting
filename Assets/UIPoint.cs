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


    //現在のポイント
    public ReactiveProperty<int> _nowPoint = new ReactiveProperty<int>(0);
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

    // Update is called once per frame
    public void AddPoint(int point)
    {
        _nowPoint.Value += point;
        _pointCountText.text = _nowPoint.Value.ToString("D9");
       
    }
}
