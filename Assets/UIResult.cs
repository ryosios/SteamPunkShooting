using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIResult : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] _pointText;
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetPoint(List<int> _pointsList)
    {
        for(int i =0; i< _pointsList.Count;i++)
        {
            _pointText[i].text = _pointsList[i].ToString("D9");
        }
        

    }

}
