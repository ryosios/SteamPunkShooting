using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;


public class CharacterLocator : MonoBehaviour
{
    [SerializeField] private GameMaster _gameMaster;
    [SerializeField] private Transform _characterLocatorTrans;
    [SerializeField] private Rigidbody2D _characterLocatorRigid;
    [SerializeField] private UICharacterGauge _uICharacterGauge;
    [SerializeField] private UICharacterHp _uICharacterHp;
    [SerializeField] private GameObject _characterSpecial;
    [SerializeField] private SkeletonAnimation _characterSpineSA;
    [SerializeField] private PlayableDirector _cutinPlayable;
    [SerializeField] private Transform _characterSpecialPosTrans;
    [SerializeField] private ParticleSystem _characterSpecialEffectParticle;

    [Header("AttackType")]
    [SerializeField] private ParticleSystem[] _characterAttackObject;
    [SerializeField] private TransformFadeGroup[] _bitsFadeGroup;
    private Tween _attackTween;

    //速度周り
    private float _characterVelocity { get; set; } = 3f;
    private float _initCharacterVelocity;
    private float _characterAnimationTimeScale = 1f;

    //HP管理用
    public ReactiveProperty<int> _characterHP { get; set; } = new ReactiveProperty<int>(5);
    public Subject<int> _getDamageSubject = new Subject<int>();//ダメージ受けたとき
    private int _previousHP;//１フレ前のHP用

    //スペシャル用
    public ReactiveProperty<int> _characterSpecialLevel { get; set; } = new ReactiveProperty<int>(0); //スペシャルレベル
    public Subject<UniRx.Unit> _playSpecialSubject = new Subject<UniRx.Unit>();//スぺシャル撃ったとき
    private float _specialTime = 2f; //スペシャルの効果時間
    private bool _isSpecialActive = false;
    private Vector3 _characterSpecialPos;
    private float _mutekiTime = 2f;

    //スキル。レベルがあがるとアタックが強化される
    public ReactiveProperty<int> _characterAttackLevel { get; set; } = new ReactiveProperty<int>(0);//アタックレベル
    private float _attackLevelTime = 7f; //アタックレベルが元に戻るまで
    private CancellationTokenSource _attackLevelCts;

    //移動用
    private Vector3 _previousPos;

    //敵とキャラが接触で受けるダメージ（固定）
    private int _enemyTouchDamage = 1;



    public enum MotionType
    {
        Default,
        Left,
        Right,
        Up,
        Down,
        LeftUp,
        RightUp,
        LeftDown,
        RightDown,
    }
    private MotionType _motionType = MotionType.Default;

    private void Awake()
    {
        _previousHP = _characterHP.Value;
        _characterSpecialPos = _characterSpecialPosTrans.localPosition;
        _initCharacterVelocity = _characterVelocity;

        if (_characterSpineSA != null)
        {
            _characterSpineSA.AnimationState.Event += OnSpineSpecialEvent;
        }

        _characterSpecial.SetActive(false);
        CharacterMoveSet(_motionType);
        
        _characterSpineSA.state.SetAnimation(1, "blink", true);//まばたき合成

        //移動用
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                _characterLocatorRigid.linearVelocity = Vector2.zero;
                if (!_isSpecialActive)
                {
                    CharacterMove();
                }
                
            })
            .AddTo(this);

        //HP
        _characterHP
            .DistinctUntilChanged()
            .Skip(1)//最初の1回目が自動再生されるのを回避
            .Subscribe(hp =>
            {
                Debug.Log("HP: {hp}");
                

                _uICharacterHp.SetHpValue(hp); //UIのHPにセット

                if (_previousHP > hp)//HPが下がった時
                {
                    if (_characterSpecialLevel.Value >= 0 && _characterSpecialLevel.Value < 6)
                    {
                        _characterSpecialLevel.Value += 1;//ダメージうけたらスペシャルレベルがあがる
                    }
                }
                if (_previousHP < hp)//HPが上がった時
                {   
                        Debug.Log("HPがあがった");   
                }


                if (hp <= 0)
                {
                    Debug.Log("ゲームオーバー時");
                }

                _previousHP = hp;
            })
            .AddTo(this);


        //キャラクターの挙動開始タイミングを監視
        _gameMaster._playCharacterSubject
            .Subscribe(_ =>
            {
                //スペシャル
                _characterSpecialLevel
                    .DistinctUntilChanged()
                    .Skip(1)//最初の1回目が自動再生されるのを回避
                    .Subscribe(specialLevel =>
                    {

                        _uICharacterGauge.SpecialGaugeValueSet(specialLevel);

                    })
                    .AddTo(this);

                //被ダメ時（敵の弾オンリー。敵との直接接触はOnTriggerEnter2D内で）
                _getDamageSubject
                    .Subscribe(damage =>
                    {
                        if (_characterHP.Value > 0 && _characterHP.Value <= 10)
                        {
                            GetDamagePoint(damage);
                            SetSpineAnimation(_characterSpineSA,3, "damaged_track3", false,1f);
                        }
                    })
                    .AddTo(this);

                //アタック（スキル）レベルを監視
                _characterAttackLevel
                    .DistinctUntilChanged()//値が同じ場合は無視
                    .Subscribe(attackLevel =>
                    {
                        CharacterAttackSet(attackLevel);
                    });

                //スペースボタンを監視。スペシャル起動
                Observable.EveryUpdate()               　　
                    .Where(_ => Input.GetKeyDown(KeyCode.Space))
                    .Subscribe(_ => {
                        CharacterSpecialSet();
                    })
                    .AddTo(this);

                //左Ctrlを監視。スキル起動
                Observable.EveryUpdate()
               　　 .Where(_ => !_isSpecialActive)//スペシャル中はコントロール不可
                    .Where(_ => Input.GetKeyDown(KeyCode.LeftControl))
                    .Subscribe(_ => {
                        CharacterSkillSet();
                    })
                    .AddTo(this);

                //左Shiftを監視。低速モード
                Observable.EveryUpdate()
               　　 .Where(_ => !_isSpecialActive)//スペシャル中はコントロール不可
                    .Where(_ => Input.GetKeyDown(KeyCode.LeftShift))
                    .Subscribe(_ => {
                        _characterVelocity *= 0.5f;
                        _characterAnimationTimeScale *= 0.7f;
                    })
                    .AddTo(this);
                Observable.EveryUpdate()
                　 .Where(_ => !_isSpecialActive)//スペシャル中はコントロール不可
                   .Where(_ => Input.GetKeyUp(KeyCode.LeftShift))
                   .Subscribe(_ => {
                       _characterVelocity = _initCharacterVelocity;
                       _characterAnimationTimeScale = 1f;
                   })
                   .AddTo(this);

            })
            .AddTo(this);

    }

    private void OnDestroy()
    {
        _attackTween?.Kill();
    }

    private void CharacterMove()
    {
       
     
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.RightUp);
            CharacterMoveRange();
            return;
        }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.LeftUp);
            CharacterMoveRange();
            return;
        }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.RightDown);
            CharacterMoveRange();
            return;
        }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.LeftDown);
            CharacterMoveRange();
            return;
        }

       
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Right);
            CharacterMoveRange();
            return;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Left);
            CharacterMoveRange();
            return;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Up);
            CharacterMoveRange();
            return;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Down);
            CharacterMoveRange();
            return;
        }

        
        CharacterMoveSet(CharacterLocator.MotionType.Default);

    }

    private void CharacterMoveRange()
    {
        if (this.transform.localPosition.x < -18 || this.transform.localPosition.x > 18 )
        {
            this.transform.localPosition = new Vector2(_previousPos.x, this.transform.localPosition.y);
        }
       
        if (this.transform.localPosition.y > 5 || this.transform.localPosition.y < -43)
        {
            this.transform.localPosition = new Vector2(this.transform.localPosition.x,_previousPos.y );
        }
        _previousPos = this.transform.localPosition;
    }

    public void CharacterMoveSet( MotionType motionType)
    {
        switch (motionType)
        {
            case MotionType.Default:
                _characterLocatorRigid.linearVelocity = Vector2.zero;
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true , _characterAnimationTimeScale);
                break;
            case MotionType.Left:
                _characterLocatorRigid.linearVelocity = new Vector2(-1 , 0) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true, _characterAnimationTimeScale);
                break;
            case MotionType.Right:
                _characterLocatorRigid.linearVelocity = new Vector2(1, 0) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true, _characterAnimationTimeScale);
                break;
            case MotionType.Up:
                _characterLocatorRigid.linearVelocity = new Vector2(0, 1) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true, _characterAnimationTimeScale);
                break;
            case MotionType.Down:
                _characterLocatorRigid.linearVelocity = new Vector2(0, -1) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true, _characterAnimationTimeScale);
                break;
            case MotionType.LeftUp:
                _characterLocatorRigid.linearVelocity = new Vector2(-1 , 1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true, _characterAnimationTimeScale);
                break;
            case MotionType.RightUp:
                _characterLocatorRigid.linearVelocity = new Vector2(1 , 1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true, _characterAnimationTimeScale);
                break;
            case MotionType.LeftDown:
                _characterLocatorRigid.linearVelocity = new Vector2(-1, -1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true, _characterAnimationTimeScale);
                break;
            case MotionType.RightDown:
                _characterLocatorRigid.linearVelocity = new Vector2(1 , -1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true, _characterAnimationTimeScale);
                break;
        }
    }

    private async void GetDamagePoint(int damage)
    {      
        this.gameObject.layer = 6;
        _characterHP.Value -= damage;
        await UniTask.Delay(TimeSpan.FromSeconds(_mutekiTime));
        this.gameObject.layer = 3;
    }
   
    public async UniTaskVoid CharacterSpecialSet()//スペシャルの処理
    {

        if(_characterSpecialLevel.Value >= 6 && _isSpecialActive == false)
        {
            _isSpecialActive = true;
            
            _characterSpecialLevel.Value = 0;
            this.gameObject.layer = 6;//無敵レイヤー
            _cutinPlayable.Play();
            CharacterAttackSetStop();//ビット攻撃をいったんストップ
            await UniTask.Delay(TimeSpan.FromSeconds(0.9f));//Completeとれないのでカットイン時間決め打ち

            //_specialTimeの間全画面攻撃。ここにミニキャラ側のスペシャル演出いれる 
            SetSpineAnimation(_characterSpineSA, 0, "special", false, 0.9f);
            Debug.Log("スペシャル！");

            await UniTask.Delay(TimeSpan.FromSeconds(_specialTime));
            _characterSpecial.SetActive(false);
            CharacterAttackSet(_characterAttackLevel.Value);//ビット攻撃を再開
            this.gameObject.layer = 3;//無敵解除
            _isSpecialActive = false;

        }
           
    }
    private void OnSpineSpecialEvent(TrackEntry trackEntry, Spine.Event e)//スペシャル用のイベントキー挙動
    {
        // イベント名で分岐
        if (e.Data.Name == "special_pos")
        {
            //CharacterSpecialPosの位置にキャラを移動
            _characterLocatorTrans.DOLocalMove(_characterSpecialPos, 0.18f).SetEase(Ease.OutCubic);

        }
        if (e.Data.Name == "special_eff")
        {
            //スペシャルエフェクト
            _characterSpecial.SetActive(true);//全画面コリジョンON
            _characterSpecialEffectParticle.Play();
            
        }
    }

    public async UniTaskVoid CharacterSkillSet()//スキル処理
    {
        //HP自傷
        _characterHP.Value -= 1;

        //Attackレベルをあげる
        if(_characterAttackLevel.Value < 4)
        {
            _characterAttackLevel.Value += 1;
        }

        //キャンセル用トークン
        _attackLevelCts?.Cancel();//前の処理をキャンセル
        _attackLevelCts = new CancellationTokenSource();
        var token = _attackLevelCts.Token;

        DecreaseAttackLevelOverTime(token).Forget();

    }
    private async UniTaskVoid DecreaseAttackLevelOverTime(CancellationToken token)
    {
        try
        {
            while (_characterAttackLevel.Value > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_attackLevelTime), cancellationToken: token);

                if (_characterAttackLevel.Value > 0)
                {
                    _characterAttackLevel.Value -= 1;
                }
            }
        }
        catch (OperationCanceledException)
        {
            //例外
        }
    }


    public void CharacterAttackSet(int characterAttackLevel)
    {
        //アタックレベルで攻撃変わる
        Debug.Log("アタックレベル" +characterAttackLevel);
        switch (characterAttackLevel)
        {
            case 0:
                _characterAttackObject[0].Play();
                _characterAttackObject[1].Stop();
                _characterAttackObject[2].Stop();
                _characterAttackObject[3].Stop();
                _characterAttackObject[4].Stop();
                CharacterAttackBitSet(0);

                break;
            case 1:
                _characterAttackObject[0].Stop();
                _characterAttackObject[1].Play();
                _characterAttackObject[2].Stop();
                _characterAttackObject[3].Stop();
                _characterAttackObject[4].Stop();
                CharacterAttackBitSet(1);

                break;
            case 2:
                _characterAttackObject[0].Stop();
                _characterAttackObject[1].Stop();
                _characterAttackObject[2].Play();
                _characterAttackObject[3].Stop();
                _characterAttackObject[4].Stop();
                CharacterAttackBitSet(2);

                break;
            case 3:
                _characterAttackObject[0].Stop();
                _characterAttackObject[1].Stop();
                _characterAttackObject[2].Stop();
                _characterAttackObject[3].Play();
                _characterAttackObject[4].Stop();
                CharacterAttackBitSet(3);

                break;
            case 4:
                _characterAttackObject[0].Stop();
                _characterAttackObject[1].Stop();
                _characterAttackObject[2].Stop();
                _characterAttackObject[3].Stop();
                _characterAttackObject[4].Play();
                CharacterAttackBitSet(4);

                break;
        }
    }
    private void CharacterAttackSetStop()
    {
        _characterAttackObject[0].Stop();
        _characterAttackObject[1].Stop();
        _characterAttackObject[2].Stop();
        _characterAttackObject[3].Stop();
        _characterAttackObject[4].Stop();
        CharacterAttackBitSetStop();
    }

    private void CharacterAttackBitSet(int number)//キャラのビットの表示をセット
    {
        foreach (TransformFadeGroup bitFadeGroup in _bitsFadeGroup)
        {
            bitFadeGroup._spriteAlpha.Value = 0;
        }

        if (_attackTween != null) { _attackTween.Kill(); }

        _attackTween = DOTween.To
            (
                 () => _bitsFadeGroup[number]._spriteAlpha.Value,
                 x => _bitsFadeGroup[number]._spriteAlpha.Value = x,
                 1f,
                 0.5f
            );
    }
    private void CharacterAttackBitSetStop()//キャラのビットを非表示
    {
        foreach (TransformFadeGroup bitFadeGroup in _bitsFadeGroup)
        {
            bitFadeGroup._spriteAlpha.Value = 0;
        }

    }

    private void SetSpineAnimation(SkeletonAnimation skeletonAnimation,int trackNumber , String animationName,bool loop , float timeScale)
    {
        //キャラ用汎用アニメーションメソッド
        skeletonAnimation.state.TimeScale = timeScale;
        if (!IsPlayingAnimation(skeletonAnimation, animationName, trackNumber))
        {
            skeletonAnimation.state.SetAnimation(trackNumber, animationName, loop);     
        }
       
    }
    bool IsPlayingAnimation(SkeletonAnimation skeleton, string animationName, int trackIndex = 0)
    {
        var current = skeleton.AnimationState.GetCurrent(trackIndex);
        return current != null && current.Animation != null && current.Animation.Name == animationName;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "EnemyBody")//敵との直接接触用
        {
            if (_characterHP.Value > 0 && _characterHP.Value <= 10)
            {
                GetDamagePoint(_enemyTouchDamage);
                SetSpineAnimation(_characterSpineSA, 3, "damaged_track3", false, 1f);
                Debug.Log("エネミーに接触");
            }
        }

        if (collision.gameObject.tag == "HPItem")//敵との直接接触用
        {
            if (_characterHP.Value > 0 && _characterHP.Value <= 10)
            {
                GetHPItemPoint();
                Destroy(collision.gameObject);
                Debug.Log("HPアイテム取得");
            }
        }

    }

    private void GetHPItemPoint()
    {    
        _characterHP.Value += 1;
    }


}
