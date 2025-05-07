using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultMaster : MonoBehaviour
{
    [SerializeField] private UIResult _uIResult;
    [SerializeField] private Button BackButton;
    [SerializeField] private PlayableDirector BackToTimelinePlayable;
    public SaveData _loadData { get; set; }
    private CancellationToken _destroyToken;
    private void Awake()
    {
        _destroyToken = this.GetCancellationTokenOnDestroy(); // ゲームオブジェクトが破棄されたらキャンセル
        _loadData = SaveSystem.Load();

        if (_loadData != null)
        {
            Debug.Log($"ステージとポイント: {_loadData._resultPoint},現在のステージ: {_loadData._nowStage}, クリアしたかどうかのフラグ: {_loadData._isStageCleared}");
        }

        BackButton.OnClickAsObservable()
            .First() // 最初の1回だけ通す
               .Subscribe(_ => 
               {
                   BackToTitle(_destroyToken);
               })
               .AddTo(this); // thisが破棄されたときに自動解除

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //スコアを表示
        _uIResult.SetPoint(_loadData._resultPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void BackToTitle(CancellationToken destroyToken)
    {
        try
        {
            BackToTimelinePlayable.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: destroyToken);//Timelineの終了を待つ
            SceneManager.LoadScene("Game");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ダメージ処理中にオブジェクトが破棄されました（処理中断）");
            return; // 以降の処理を中止
        }

    }
}
