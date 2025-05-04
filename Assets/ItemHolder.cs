using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
public class ItemHolder : MonoBehaviour
{
    
    [SerializeField] private Transform _hPItem;
    [SerializeField] private Transform _itemHolderLocator;
    [SerializeField] private GameMaster _gameMaster;
    private List<Transform> _hPItems = new List<Transform>();//現在存在しているHPアイテム

    public Subject<UniRx.Unit> _setHPItemSubject = new Subject<UniRx.Unit>();//アイテムが生まれるタイミング

    //ポイント10000ごとに1つHPアイテム出す
    private int _hpItemPointInterval = 10000;
    //10秒ごとに1つHPアイテム出す
    private float _hpItemTimeInterval = 10f;

 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _setHPItemSubject
           .Subscribe(_ =>
           {
               HPItemSetting();

           }).AddTo(this);
    }
    private void Start()
    {      

        //10秒ごとにHPアイテム生む
        Observable.Interval(System.TimeSpan.FromSeconds(_hpItemTimeInterval))
            .SkipUntil(_gameMaster._startChapter1Subject)//チャプター1が開始されるまでは購読無視する
            .Subscribe(_ =>
            {
                Debug.Log("HPアイテム出現1");
                _setHPItemSubject.OnNext(Unit.Default);
            })
            .AddTo(this);

        //ポイント10000ごとに1つHPアイテム生む
        UIPoint._instance._nowPoint //ポイントが変動するタイミングを監視
            .SkipUntil(_gameMaster._startChapter1Subject)//チャプター1が開始されるまでは購読無視する
            .DistinctUntilChanged()
             .Subscribe(nowPoint =>
             {
                 Debug.Log("HPアイテム出現2");
                 int nowPointRemainder = nowPoint % _hpItemPointInterval;

                 if (nowPointRemainder == 0)
                 {
                     _setHPItemSubject.OnNext(Unit.Default);
                 }

             })
            .AddTo(this);

    }



    private void HPItemSetting()
    {
        Transform hPITem = Instantiate(_hPItem, this.transform);

        //ランダムでItemHolderLocatorの位置を移動し、その位置にアイテムを出す
        float randomValue = UnityEngine.Random.Range(-3.3f, 3.3f);
        Vector3 _newItemHolderLocatorPos = new Vector3(randomValue , _itemHolderLocator.localPosition.y,0f);

        hPITem.transform.localPosition = _newItemHolderLocatorPos;
        hPITem.gameObject.SetActive(true);
        _hPItems.Add(hPITem);
    }

    private void ItemHolderLocatorRandomSetting()//ランダムの位置に動かす
    {
        float randomValue = UnityEngine.Random.Range(-3.3f, 3.3f);

    }


}
