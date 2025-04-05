using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterGauge : MonoBehaviour
{

    [SerializeField] private Image GaugeImage;

    // Start is called before the first frame update

    private void Awake()
    {
        GaugeImage.fillAmount = 0;
    }
    public void GaugeValueSet()
    {
        if(GaugeImage.fillAmount < 1)
        {
            GaugeImage.fillAmount += 0.2f;
        }
        
    }

    public void GaugeValueReSet()
    { 
            GaugeImage.fillAmount = 0f;
    }

}
