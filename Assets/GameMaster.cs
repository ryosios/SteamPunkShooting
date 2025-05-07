using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private BackGroundMaker _backGroundMaker;
    [SerializeField] private Animator _titleImageAnimator;
    [SerializeField] private RectTransform[] _titleTweenHaguruma;
    [SerializeField] private Transform _stageTrans;
    [SerializeField] private CharacterLocator _characterLocator;

    //キャラクターの挙動開始フラグ
    public Subject<UniRx.Unit> _startChapter1Subject = new Subject<UniRx.Unit>();//挙動開始。chapter1開始時。アタック撃つフラグ
    public Subject<UniRx.Unit> _gameOverSubject = new Subject<UniRx.Unit>();//ゲームオーバーイベント
    public Subject<UniRx.Unit> _nextStageSubject = new Subject<UniRx.Unit>();//ネクストステージイベント

    public ReactiveProperty<int> _stageNumber = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> _chapterNumber = new ReactiveProperty<int>(0);

    public Button _startButton;
    public Button TestButtonS;
    public Button TestButtonC;
    public Button TestButtonN;
    public Toggle TestToggle;
    public Button TestButtonDataClear;

    [Header("テスト用変数")]
    [SerializeField] private int _testChapterNumber;
    [SerializeField] private bool _isDebugGameover = true;

    //Chapter
    private ChapterBase _chapter;


    //ポイントセーブ用
    //ステージが増えたらSavaDataクラスに追加
    //並べ替え用
    private List<int> _sortPointList = new List<int>(5);

    public bool _isStagePlay { get; set; } = false;
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        TestButtonS.gameObject.SetActive(false);
        TestButtonC.gameObject.SetActive(false);
        TestButtonN.gameObject.SetActive(false);
        TestToggle.gameObject.SetActive(false);
        TestButtonDataClear.gameObject.SetActive(false);
#endif
        _gameOverSubject
               .Where(_ => _isDebugGameover)
                .Subscribe(_ =>
                {
                    //ゲームオーバー時
                    _isStagePlay = false;
                    Debug.Log("ゲームオーバー");
                    HandleGameOverAsync();
                    SaveGame(_stageNumber.Value, false);

                }).AddTo(this);
        _nextStageSubject             
               .Subscribe(_ =>
               {
                   //ネクストステージ時
                   _isStagePlay = false;
                   Debug.Log("ネクストステージ");
                   HandleNextGameAsync();
                   SaveGame(_stageNumber.Value, true);
               }).AddTo(this);


        _startButton.OnClickAsObservable()
            .First() // 最初の1回だけ通す
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
        TestToggle
            .OnValueChangedAsObservable()
            .Subscribe(isOn =>
            {
                _isDebugGameover = isOn;
            })
            .AddTo(this); // オブジェクト破棄時に購読解除
        TestButtonDataClear.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SaveDataClear();
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
                                        Debug.Log("0_chapter0");
                                        _isStagePlay = true;
                                        break;
                                    case 1:
                                        Debug.Log("0_chapter1");

                                        GameObject chapter = (GameObject) Resources.Load("Chapter/Chapter1");
                                        _chapter = Instantiate(chapter, _stageTrans).GetComponent<ChapterBase>();
                                        _startChapter1Subject.OnNext(UniRx.Unit.Default);//ステージごとのChapter1に必ず必要！
                                        _chapter._selectNumber.Value = 0;//チャプター1のセレクトナンバー0を設定。待機時間ののち向こうで1になる
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
    void FixedUpdate()
    {  
        BackGround();
        
    }

    private void BackGround()
    {
        _backGroundMaker.CreateBgObject();
    }
    private async UniTaskVoid HandleGameOverAsync()
    {
        //ゲームオーバーのときの処理
        _backGroundMaker.SetBgSpeed(0f);
        _characterLocator.AttackSetActive(false);
        _characterLocator.CharacterGameoverAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(3f)); // 3秒待つ

        SceneManager.LoadScene("Result");
    }
    private async UniTaskVoid HandleNextGameAsync()
    {
        //ネクストステージのときの処理
        _backGroundMaker.SetBgSpeed(0f);
        //_characterLocator.CharacterGameoverAnimation();
        //await UniTask.Delay(TimeSpan.FromSeconds(3f)); // 3秒待つ

       // SceneManager.LoadScene("Result");
    }

    private void SaveGame(int nowStage, bool isCleared)
    {
        //ステージが増えたら追加。
        SaveData loaded = SaveSystem.Load();

        if (loaded == null)
        {
            //セーブファイルがなかったら全部0いれて作成
            for(int i = 0; i<_sortPointList.Count; i++)
            {
                _sortPointList[i] = 0;
            }
            SaveData newData = new SaveData //セーブデータまとめたクラス
            {
                _resultPoint = _sortPointList,
                _finalPoint = 0,
                _nowStage = 0,
                _isStageCleared = false
            };
            SaveSystem.Save(newData);

        }

        if (loaded != null)
        {
            Debug.Log($"ステージとポイント: {loaded._resultPoint},現在のステージ: {loaded._nowStage}, クリアしたかどうかのフラグ: {loaded._isStageCleared}");
            //スコアデータに新規データを交えて５つに並べ替え。
            foreach (var resultPoint in loaded._resultPoint)
            {
                //ソート用のリストに保存
                _sortPointList.Add(resultPoint);
            }
            
            //現在のステージのポイントを取得（ステージごとに0に戻るのでたぶん引数いらない）
            int thisStagePoint = UIPoint._instance._nowPoint.Value;
            if(nowStage == 0)
            {
                //最初のステージ開始時は最終ポイントを0に戻す
                loaded._finalPoint = 0;
            }
            int finalPoint = thisStagePoint + loaded._finalPoint;
            _sortPointList.Add(finalPoint);
            // 降順に並べて、先頭5個を抽出。
            List<int> _sorted =　_sortPointList
                .OrderByDescending(n => n) // 降順ソート
                .Take(5)                    // 上から5個だけ取得
                .ToList();
            _sortPointList.Clear();//一度クリアしたリストにソートした配列をいれなおす
            _sortPointList = _sorted;

            SaveData data = new SaveData
            {
                _resultPoint = _sortPointList,//ソート済みスコア
                _finalPoint = finalPoint,//ゲームオーバーまで合算したスコア
                _nowStage = nowStage,//現在のステージ
                _isStageCleared = isCleared//クリアしたかどうか

            };

            SaveSystem.Save(data);
            Debug.Log("セーブ完了。保存場所：" + Application.persistentDataPath);
        }
     
    }

    private void SaveDataClear()
    {
        //スコア完全初期化用
        SaveData loaded = SaveSystem.Load();
        if (loaded == null)
        {
            return;
        }
        if (loaded != null)
        {

            _sortPointList.Clear();
            SaveData newData = new SaveData
            {
                _resultPoint = _sortPointList,
                _finalPoint = 0,
                _nowStage = 0,
                _isStageCleared = false
            };

            SaveSystem.Save(newData);
            Debug.Log("セーブクリア完了。保存場所：" + Application.persistentDataPath);
        }
      
    }

}
