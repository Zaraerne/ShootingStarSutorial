using UnityEngine;
using UnityEngine.UI;

public class MissileDisplay : MonoBehaviour
{
    static Text amountText;
    static Image coolDownImage;

    private void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        coolDownImage = transform.Find("Cooldown Image").GetComponent<Image>();
    }


    public static void UpdateAmountText(int amount) => amountText.text = amount.ToString();

    public static void UpdateCoolDownImage(float fillAmount) => coolDownImage.fillAmount = fillAmount;


}
