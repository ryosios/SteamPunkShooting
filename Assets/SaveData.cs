using NUnit.Framework;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<int> _resultPoint = new List<int>(5);//スコア表示用のリスト。５個まで保存
    public int _finalPoint = 0;//最終ポイントとして加算されていく用
    public int _nowStage = 0;//現在のステージ
    public bool _isStageCleared = false;//クリアしたかどうかのフラグ

}