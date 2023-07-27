using System.Collections;
using UnityEngine;

public class EnemyProjectileAiming : Projectile
{
    private void Awake()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    protected override void OnEnable()
    {
        StartCoroutine(MoveDirectionCoroutine());
        base.OnEnable();
    }


    IEnumerator MoveDirectionCoroutine()
    {
        yield return null;
        if (target.activeSelf)
        {
            moveDirction = (target.transform.position - transform.position).normalized;
        }
        
    }


}
