using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Chapter1 : ChapterBase
{
    // Start is called before the first frame update

    [SerializeField] private GameMaster _gameMaster;
    [SerializeField] private Transform[] _enemys;
    private EnemyLocator[] _enemyLocators;
    private ParticleSystem[] _attackParticles;

    private List<EnemyLocator> _enemyLocatorsAlliveList = new List<EnemyLocator>(); //生きてる扱いのEnemyLocatorをいれておくリスト
    public override ReactiveProperty<int> _selectNumber { get; set; } = new ReactiveProperty<int>(-1);

    [HideInInspector] public DG.Tweening.Sequence _sequence;

    //個別設定
    private Vector3 _initEnemyPos;//先頭のエネミーの位置
    private Vector3 _posDuration = new Vector3(0, 0, 0);//距離の差分
    private float _timeDuration = 1f;//時間の差分


    void Awake()
    {
        _enemyLocators = new EnemyLocator[_enemys.Length];
        for(int i = 0; i< _enemys.Length; i++)
        {
            //生存フラグ用にインスタンスごとのEnemyLocatorを取得
            _enemyLocators[i] = _enemys[i].GetComponent<EnemyLocator>();
        }
        foreach(var enemyLocator in _enemyLocators)
        {
            _enemyLocatorsAlliveList.Add(enemyLocator);
        }

        _initEnemyPos = _enemys[0].localPosition;

        _attackParticles = new ParticleSystem[_enemys.Length];
        for (int i = 0; i < _enemys.Length; i++)
        {
            _attackParticles[i] = _enemys[i].transform.Find("AttackParticle").GetComponent<ParticleSystem>();
            _attackParticles[i].Stop();
        }

        _selectNumber.Subscribe(selectNumber => SetChapter(selectNumber)).AddTo(this);//_selectNumberが変更されたらその数に応じたChapterが再生される
    }

    public override void SetChapter(int selectNumber)
    {
        
        switch (selectNumber)
        {
            case 0://待機時間用
                WaitStartAsync(1f).Forget(); 
                break;
            case 1://動作開始
                for (int i = 0; i < _attackParticles.Length; i++)
                {
                    _attackParticles[i].Play();//攻撃パーティクルを開始
                  
                }

                _sequence = DOTween.Sequence();
                for (int i = 0; i < _enemys.Length; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        _enemys[i].localPosition = new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f),
                            new Vector3(2.41f,-0.5f,0f),
                            //new Vector3(2.41f,5.71f,0f),

                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));
                       
                    }
                   
                }
                _sequence.OnUpdate(() =>
                {
                    //敵が全員負けリストに入ったら強制次チャプターへ
                    NextChapterCompleteEnemy().Forget();
                });
                _sequence.OnComplete(() =>
                { 
                    _selectNumber.Value += 1;

                });
                _sequence.Play();
                break;

            case 2:
                _sequence.Kill();
                _sequence = DOTween.Sequence();

                for (int i = 0; i < _enemys.Length; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(2.41f,-0.5f,0f),
                            new Vector3(2.41f,5.71f,0f),
                           

                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));         
                    }                  
                }
                _sequence.OnUpdate(() =>
                {
                    NextChapterCompleteEnemy().Forget();
                });
                _sequence.OnComplete(() =>
                {
                    NextChapter().Forget();
                });
                _sequence.Play();
                break;

        }

    }


    private async UniTaskVoid NextChapterCompleteEnemy()
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
            
            
            NextChapter().Forget();

        }

    }

    private async UniTaskVoid NextChapter()
    {
        _selectNumber.Value += 1;
        _gameMaster._chapterNumber.Value += 1;
        for (int i = 0; i < _attackParticles.Length; i++)
        {
            _attackParticles[i].Stop();//攻撃パーティクルを終了
        }
        _sequence.Kill();
        await UniTask.Delay(System.TimeSpan.FromSeconds(10f));
        _enemyLocatorsAlliveList.Clear();
        Destroy(this.gameObject);

    }

    private async UniTaskVoid WaitStartAsync(float waitSecond)
    {
        
        await UniTask.Delay(System.TimeSpan.FromSeconds(waitSecond));  // 1秒待つ
        _selectNumber.Value += 1;
        Debug.Log("selectNumber1開始");
    }

}
