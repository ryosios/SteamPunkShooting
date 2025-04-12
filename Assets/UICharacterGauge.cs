using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UICharacterGauge : MonoBehaviour
{

    [SerializeField] private Image GaugeImage;
    [SerializeField] private CharacterLocator _characterLocator;
    // Start is called before the first frame update

    private void Awake()
    {
        GaugeImage.fillAmount = _characterLocator._characterSpecialLevel.Value;
        _characterLocator._characterSpecialLevel
            .DistinctUntilChanged()//“¯‚¶’l‚È‚ç–³‹
            .Subscribe(specialLevel => //’l‚ªˆø”‚Å©“®‚Å“ü‚é
            {
                GaugeValueSet(specialLevel);
            });
    }
    public void GaugeValueSet(float specialLevel)
    {
        if(GaugeImage.fillAmount <= 1)
        {
            GaugeImage.fillAmount = specialLevel;
        }
        
    }

}
