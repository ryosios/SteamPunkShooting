using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private BackGroundMaker _backGroundMaker;
    [SerializeField] private Animator _titleImageAnimator;
    [SerializeField] private RectTransform[] _titleTweenHaguruma;

    public ReactiveProperty<int> _stageNumber = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> _chapterNumber = new ReactiveProperty<int>(0);

    public Button _startButton;
    public Button TestButtonS;
    public Button TestButtonC;
    public Button TestButtonN;

    [Header("テスト用変数")]
    [SerializeField] private int _testChapterNumber;

    //Chapter
    [SerializeField] private ChapterBase[] _chapters;



    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        TestButtonS.gameObject.SetActive(false);
        TestButtonC.gameObject.SetActive(false);
        TestButtonN.gameObject.SetActive(false);
     
              
               

#endif
        _startButton.OnClickAsObservable()
               .Subscribe(_ =>
               {
                   foreach(RectTransform tweenHaguruma in _titleTweenHaguruma)
                   {
                       tweenHaguruma.DOKill();
                   }
                   _titleImageAnimator.SetTrigger("TitleAnimation4");
                   _chapterNumber.Value += 1;
               
               })
               .AddTo(this); // thisが破棄されたときに自動解除

        TestButtonS.OnClickAsObservable()
               .Subscribe(_ => { _stageNumber.Value += 1; })
               .AddTo(this); // thisが破棄されたときに自動解除
        TestButtonC.OnClickAsObservable()
              .Subscribe(_ => { _chapterNumber.Value += 1; })
              .AddTo(this); // thisが破棄されたときに自動解除
        TestButtonN.OnClickAsObservable()
             .Subscribe(_ => 
             {
                 _chapterNumber.Value = _testChapterNumber;
             })
             .AddTo(this); // thisが破棄されたときに自動解除

        _stageNumber
            .Subscribe(stageNumber => //値が引数で自動で入る
            {
                //チャプター切り替わった
                switch (stageNumber)
                {
                    case 0:
                        //ステージ0開始時
                        _chapterNumber
                            .Subscribe(chapterNumber => //値が引数で自動で入る
                            {
                                switch (chapterNumber)
                                {
                                    case 0:
                                        Debug.Log("1_chapter0");
                                        
                                        break;
                                    case 1:
                                        Debug.Log("1_chapter1");
                                        _chapters[1]._selectNumber.Value = 1;
                                        //ChapterNumberを0にセットする処理が必要
                                        break;
                                    case 2:
                                        Debug.Log("1_chapter2");
                                        
                                        //ChapterNumberを0にセットする処理が必要
                                        break;

                                }
                            }).AddTo(this);
                        break;

                    case 1:
                        //ステージ0開始時
                        _chapterNumber
                            .Subscribe(chapterNumber => //値が引数で自動で入る
                            {
                                switch (chapterNumber)
                                {
                                    case 0:
                                        Debug.Log("2_chapter0");
                                        break;
                                    case 1:
                                        Debug.Log("2_chapter1");
                                        break;

                                }
                            }).AddTo(this);
                        break;

                }

            }).AddTo(this);


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
