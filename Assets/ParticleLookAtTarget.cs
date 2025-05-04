using UnityEngine;

public class particleLookAtTarget : EnemyLocator
{
    //EnemyLocatorを継承
   
    private Transform _characterLocatorTrans;

    private void Start()
    {
        _characterLocatorTrans = _characterLocator.transform;//EnemyLocator経由でCharacterLocator取得
    }
    void Update()
    {
        if (_characterLocatorTrans == null) return;

        Vector2 direction = _characterLocatorTrans.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}