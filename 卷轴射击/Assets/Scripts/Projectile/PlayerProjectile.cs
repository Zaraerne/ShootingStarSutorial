using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trailRenderer;
    protected virtual void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        if (moveDirction != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirction);
        }
    }

    private void OnDisable()
    {
        trailRenderer.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);
    }


}
