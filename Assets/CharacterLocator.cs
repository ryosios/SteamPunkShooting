using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocator : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _characterLocatorRigid;
    [SerializeField] private UICharacterGauge _uICharacterGauge;


    private float _characterVelocity { get; set; } = 1f;
    public int _characterHP { get; set; } = 5;

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
        CharacterMove(_motionType);
       
    }

    public void CharacterMove( MotionType motionType)
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

    public void CharacterSpecial()
    {
        //ãZèàóù

        //ÉQÅ[ÉWè¡îÔ
        _uICharacterGauge.GaugeValueReSet();
    }

}
