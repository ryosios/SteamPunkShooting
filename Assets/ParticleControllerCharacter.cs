using UnityEngine;

public class ParticleControllerCharacter : MonoBehaviour
{
    //キャラクター側の弾がエネミー側にダメージを与えるスクリプト

    [SerializeField] private int _particleCharacterAttackPoint = 1;//このパーティクル弾の攻撃力


    void OnParticleCollision(GameObject obj)
    {
        if (obj.tag == "Enemy")
        {
            EnemyLocator enemyLocator = obj.GetComponent<EnemyLocator>();
            enemyLocator._enemyDamagedSubject.OnNext(_particleCharacterAttackPoint);

        }

    }
}
