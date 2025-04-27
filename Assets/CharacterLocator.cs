using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using UniRx;
using Spine.Unity;
using System.Threading;


public class CharacterLocator : MonoBehaviour
{
   
    [SerializeField] private Rigidbody2D _characterLocatorRigid;
    [SerializeField] private UICharacterGauge _uICharacterGauge;
    [SerializeField] private UICharacterHp _uICharacterHp;
    [SerializeField] private GameObject _characterSpecial;
    [SerializeField] private SkeletonAnimation _characterSpineSA;

    [Header("AttackType")]
    [SerializeField] private GameObject[] _characterAttackObject ;


    private float _characterVelocity { get; set; } = 5f;

    //HP管理用
    public ReactiveProperty<int> _characterHP { get; set; } = new ReactiveProperty<int>(5);
    public Subject<int> _getDamageSubject = new Subject<int>();//ダメージ受けたとき

    //スペシャル用
    public ReactiveProperty<int> _characterSpecialLevel { get; set; } = new ReactiveProperty<int>(0); //スペシャルレベル
    public Subject<Unit> _playSpecialSubject = new Subject<Unit>();//スぺシャル撃ったとき
    private float _specialTime = 2f; //スペシャルの効果時間
    private bool _isSpecialActive = false;
 
    //スキル。レベルがあがるとアタックが強化される
    public ReactiveProperty<int> _characterAttackLevel { get; set; } = new ReactiveProperty<int>(0);//アタックレベル
    private float _attackLevelTime = 7f; //アタックレベルが元に戻るまで
    private CancellationTokenSource _attackLevelCts;

    //移動用
    private Vector3 _previousPos;



    private float _mutekiTime = 1f;

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
        
        _characterSpecial.SetActive(false);
        CharacterMoveSet(_motionType);
        CharacterAttackSet(_characterAttackLevel.Value);//
        _characterSpineSA.state.SetAnimation(1, "blink", true);//まばたき合成

        //移動用
        Observable.EveryUpdate()
            .Subscribe(_ => {
                CharacterMove();
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
                if (_characterSpecialLevel.Value >= 0 && _characterSpecialLevel.Value < 6)
                {
                    _characterSpecialLevel.Value += 1;//ダメージうけたらスペシャルレベルがあがる
                }

                if (hp <= 0)
                {
                    Debug.Log("ゲームオーバー時");
                }
            })
            .AddTo(this);

        //スペシャル
        _characterSpecialLevel
            .DistinctUntilChanged()
            .Skip(1)//最初の1回目が自動再生されるのを回避
            .Subscribe(specialLevel =>
            {
                
                _uICharacterGauge.SpecialGaugeValueSet(specialLevel);

            })
            .AddTo(this);

        //被ダメ時
        _getDamageSubject
            .Subscribe(damage => 
            {
                if (_characterHP.Value > 0 && _characterHP.Value <= 10)
                {
                    GetDamagePoint(damage);
                   
                }
            })
            .AddTo(this);


   

        //スペシャル起動を監視
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                CharacterSpecialSet();
            })
            .AddTo(this);

        //スキル起動を監視
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftControl))
            .Subscribe(_ => {
                CharacterSkillSet();
            })
            .AddTo(this);

        //アタック（スキル）レベルを監視
        _characterAttackLevel
            .DistinctUntilChanged()//値が同じ場合は無視
            .Subscribe(attackLevel => 
            {
                CharacterAttackSet(attackLevel);
            });

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
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true);
                break;
            case MotionType.Left:
                _characterLocatorRigid.linearVelocity = new Vector2(-1 , 0) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true);
                break;
            case MotionType.Right:
                _characterLocatorRigid.linearVelocity = new Vector2(1, 0) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true);
                break;
            case MotionType.Up:
                _characterLocatorRigid.linearVelocity = new Vector2(0, 1) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true);
                break;
            case MotionType.Down:
                _characterLocatorRigid.linearVelocity = new Vector2(0, -1) * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_forwardback", true);
                break;
            case MotionType.LeftUp:
                _characterLocatorRigid.linearVelocity = new Vector2(-1 , 1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true);
                break;
            case MotionType.RightUp:
                _characterLocatorRigid.linearVelocity = new Vector2(1 , 1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true);
                break;
            case MotionType.LeftDown:
                _characterLocatorRigid.linearVelocity = new Vector2(-1, -1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_left", true);
                break;
            case MotionType.RightDown:
                _characterLocatorRigid.linearVelocity = new Vector2(1 , -1).normalized * _characterVelocity;
                SetSpineAnimation(_characterSpineSA, 0, "run_right", true);
                break;
        }
    }

    private async void GetDamagePoint(int damage)
    {
        _characterHP.Value -= damage;
        this.gameObject.layer = 6;
        await UniTask.Delay(TimeSpan.FromSeconds(_mutekiTime));
        this.gameObject.layer = 3;
    }
  

    public async UniTaskVoid CharacterSpecialSet()//スペシャルの処理
    {

        if(_characterSpecialLevel.Value >= 6 && _isSpecialActive == false)
        {
            _isSpecialActive = true;

            _characterSpecialLevel.Value = 0;
            //_specialTimeの間全画面攻撃
            _characterSpecial.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_specialTime));
            _characterSpecial.SetActive(false);
            _isSpecialActive = false;

        }
           
    }

    public async UniTaskVoid CharacterSkillSet()//スキル処理
    {
        //HP自傷
        _characterHP.Value -= 1;

        //Attackレベルをあげる
        if(_characterAttackLevel.Value < 5)
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
        switch (characterAttackLevel)
        {
            case 0:
                _characterAttackObject[0].SetActive(true);
                _characterAttackObject[1].SetActive(false);
                _characterAttackObject[2].SetActive(false);
                _characterAttackObject[3].SetActive(false);
                _characterAttackObject[4].SetActive(false);

                break;
            case 1:
                _characterAttackObject[0].SetActive(true);
                _characterAttackObject[1].SetActive(true);
                _characterAttackObject[2].SetActive(false);
                _characterAttackObject[3].SetActive(false);
                _characterAttackObject[4].SetActive(false);

                break;
            case 2:
                _characterAttackObject[0].SetActive(true);
                _characterAttackObject[1].SetActive(true);
                _characterAttackObject[2].SetActive(true);
                _characterAttackObject[3].SetActive(false);
                _characterAttackObject[4].SetActive(false);

                break;
            case 3:
                _characterAttackObject[0].SetActive(true);
                _characterAttackObject[1].SetActive(true);
                _characterAttackObject[2].SetActive(true);
                _characterAttackObject[3].SetActive(true);
                _characterAttackObject[4].SetActive(false);

                break;
            case 4:
                _characterAttackObject[0].SetActive(true);
                _characterAttackObject[1].SetActive(true);
                _characterAttackObject[2].SetActive(true);
                _characterAttackObject[3].SetActive(true);
                _characterAttackObject[4].SetActive(true);

                break;
        }
    }

    private void SetSpineAnimation(SkeletonAnimation skeletonAnimation,int trackNumber , String animationName,bool loop)
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"{collision.gameObject.name} とぶつかった！");
    }
}
