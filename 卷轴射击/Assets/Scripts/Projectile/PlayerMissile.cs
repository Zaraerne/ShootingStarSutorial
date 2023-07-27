using System.Collections;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquiredVoice = null;

    [Header("=== SPeED CHANHE ===")]
    [SerializeField] float lowSpeed = 8;
    [SerializeField] float highSpeed = 25;
    [SerializeField] float variableSpeedDelay = 0.5f;

    [Header("=== SPeED CHANHE ===")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] float explosionRadius = 3;
    [SerializeField] float explisionDamage = 100f;


    WaitForSeconds waitVariableSpeedDelay;


    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PoolManager.Release(explosionVFX,transform.position);

        AudioManager.Instance.PlayerRandomSFX(explosionSFX);

        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);
        foreach(var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explisionDamage);
            }
        }

    }

    IEnumerator VariableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = highSpeed;

        if(target != null)
        {
            AudioManager.Instance.PlayerRandomSFX(targetAcquiredVoice);
        }


    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
