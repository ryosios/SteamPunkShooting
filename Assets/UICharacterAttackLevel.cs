using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterAttackLevel : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Slider _uICharacterAttackLevelSlider;
    public void CharacterAttackGaugeSet(int AttackLevel)
    {
        _uICharacterAttackLevelSlider.value = AttackLevel;
    }
}
