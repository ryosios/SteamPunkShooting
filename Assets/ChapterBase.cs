using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;

public abstract class ChapterBase : MonoBehaviour
{
    public abstract ReactiveProperty<int> _selectNumber { get; set; } 
   
    public abstract void SetChapter(int selectNumber);

    private GameMaster _gameMaster;
    [SerializeField] protected Transform[] _enemys;
    private EnemyLocator[] _enemyLocators;
    protected ParticleSystem[] _attackParticles;

    private List<EnemyLocator> _enemyLocatorsAlliveList = new List<EnemyLocator>(); //生きてる扱いのEnemyLocatorをいれておくリスト

    //個別設定
    protected Vector3[] _initEnemyPos;//先頭のエネミーの位置
    protected Vector3 _posDuration = new Vector3(0, 0, 0);//距離の差分
    protected float _timeDuration = 1f;//時間の差分

    protected CancellationToken _destroyToken;

    [HideInInspector] public DG.Tweening.Sequence _sequence;

    private void OnDestroy()
    {
        _sequence.Kill();
    }

    private void Start()
    {
        _gameMaster = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();

        _initEnemyPos = new Vector3[_enemys.Length];
        _destroyToken = this.GetCancellationTokenOnDestroy(); // ゲームオブジェクトが破棄されたらキャンセル
        _enemyLocators = new EnemyLocator[_enemys.Length];
        for (int i = 0; i < _enemys.Length; i++)
        {
            //生存フラグ用にインスタンスごとのEnemyLocatorを取得
            _enemyLocators[i] = _enemys[i].GetComponent<EnemyLocator>();
        }
        foreach (var enemyLocator in _enemyLocators)
        {
            _enemyLocatorsAlliveList.Add(enemyLocator);
        }

        for (int i = 0; i < _enemys.Length; i++)
        {
            _initEnemyPos[i] = _enemys[i].localPosition;
        }


        _attackParticles = new ParticleSystem[_enemys.Length];
        for (int i = 0; i < _enemys.Length; i++)
        {
            _attackParticles[i] = _enemys[i].transform.Find("AttackParticle").GetComponent<ParticleSystem>();
            _attackParticles[i].Stop();
        }

        _selectNumber.Subscribe(selectNumber => SetChapter(selectNumber)).AddTo(this);//_selectNumberが変更されたらその数に応じたChapterが再生される
    }

    protected async UniTaskVoid NextChapter(CancellationToken destroyToken)
    {
        _selectNumber.Value += 1;
        _gameMaster._chapterNumber.Value += 1;
        for (int i = 0; i < _attackParticles.Length; i++)
        {
            _attackParticles[i].Stop();//攻撃パーティクルを終了
        }
        _sequence.Kill();
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(10f), cancellationToken: destroyToken);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ダメージ処理中にオブジェクトが破棄されました（処理中断）");
            return; // 以降の処理を中止
        }
        _enemyLocatorsAlliveList.Clear();
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

    protected async UniTaskVoid NextChapterCompleteEnemy()
    {

        for (int i = _enemyLocatorsAlliveList.Count - 1; i >= 0; i--)
        {
            if (!_enemyLocatorsAlliveList[i]._isAlive)
            {
                _enemyLocatorsAlliveList.Remove(_enemyLocatorsAlliveList[i]);
            }
        }

        if (_enemyLocatorsAlliveList.Count == 0)
        {
            //敵全滅したら次チャプター
            NextChapter(_destroyToken).Forget();
        }
    }

    protected async UniTaskVoid WaitStartAsync(float waitSecond, CancellationToken destroyToken)
    {
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitSecond), cancellationToken: destroyToken);  // 1秒待つ
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ダメージ処理中にオブジェクトが破棄されました（処理中断）");
            return; // 以降の処理を中止
        }
        _selectNumber.Value += 1;
        Debug.Log("selectNumber1開始");
    }
}
