using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private CharacterLocator _characterLocator;
    [SerializeField] private BackGroundMaker _backGroundMaker;
    [SerializeField] private UICharacterHp _uICharacterHp;

    int previousHp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CharacterMoveSet();//キャラクター移動

        CharacterHPSet();


        _backGroundMaker.CreateBgObject();
    }

    private void CharacterMoveSet()
    {
        _characterLocator.CharacterMove(CharacterLocator.MotionType.Default);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.Right); } 
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.Left); }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.Up); }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.Down); }
        if ((Input.GetKey(KeyCode.D)&& Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.RightArrow)&& Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.RightUp); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.LeftUp); }
        if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.RightDown); }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow)) { _characterLocator.CharacterMove(CharacterLocator.MotionType.LeftDown); }
    }

    private void CharacterHPSet()
    {
        if (previousHp != _characterLocator._characterHP)
        {
            _uICharacterHp.SetHpValue(_characterLocator._characterHP);//現在のHPをセットする
            previousHp = _characterLocator._characterHP;
        }
    }
}
