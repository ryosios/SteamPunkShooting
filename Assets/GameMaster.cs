using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private CharacterLocator _characterLocator;
    [SerializeField] private BackGroundMaker _backGroundMaker;
    [SerializeField] private UICharacterHp _uICharacterHp;
    [SerializeField] private UICharacterGauge _uICharacterGauge;
    [SerializeField] private UICharacterAttackLevel _uICharacterAttackLevel;
    private int previousHp;
    private float previousSpecialLevel;
    private float previousAttackLevel;
    // Start is called before the first frame update
    void Awake()
    {
        previousHp = _characterLocator._characterHP;
        previousSpecialLevel = _characterLocator._characterSpecialLevel;
        previousAttackLevel = _characterLocator._characterAttackLevel;
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMove();//キャラクター移動
        

        CharacterHP();//キャラクターHP
        CharacterSpacialGauge();//キャラクタースペシャルUIゲージ
        CharacterSkill();   
        CharacterSpecial();//キャラクタースペシャル
        CharacterAttack();//攻撃セットを選ぶ

        BackGround();


    }

    private void BackGround()
    {
        _backGroundMaker.CreateBgObject();
    }

    private void CharacterMove()
    {
        _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.Default);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.Right); } 
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.Left); }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.Up); }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.Down); }
        if ((Input.GetKey(KeyCode.D)&& Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.RightArrow)&& Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.RightUp); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.LeftUp); }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.RightDown); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMoveSet(CharacterLocator.MotionType.LeftDown); }
    }

    private void CharacterHP()
    {
        if (previousHp != _characterLocator._characterHP)
        {
            _uICharacterHp.SetHpValue(_characterLocator._characterHP);//現在のHPをセットする
            previousHp = _characterLocator._characterHP;
        }
    }

    private void CharacterSpacialGauge()
    {
        if (previousSpecialLevel != _characterLocator._characterSpecialLevel)
        {
            _uICharacterGauge.GaugeValueSet(_characterLocator._characterSpecialLevel);//現在のスペシャルゲージをセットする
            previousSpecialLevel = _characterLocator._characterSpecialLevel;
        }
    }

    private void CharacterSpecial()
    {
        //キャラクターにスペシャルをうたせる
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _characterLocator.CharacterSpecialSet();
        }

    }

    private void CharacterSkill() //自壊スキル
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _characterLocator.CharacterSkillSet();
        }         
    
    }
    private void CharacterAttack()
    {
        if(previousAttackLevel != _characterLocator._characterAttackLevel)
        {
            _characterLocator.CharacterAttackSet();
            previousAttackLevel = _characterLocator._characterAttackLevel;
            CharacterAttackGauge();
            Debug.Log(_characterLocator._characterAttackLevel);
        }

    }

    private void CharacterAttackGauge()
    {
        _uICharacterAttackLevel.CharacterAttackGaugeSet((int)_characterLocator._characterAttackLevel);
    }
}
