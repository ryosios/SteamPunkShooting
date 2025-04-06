using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class CharacterLocator : MonoBehaviour
{
   
    [SerializeField] private Rigidbody2D _characterLocatorRigid;
    [SerializeField] private UICharacterGauge _uICharacterGauge;
    [SerializeField] private GameObject _characterSpecial;

    [Header("AttackType")]
    [SerializeField] private GameObject[] _characterAttackObject ;


    private float _characterVelocity { get; set; } = 1f;
    public int _characterHP { get; set; } = 5;
    public float _characterSpecialLevel { get; set; } = 0;//スペシャルゲージ。最大1。0.1ずつ上昇。
    public float _characterAttackLevel { get; set; } = 0;//弾のレベル

    private float _specialTime = 2f; //2秒間スキルで弾を消す
    private bool _isSpecialActive = false;

    private float _skillTime = 7f; //7秒間スキルで弾を消す

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
       
    }

    public void CharacterMoveSet( MotionType motionType)
    {
        switch (motionType)
        {
            case MotionType.Default:
                _characterLocatorRigid.velocity = Vector2.zero;
                break;
            case MotionType.Left:
                _characterLocatorRigid.velocity = new Vector2(-1 , 0) * _characterVelocity;
                break;
            case MotionType.Right:
                _characterLocatorRigid.velocity = new Vector2(1, 0) * _characterVelocity;
                break;
            case MotionType.Up:
                _characterLocatorRigid.velocity = new Vector2(0, 1) * _characterVelocity;
                break;
            case MotionType.Down:
                _characterLocatorRigid.velocity = new Vector2(0, -1) * _characterVelocity;
                break;
            case MotionType.LeftUp:
                _characterLocatorRigid.velocity = new Vector2(-1 , 1).normalized * _characterVelocity;
                break;
            case MotionType.RightUp:
                _characterLocatorRigid.velocity = new Vector2(1 , 1).normalized * _characterVelocity;
                break;
            case MotionType.LeftDown:
                _characterLocatorRigid.velocity = new Vector2(-1, -1).normalized * _characterVelocity;
                break;
            case MotionType.RightDown:
                _characterLocatorRigid.velocity = new Vector2(1 , -1).normalized * _characterVelocity;
                break;
        }
    }

    public async UniTaskVoid CharacterSpecialSet()
    {

        if(_characterSpecialLevel >= 1 && _isSpecialActive == false)
        {
            _isSpecialActive = true;
            //ゲージ消費
            _uICharacterGauge.GaugeValueReSet();
            //スキル処理
            _characterSpecial.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_specialTime));
            _characterSpecial.SetActive(false);
            _isSpecialActive = false;
            Debug.Log("CharacterSkill終了");
        }
           
    }

    public async UniTaskVoid CharacterSkillSet()
    {
        //HPを1減らす
        _characterHP -= 1;
        //Attackレベルをあげる。0～4
        if(_characterAttackLevel < 4)
        {
            _characterAttackLevel += 1;
        }
        //弾を増やすのはCharacterAttackで
        //一定時間後にAttackレベルを下げる
        await UniTask.Delay(TimeSpan.FromSeconds(_skillTime));
        if (_characterAttackLevel > 0)
        {
            _characterAttackLevel -= 1;
        }

    }

    public void CharacterAttackSet()
    {
        //基本垂れ流し
        //アタックレベルによって弾を変える
        switch (_characterAttackLevel)
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
