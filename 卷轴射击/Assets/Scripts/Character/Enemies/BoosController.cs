using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosController : EnemyController
{
    [SerializeField] float continuousFireDuration = 1.5f;
    [Header("=== PLAYER DETECTION ===")]
    [SerializeField] Transform playerDetectionTransform;
    [SerializeField] Vector3 playerDetectionSize;
    [SerializeField] LayerMask playerLayer;
    [Header("=== BEAM ===")]
    [SerializeField] float beamCooldownTime = 12f;
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;
    bool isBeamReady;


    List<GameObject> magazine;
    AudioData launchSFX;
    Transform playerTransform;

    WaitForSeconds waitForCotiunousFireInterval;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitBeamCooldownTime;

    Animator animator;
    int launchBeam = Animator.StringToHash("launchBeam");

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        waitForCotiunousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);

        
    }

    protected override void OnEnable()
    {
        isBeamReady = false;
        StartCoroutine(nameof(BeamCooldownCoroutine));
        muzzleVFX.Stop();

        base.OnEnable();
    }

    protected override IEnumerator RandomlyFireCoroutine()
    {
        

        while (isActiveAndEnabled)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;

            if (isBeamReady)
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));

                yield break;
            }
            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeam);
        AudioManager.Instance.PlayerRandomSFX(beamChargingSFX);
    }

    void AE_LaunchBeam()
    {
        AudioManager.Instance.PlayerRandomSFX(beamLaunchSFX);
    }

    void StopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    void LoadProjectiles()
    {
        magazine.Clear();

        if (Physics2D.OverlapBox(playerDetectionTransform.position,playerDetectionSize,0f,playerLayer))
        {
            magazine.Add(projectiles[0]);
            launchSFX = projectileLaunchSFX[0];
        }
        else
        {
            if(Random.value < 0.5f)
            {
                magazine.Add(projectiles[1]);
                launchSFX = projectileLaunchSFX[1];
            }
            else
            {
                for(int i=2;i<projectiles.Length; i++)
                {
                    magazine.Add(projectiles[i]);
                }

                launchSFX = projectileLaunchSFX[2];
            }

        }
    }




    IEnumerator ContinuousFireCoroutine()
    {
        LoadProjectiles();
        muzzleVFX.Play();

        float continuousFireTimer = 0f;

        while (continuousFireTimer < continuousFireDuration)
        {
            foreach(var projectile in magazine)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            continuousFireTimer += minFireInterval;
            AudioManager.Instance.PlayerRandomSFX(launchSFX);

            yield return waitForCotiunousFireInterval;
        }

        muzzleVFX.Stop();
    }

    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitBeamCooldownTime;

        isBeamReady = true;



    }


    IEnumerator ChasingPlayerCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetPosition.x = ViewPort.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;

            yield return null;
        }

    }

}
