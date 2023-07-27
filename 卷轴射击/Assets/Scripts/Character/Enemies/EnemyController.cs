using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("----MOVE----")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float moveRotationAngle = 25f;

    [Header("----FIRE----")]
    [SerializeField] protected GameObject[] projectiles;
    [SerializeField] protected AudioData[] projectileLaunchSFX;
    [SerializeField] protected ParticleSystem muzzleVFX;

    [SerializeField] protected Transform muzzle;
    [SerializeField] protected float minFireInterval;
    [SerializeField] protected float maxFireInterval;

    protected float paddingX;
    protected float paddingY;
    protected Vector3 targetPosition;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake()
    {
        var size = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
        paddingX = size.x / 2;
        paddingY = size.y / 2;
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(RandomlyMovingCoroutine());
        StartCoroutine(RandomlyFireCoroutine());
    }

    protected virtual void OnDisable()
    {
        StopCoroutine(RandomlyMovingCoroutine());
        StopCoroutine(RandomlyFireCoroutine());
    }


    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = ViewPort.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);
        while (gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, targetPosition) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return waitForFixedUpdate;
        }
    }

    protected virtual IEnumerator RandomlyFireCoroutine()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval,maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            foreach(GameObject projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayerRandomSFX(projectileLaunchSFX);
            muzzleVFX.Play();
        }

    }

}
