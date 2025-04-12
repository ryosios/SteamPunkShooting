using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using UniRx;


public class CharacterLocator : MonoBehaviour
{
   
    [SerializeField] private Rigidbody2D _characterLocatorRigid;
    [SerializeField] private UICharacterGauge _uICharacterGauge;
    [SerializeField] private GameObject _characterSpecial;

    [Header("AttackType")]
    [SerializeField] private GameObject[] _characterAttackObject ;


    private float _characterVelocity { get; set; } = 1000f;

    //Hp関連
    public ReactiveProperty<int> _characterHP { get; set; } = new ReactiveProperty<int>(5);
    public Subject<int> _getDamageSubject = new Subject<int>();

    //スペシャル関連
    public ReactiveProperty<float> _characterSpecialLevel { get; set; } = new ReactiveProperty<float>(0); //スペシャルゲージ。最大1。0.1ずつ上昇。
    public Subject<float> _getSpecialLevelSubject = new Subject<float>();
    public Subject<Unit> _playSpecialSubject = new Subject<Unit>();
    private float _specialTime = 2f; //2秒間スペシャルで弾を消す
    private bool _isSpecialActive = false;
 
    //スキル関連
    public ReactiveProperty<int> _characterAttackLevel { get; set; } = new ReactiveProperty<int>(0);//弾のレベル
    private float _attackLevelTime = 7f; //7秒間たったらアタックレベルをさげる





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
        CharacterAttackSet(_characterAttackLevel.Value);

        //移動
        Observable.EveryUpdate()
            .Subscribe(_ => {
                CharacterMove();
            })
            .AddTo(this);

        //ダメージ監視
        _getDamageSubject
            .Subscribe(damage => 
            {
                if (_characterHP.Value > 0 && _characterHP.Value <= 10)
                {
                    GetDamagePoint(damage);           
                }
            })
            .AddTo(this);
        //スペシャルレベル監視
        _getSpecialLevelSubject
            .Subscribe(specialPoint =>
            {
                if (_characterSpecialLevel.Value >= 0 && _characterSpecialLevel.Value < 1)
                {
                    GetSpecialPoint(specialPoint);
                }
            })
            .AddTo(this);

        //スペシャル
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                CharacterSpecialSet();
            })
            .AddTo(this);

        //スキル
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftControl))
            .Subscribe(_ => {
                CharacterSkillSet();
            })
            .AddTo(this);

        //アタック種類変更
        _characterAttackLevel
            .DistinctUntilChanged()//同じ値なら無視
            .Subscribe(attackLevel => //値が引数で自動で入る
            {
                CharacterAttackSet(attackLevel);
            });

    }

    private void CharacterMove()
    {
        CharacterMoveSet(CharacterLocator.MotionType.Default);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {CharacterMoveSet(CharacterLocator.MotionType.Right); }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {CharacterMoveSet(CharacterLocator.MotionType.Left); }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {CharacterMoveSet(CharacterLocator.MotionType.Up); }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {CharacterMoveSet(CharacterLocator.MotionType.Down); }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)) {CharacterMoveSet(CharacterLocator.MotionType.RightUp); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)) {CharacterMoveSet(CharacterLocator.MotionType.LeftUp); }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) {CharacterMoveSet(CharacterLocator.MotionType.RightDown); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)) {CharacterMoveSet(CharacterLocator.MotionType.LeftDown); }
    }
    public void CharacterMoveSet( MotionType motionType)
    {
        switch (motionType)
        {
            case MotionType.Default:
                _characterLocatorRigid.velocity = Vector2.zero;
                break;
            case MotionType.Left:
                _characterLocatorRigid.velocity = new Vector2(-1 , 0) * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.Right:
                _characterLocatorRigid.velocity = new Vector2(1, 0) * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.Up:
                _characterLocatorRigid.velocity = new Vector2(0, 1) * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.Down:
                _characterLocatorRigid.velocity = new Vector2(0, -1) * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.LeftUp:
                _characterLocatorRigid.velocity = new Vector2(-1 , 1).normalized * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.RightUp:
                _characterLocatorRigid.velocity = new Vector2(1 , 1).normalized * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.LeftDown:
                _characterLocatorRigid.velocity = new Vector2(-1, -1).normalized * _characterVelocity * Time.deltaTime;
                break;
            case MotionType.RightDown:
                _characterLocatorRigid.velocity = new Vector2(1 , -1).normalized * _characterVelocity * Time.deltaTime;
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
    private void GetSpecialPoint(float specialPoint)
    {
        _characterSpecialLevel.Value += specialPoint;
    }

    public async UniTaskVoid CharacterSpecialSet()
    {

        if(_characterSpecialLevel.Value >= 1 && _isSpecialActive == false)
        {
            _isSpecialActive = true;

            _characterSpecialLevel.Value = 0f;
            //スキル処理
            _characterSpecial.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_specialTime));
            _characterSpecial.SetActive(false);
            _isSpecialActive = false;

        }
           
    }

    public async UniTaskVoid CharacterSkillSet()
    {
        //HPを1減らす
        _characterHP.Value -= 1;
        //Attackレベルをあげる。0～5
        if(_characterAttackLevel.Value < 5)
        {
            _characterAttackLevel.Value += 1;
        }
        //弾を増やすのはCharacterAttackで
        //一定時間後にAttackレベルを下げる
        await UniTask.Delay(TimeSpan.FromSeconds(_attackLevelTime));
        if (_characterAttackLevel.Value > 0)
        {
            _characterAttackLevel.Value -= 1;
        }

    }

    public void CharacterAttackSet(int characterAttackLevel)
    {
        //アタックレベルによって弾を変える
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

}
