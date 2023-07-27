using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile
{
    [SerializeField] ProjectileGuiddanceSystem projectileGuiddanceSystem;
    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        if (target == null) base.OnEnable();
        else
        {
            StartCoroutine(projectileGuiddanceSystem.HomingCoroutine(target));
        }

    }


}
