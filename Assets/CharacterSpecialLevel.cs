using System.Collections;
using System.Collections.Generic;
using System;

class CharacterSpecialLevel : ReactiveProperty<int>
{
    public const int MAX_SPECIAL_LEVEL = 6;

    /// <summary>
    /// スペシャルレベルをリセットする
    /// </summary>
    public void Reset()
    {
        this.Value = 0;
    }

    /// <summary>
    /// スペシャルレベルの値を設定する
    /// </summary>
    /// <param name="Value">スペシャルレベル</param>
    public void Set(int Value)
    {
        this.Value = math_between(0, MAX_SPECIAL_LEVEL, Value);
    }

    /// <summary>
    /// スペシャルレベルを一段階増やす。最大値以上にはならない
    /// </summary>
    public void Increment()
    {
        if (this.Value < MAX_SPECIAL_LEVEL)
        {
            this.Value++;
        }
    }

    /// <summary>
    /// スペシャルレベルが最大である
    /// </summary>
    /// <returns>最大値であればtrue</returns>
    public bool IsMaxLevel()
    {
        return this.Value == MAX_SPECIAL_LEVEL;
    }

    private int math_between(int min, int max, int value)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}
