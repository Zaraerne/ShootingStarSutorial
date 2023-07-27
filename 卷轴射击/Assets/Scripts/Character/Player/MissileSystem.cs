using System.Collections;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float colldownTime = 1f;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData launchSFX = null;


    int amount;
    bool isReady = true;

    private void Awake()
    {
        amount = defaultAmount;
    }

    private void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void PickUp()
    {
        amount++;
        MissileDisplay.UpdateAmountText(amount);

        if(amount == 1)
        {
            MissileDisplay.UpdateCoolDownImage(0f);
            isReady = true;
        }
    }

    public void Launch(Transform muzzleTransform)
    {

        if (amount == 0 || !isReady) return;

        isReady = false;

        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayerRandomSFX(launchSFX);

        amount--;
        MissileDisplay.UpdateAmountText(amount);
        if(amount == 0)
        {
            MissileDisplay.UpdateCoolDownImage(1f);
        }
        else
        {
            StartCoroutine(nameof(CooldownCoroutine));
        }
    }

    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(colldownTime);

        var cooldownValue = colldownTime;
        while(cooldownValue > 0f)
        {
            MissileDisplay.UpdateCoolDownImage(cooldownValue / colldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }

}
