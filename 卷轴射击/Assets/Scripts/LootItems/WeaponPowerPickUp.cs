using UnityEngine;

public class WeaponPowerPickUp : LootItem
{
    [SerializeField] AudioData fullPowerPickUpSFX;
    [SerializeField] int fullPowerScoreBouns = 200;

    protected override void PickUp()
    {
        if (player.IsFullPower)
        {
            pickUpSFX = fullPowerPickUpSFX;
            lootMessage.text = $"SCORE + {fullPowerScoreBouns}";
            ScoreManager.Instance.AddScore(fullPowerScoreBouns);
        }
        else
        {
            pickUpSFX = defaultSFX;
            lootMessage.text = $"POWER UP!";
            player.PowerUp();
        }
        base.PickUp();
    }
}
