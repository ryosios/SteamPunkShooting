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

    //Hp�֘A
    public ReactiveProperty<int> _characterHP { get; set; } = new ReactiveProperty<int>(5);
    public Subject<int> _getDamageSubject = new Subject<int>();//��e�C�x���g

    //�X�y�V�����֘A
    public ReactiveProperty<int> _characterSpecialLevel { get; set; } = new ReactiveProperty<int>(0); //�X�y�V�����Q�[�W�B���x��0�`6�B40�x�����݁B
    public Subject<Unit> _playSpecialSubject = new Subject<Unit>();//�X�y�V�����������Ƃ��̃C�x���g
    private float _specialTime = 2f; //2�b�ԃX�y�V�����Œe������
    private bool _isSpecialActive = false;
 
    //�X�L���֘A
    public ReactiveProperty<int> _characterAttackLevel { get; set; } = new ReactiveProperty<int>(0);//�L�����N�^�[�̒e�̃��x��
    private float _attackLevelTime = 7f; //7�b�Ԃ�������A�^�b�N���x����������
    private CancellationTokenSource _attackLevelCts;





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
        CharacterAttackSet(_characterAttackLevel.Value);//�A�^�b�N���x���ɂ���Ēe��ς���B�����ݒ�B
        _characterSpineSA.state.SetAnimation(1, "blink", true);//�܂΂����A�j���[�V�������g���b�N1�ɍ���

        //�ړ�
        Observable.EveryUpdate()
            .Subscribe(_ => {
                CharacterMove();
            })
            .AddTo(this);

        //HP�Ď�
        _characterHP
            .DistinctUntilChanged()
            .Skip(1)//�����Έ��Ă΂��΍�
            .Subscribe(hp =>
            {
                Debug.Log($"�L������HP���ς������I����HP: {hp}");

                _uICharacterHp.SetHpValue(hp); //UI�ɃZ�b�g
                if (_characterSpecialLevel.Value >= 0 && _characterSpecialLevel.Value < 6)
                {
                    _characterSpecialLevel.Value += 1;//�X�y�V�������x��+1
                }

                if (hp <= 0)
                {
                    Debug.Log("�L���������ꂽ�I");
                }
            })
            .AddTo(this);

        //�X�y�V�������x���Ď��i0�`6�j
        _characterSpecialLevel
            .DistinctUntilChanged()
            .Skip(1)//�����Έ��Ă΂��΍�
            .Subscribe(specialLevel =>
            {
                Debug.Log($"�K�E�Z�̃��x�����ς������I����SpecialLevel: {specialLevel}");
                _uICharacterGauge.SpecialGaugeValueSet(specialLevel);

            })
            .AddTo(this);

        //�_���[�W�Ď��i�L�����N�^�[����e���j
        _getDamageSubject
            .Subscribe(damage => 
            {
                if (_characterHP.Value > 0 && _characterHP.Value <= 10)
                {
                    GetDamagePoint(damage);
                   
                }
            })
            .AddTo(this);


   

        //�X�y�V����
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                CharacterSpecialSet();
            })
            .AddTo(this);

        //�X�L��
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftControl))
            .Subscribe(_ => {
                CharacterSkillSet();
            })
            .AddTo(this);

        //�A�^�b�N��ޕύX
        _characterAttackLevel
            .DistinctUntilChanged()//�����l�Ȃ疳��
            .Subscribe(attackLevel => //�l�������Ŏ����œ���
            {
                CharacterAttackSet(attackLevel);
            });

    }

    private void CharacterMove()
    {
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.RightUp);
            return;
        }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.LeftUp);
            return;
        }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.RightDown);
            return;
        }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            CharacterMoveSet(CharacterLocator.MotionType.LeftDown);
            return;
        }

        // ���ɒP�̕���
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Right);
            return;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Left);
            return;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Up);
            return;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            CharacterMoveSet(CharacterLocator.MotionType.Down);
            return;
        }

        // �ǂ̃L�[��������Ă��Ȃ�������~�܂�
        CharacterMoveSet(CharacterLocator.MotionType.Default);

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
  

    public async UniTaskVoid CharacterSpecialSet()//�X�y�V�����̏���
    {

        if(_characterSpecialLevel.Value >= 6 && _isSpecialActive == false)
        {
            _isSpecialActive = true;

            _characterSpecialLevel.Value = 0;
            //�X�y�V��������
            _characterSpecial.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_specialTime));
            _characterSpecial.SetActive(false);
            _isSpecialActive = false;

        }
           
    }

    public async UniTaskVoid CharacterSkillSet()//�X�L�������B�X�L�����g������g�o����ăA�^�b�N���x����������B
    {
        //HP��1���炷
        _characterHP.Value -= 1;

        //Attack���x����������B0�`5
        if(_characterAttackLevel.Value < 5)
        {
            _characterAttackLevel.Value += 1;
        }
        //�e�𑝂₷�̂�CharacterAttack��
        //��莞�Ԍ��Attack���x����������

        // �O��̃L�����Z�������i����ꍇ�j
        _attackLevelCts?.Cancel();
        _attackLevelCts = new CancellationTokenSource();
        var token = _attackLevelCts.Token;

        // �V���������������X�^�[�g�i�J��Ԃ��j
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
            // �L�����Z�����ꂽ�Ƃ��͉������Ȃ�
        }
    }

    public void CharacterAttackSet(int characterAttackLevel)
    {
        //�A�^�b�N���x���ɂ���Ēe��ς���
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
}
