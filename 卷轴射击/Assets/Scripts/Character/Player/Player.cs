using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] StatesBar_HUD statesBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0,1f)] float healthRegeneratePercent;

    [Header("----INPUT----")]
    [SerializeField] PlayerInput playerInput;
    [Header("----MOVE----")]
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;

    [Header("----FIRE----")]
    [SerializeField] AudioData projectileLaunchSFX;
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdrive;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] Transform muzzleMid;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] float fireInterval = 0.2f;

    [Header("---DODGE---")]
    [SerializeField] AudioData dodgeSFX;
    [SerializeField,Range(0,100)] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("---OVERDRIVE---")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;

    [Header("=== Missile ===")]
    MissileSystem missileSystem;


    float paddingX;
    float paddingY;

    bool isDodging = false;
    bool isOverridriving = false;


    readonly float SlowMotionDuration = 1f;
    float InvincibleTime = 1f;

    float currentRoll;
    float dodgeDuration;
    

    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitHealthRegenerateTime;

    WaitForSeconds waitForOverdriveFireInterval;
    WaitForSeconds waitDecelerationTime;

    WaitForSeconds waitInvicibleTime;



    new Rigidbody2D rigidbody;
    new Collider2D collider;
    Coroutine moveCoroutine;
    Coroutine healthRengenerateCoroutine;

    float t;
    Vector2 moveDirection;
    Vector2 previousVelocity;
    Quaternion previousRotation;
    WaitForFixedUpdate waitForFixedUpdate;

    [SerializeField] float moveSpeed = 10f;

    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missileSystem = GetComponent<MissileSystem>();
        dodgeDuration = maxRoll / rollSpeed;

        var size = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
        paddingX = size.x / 2;
        paddingY = size.y / 2;


        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitForFixedUpdate = new WaitForFixedUpdate();
        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitInvicibleTime = new WaitForSeconds(InvincibleTime);


    }

    /// <summary>
    /// 无敌携程
    /// </summary>
    /// <returns></returns>
    IEnumerator IvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitInvicibleTime;

        collider.isTrigger = false;
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        playerInput.onMove += Move;
        playerInput.onStopMove += StopMove;

        playerInput.onFire += Fire;
        playerInput.onStopFire += StopFire;
        playerInput.onDodge += Dodge;

        playerInput.onOverdrive += Overdrive;

        playerInput.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;

    }

    

    private void OnDisable()
    {
        playerInput.onMove -= Move;
        playerInput.onStopMove -= StopMove;

        playerInput.onFire -= Fire;
        playerInput.onStopFire -= StopFire;

        playerInput.onDodge -= Dodge;

        playerInput.onOverdrive -= Overdrive;

        playerInput.onLaunchMissile -= LaunchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }


    private void Start()
    {
        playerInput.EnableGamePlayInput();
        statesBar_HUD.Initialized(health, maxHealth);
    }

    #region MOVE
    private void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, moveDirection, Quaternion.identity));
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    private void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationLimitCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;
        while (t < time)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t / time);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t / time);

            yield return waitForFixedUpdate;
        }
    }

    IEnumerator MoveRangeLimatationLimitCoroutine()
    {
        while (true)
        {
            transform.position = ViewPort.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;
        StopCoroutine(nameof(MoveRangeLimatationLimitCoroutine));
    }
    #endregion

    #region Fire

    private void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    private void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    

    IEnumerator FireCoroutine()
    {
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile1, muzzleMid.position, Quaternion.identity);
                    break;
                case 1:
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile2, muzzleTop.position, Quaternion.identity);
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile3, muzzleBottom.position, Quaternion.identity);
                    break;
                case 2:
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile2, muzzleTop.position, Quaternion.identity);
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile1, muzzleMid.position, Quaternion.identity);
                    PoolManager.Release(isOverridriving ? projectileOverdrive : projectile3, muzzleBottom.position, Quaternion.identity);
                    break;
            }
            AudioManager.Instance.PlayerSFX(projectileLaunchSFX);

            yield return isOverridriving ? waitForOverdriveFireInterval : waitForFireInterval;
        }
    }


    #endregion

    #region Health
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        statesBar_HUD.UpdateState(health, maxHealth);
        TimeController.Instance.BulletTime(SlowMotionDuration);
        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(nameof(IvincibleCoroutine));
            if (regenerateHealth)
            {
                if (healthRengenerateCoroutine != null)
                {
                    StopCoroutine(healthRengenerateCoroutine);
                }
                healthRengenerateCoroutine = StartCoroutine(HeathRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void ResetHealth(float value)
    {
        base.ResetHealth(value);
        statesBar_HUD.UpdateState(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        statesBar_HUD.UpdateState(0f, maxHealth);
        GameManager.GameState = GameState.GameOver;
        base.Die();

    }

    #endregion

    #region DODGE
    private void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));

        // 改变玩家缩放值

    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayerRandomSFX(dodgeSFX);
        // 能量值的消耗
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        // 让玩家无敌
        collider.isTrigger = true;
        // 沿着X轴旋转
        currentRoll = 0f;

        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);

        var scale = transform.localScale;

        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);
            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion

    #region OVERDRIVE
    private void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();


    }

    private void OverdriveOn()
    {
        isOverridriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(SlowMotionDuration,SlowMotionDuration);
    }
    private void OverdriveOff()
    {
        isOverridriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }


    #endregion

    #region MISSILE
    void LaunchMissile()
    {
        missileSystem.Launch(muzzleMid);
    }
    public void PickUpMissile()
    {
        missileSystem.PickUp();
    }
    #endregion

    #region WEAPON POWER
    public void PowerUp()
    {
        //weaponPower += 1;
        //weaponPower = Mathf.Clamp(weaponPower, 0, 2);
        weaponPower = Mathf.Min(weaponPower + 1, 2);
    }

    void PowerDown()
    {
        weaponPower = Mathf.Max(--weaponPower, 0);
    }
    #endregion

}
