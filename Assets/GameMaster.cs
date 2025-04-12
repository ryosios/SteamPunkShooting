using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private BackGroundMaker _backGroundMaker;

    private ReactiveProperty<int> _stageNumber = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> _chapterNumber = new ReactiveProperty<int>(0);

    // Start is called before the first frame update
    void Awake()
    {

        _chapterNumber
            .DistinctUntilChanged()//同じ値なら無視
            .Subscribe(stageNumber => //値が引数で自動で入る
            {
               //チャプター切り替わった
            });


    }

    // Update is called once per frame
    void Update()
    {  
        BackGround();
        StageSet(_stageNumber.Value, _chapterNumber.Value);
    }

    private void BackGround()
    {
        _backGroundMaker.CreateBgObject();
    }
    private void StageSet(int stageNumber, int chapterNumber)
    {
        switch (stageNumber)
        {
            case 0:
                //ステージ0開始時
                break;

                switch (chapterNumber)
                {
                    case 0:
                        //チャプター0開始時
                        break;
                    case 1:
                        //
                        break;
                }

            case 1:
                //ステージ1開始時
                switch (chapterNumber)
                {
                    case 0:
                        //チャプター1開始時
                        break;
                    case 1:
                        //
                        break;
                }

                break;

        }
       
    }
}
