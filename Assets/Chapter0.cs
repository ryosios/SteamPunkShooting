using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class Chapter0 : ChapterBase
{
    public override ReactiveProperty<int> _selectNumber { get; set; } = new ReactiveProperty<int>(0);

    public override void SetChapter(int selectNumber)
    { /*

        switch (selectNumber)
        {
            case 0:
                break;
            case 1:
                _sequence = DOTween.Sequence();
                for (int i = 0; i < _enemys.Count; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        _enemys[i].localPosition = new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f),
                            new Vector3(2.41f,-0.5f,0f),
                            //new Vector3(2.41f,5.71f,0f),

                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));

                    }

                }
                _sequence.OnUpdate(() =>
                {

                    NextChapterSet();
                });
                _sequence.OnComplete(() =>
                {
                    Debug.Log("koko");

                    _selectNumber.Value += 1;
                });
                _sequence.Play();
                break;

            case 2:
                _sequence.Kill();
                _sequence = DOTween.Sequence();

                for (int i = 0; i < _enemys.Count; i++)
                {
                    if (_enemys[i] != null)
                    {
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x - _posDuration.x * i, _initEnemyPos.y - _posDuration.y * i, 0f);
                        //_enemys[i].localPosition = new Vector3(_initEnemyPos.x, _initEnemyPos.y, 0f);
                        _sequence.Insert(0f + _timeDuration * i, _enemys[i].DOLocalPath(new[]
                        {
                            new Vector3(2.41f,-0.5f,0f),
                            new Vector3(2.41f,5.71f,0f),


                        }, 2f, PathType.CatmullRom).SetEase(Ease.Linear));
                    }
                }
                _sequence.OnUpdate(() =>
                {
                    NextChapterSet();
                });
                _sequence.OnComplete(() =>
                {
                    _selectNumber.Value += 1;
                    _gameMaster._chapterNumber.Value += 1;
                    _sequence.Kill();
                });
                _sequence.Play();
                break;

        }*/

    }
}

