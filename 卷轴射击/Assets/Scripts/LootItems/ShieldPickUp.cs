using UnityEngine;

public class ShieldPickUp : LootItem
{
    [SerializeField] AudioData fullHealthPickUpSFX;
    [SerializeField] int fullHeathScoreBouns = 200;
    [SerializeField] float shiledBouns = 20f;
    protected override void PickUp()
    {
        if (player.IsFullHealth)
        {
            pickUpSFX = fullHealthPickUpSFX;
            lootMessage.text = $"SCORE + {fullHeathScoreBouns}";
            ScoreManager.Instance.AddScore(fullHeathScoreBouns);
        }
        else
        {
            pickUpSFX = defaultSFX;
            lootMessage.text = $"SHIELD + {shiledBouns}";
            player.ResetHealth(shiledBouns);
        }
        base.PickUp();
    }
}
