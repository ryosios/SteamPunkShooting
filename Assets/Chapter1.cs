using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Chapter1 : ChapterBase
{
    public override ReactiveProperty<int> _selectNumber { get; set; } = new ReactiveProperty<int>(-1);

    public override void SetChapter(int selectNumber)
    {
        
        switch (selectNumber)
        {
            case 0://待機時間用
                WaitStartAsync(1f,_destroyToken).Forget(); 
                break;
            case 1://動作開始
                for (int i = 0; i < _attackParticles.Length; i++)
                {
                    _attackParticles[i].Play();//攻撃パーティクルを開始
                  
                }

                _sequence = DOTween.Sequence();
                for (int i = 0; i < _enemys.Length; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        _enemys[i].localPosition = new Vector3(_initEnemyPos[i].x, _initEnemyPos[i].y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(_initEnemyPos[i].x, _initEnemyPos[i].y, 0f),
                            new Vector3(-2.48f + 2.48f* i,-2.83f,0f),
                            //new Vector3(2.41f,5.71f,0f),

                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));
                       
                    }
                   
                }
                _sequence.OnUpdate(() =>
                {
                    //敵が全員負けリストに入ったら強制次チャプターへ
                    NextChapterCompleteEnemy().Forget();
                });
                _sequence.OnComplete(() =>
                { 
                    _selectNumber.Value += 1;

                });
                _sequence.Play();
                break;

            case 2:
                _sequence.Kill();
                _sequence = DOTween.Sequence();

                for (int i = 0; i < _enemys.Length; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(-2.48f + 2.48f* i,-2.83f,0f),
                            new Vector3(-2.48f + 2.48f* i,5.83f,0f),
                           

                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));         
                    }                  
                }
                _sequence.OnUpdate(() =>
                {
                    NextChapterCompleteEnemy().Forget();
                });
                _sequence.OnComplete(() =>
                {
                    NextChapter(_destroyToken).Forget();
                });
                _sequence.Play();
                break;
        }
    }
}
