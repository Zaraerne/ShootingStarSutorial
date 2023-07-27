using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("--- DEATH ---")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioData[] deathSFX;
    [Header("----HEALTH----")]
    [SerializeField] protected float maxHealth;
    protected float health;

    [SerializeField] StateBar onHealthBar;
    [SerializeField] bool showOnHealrhBar = true;

    protected virtual void OnEnable()
    {
        health = maxHealth;

        if (showOnHealrhBar)
        {
            ShowOnHealthBar();
        }
        else
        {
           HideOnHealthBar();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (health == 0) return;
        health -= damage;

        if(showOnHealrhBar)
        {
            onHealthBar.UpdateState(health, maxHealth);
        }

        if(health <= 0f)
        {
            Die();
        }

    }

    public virtual void Die()
    {
        health = 0f;
        AudioManager.Instance.PlayerRandomSFX(deathSFX);
        PoolManager.Release(deathVFX,transform.position);
        gameObject.SetActive(false);

    }

    public virtual void ResetHealth(float value)
    {
        if (health == maxHealth) return;
        //health += value;
        //health = Mathf.Clamp(health, 0f, maxHealth);
        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHealrhBar)
        {
            onHealthBar.UpdateState(health, maxHealth);
        }
    }

    public void ShowOnHealthBar()
    {
        onHealthBar.gameObject.SetActive(true);
        onHealthBar.Initialized(health, maxHealth);

    }

    public void HideOnHealthBar()
    {
        onHealthBar.gameObject.SetActive(false);
    }


    protected IEnumerator HeathRegenerateCoroutine(WaitForSeconds waitForSeconds,float percent)
    {
        while(health < maxHealth)
        {
            yield return waitForSeconds;
            ResetHealth(maxHealth * percent);
        }
    }

    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitForSeconds, float percent)
    {
        while (health > 0f)
        {
            yield return waitForSeconds;
            TakeDamage(maxHealth * percent);
        }
    }



}
