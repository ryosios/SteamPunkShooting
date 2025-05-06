using NUnit.Framework;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<int> _stage1AndPoint = new List<int>(5);
    public int _nowStage = 0;
    public bool _isStageCleared = false;

}