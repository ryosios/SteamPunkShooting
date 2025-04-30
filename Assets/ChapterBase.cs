using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public abstract class ChapterBase : MonoBehaviour
{
    public abstract ReactiveProperty<int> _selectNumber { get; set; } 
   
    public abstract void SetChapter(int selectNumber);

}
