using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boos : Enemy
{
    BoosHealthBar healthBar;
    Canvas healthBarCanvas;

    protected override void Awake()
    {
        base.Awake();
        healthBar = FindObjectOfType<BoosHealthBar>();
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.Initialized(health, maxHealth);
        healthBarCanvas.enabled = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }

    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateState(health, maxHealth);
    }

    protected override void SetHealth()
    {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber * healthFactor);
    }
}
