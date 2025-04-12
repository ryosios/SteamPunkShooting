using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UICharacterAttackLevel : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Slider _uICharacterAttackLevelSlider;
    [SerializeField] private CharacterLocator _characterLocator;

    private void Awake()
    {
        CharacterAttackGaugeSet(_characterLocator._characterAttackLevel.Value);

        _characterLocator._characterAttackLevel
            .DistinctUntilChanged()//“¯‚¶’l‚È‚ç–³‹
            .Subscribe(attackLevel => //’l‚ªˆø”‚Å©“®‚Å“ü‚é
            {
                CharacterAttackGaugeSet(attackLevel);
            });
    }
        public void CharacterAttackGaugeSet( int characterAttackLevel)
    {
        _uICharacterAttackLevelSlider.value = (float)characterAttackLevel;
    }
}
