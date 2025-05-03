using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyLocator : MonoBehaviour
{
    public CharacterLocator _characterLocator { get; set; }

    [SerializeField] private Transform _enemyBodyRect;
    [SerializeField] private ParticleSystem _enemyAttackParticle;
    [SerializeField] private ParticleSystem _effectBombParticle;
    [SerializeField] private ParticleSystem _effectShockParticle;


    private CircleCollider2D _thisCircleCollider;

    //HP
    [SerializeField] private int _Hp = 2;
    private ReactiveProperty<int> _enemyHp;

    //ポイント
    [SerializeField] private int _point = 100;

    //生存フラグ
    public bool _isAlive { get; set; } = true;

    //エネミーがダメージを受けるサブジェクト
    public Subject<int> _enemyDamagedSubject = new Subject<int>();


    // Start is called before the first frame update
    void Awake()
    {
        _thisCircleCollider = GetComponent<CircleCollider2D>();
        _characterLocator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterLocator>();

        _enemyHp = new ReactiveProperty<int>(_Hp);
        _enemyDamagedSubject. //被ダメ時のイベント。HP0になるまで毎回呼ばれる
            Subscribe(damage =>
            {
                _enemyHp.Value -= damage;
                _effectShockParticle.Play();

            }).AddTo(this);

        _enemyHp //HPが減った時のイベント
            .DistinctUntilChanged()
             .Subscribe(hp =>
             {
                 if(hp == 0)//敵がしんだときのイベント
                 {
                     _isAlive = false;
                     _thisCircleCollider.enabled = false;
                     _enemyBodyRect.gameObject.SetActive(false);
                     UIPoint._instance.AddPoint(_point);
                     _enemyAttackParticle.Stop();
                     _effectBombParticle.Play();
                 }

             }).AddTo(this);


    }

   
}
