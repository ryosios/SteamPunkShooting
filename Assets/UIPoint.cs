using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPoint : MonoBehaviour
{
    public static UIPoint _instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _pointCountText;


    //現在のポイント
    private static int _nowPoint =0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _pointCountText.text = _nowPoint.ToString("D9");
    }

    // Update is called once per frame
    public void AddPoint(int point)
    {
        _nowPoint += point;
        _pointCountText.text = _nowPoint.ToString("D9");
    }
}
