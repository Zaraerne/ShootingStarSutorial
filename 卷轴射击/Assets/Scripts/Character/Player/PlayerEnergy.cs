using System.Collections;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar;
    [SerializeField] float overdriveInterval = 0.1f;
    public const int MAX = 100;
    public const int PERCENT = 1;
    int energy;
    bool avaliable = true;

    WaitForSeconds waitForOverdriveInterval;

    private void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    private void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    private void Start()
    {
        
        energyBar.Initialized(energy, MAX);
        Obtain(MAX);
    }




    public void Obtain(int value)
    {
        if(energy == MAX || !avaliable) { return; }
        //energy += value;
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateState(energy, MAX);

        

    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateState(energy, MAX);

        if (energy == 0 && !avaliable)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    public bool IsEnough(int value) => energy >= value;


    void PlayerOverdriveOn()
    {
        avaliable = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }
    void PlayerOverdriveOff()
    {
        StopCoroutine(nameof(KeepUsingCoroutine));
        avaliable = true;
    }

    IEnumerator KeepUsingCoroutine()
    {
        while(gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;
            Use(PERCENT);
        }
    }
}
