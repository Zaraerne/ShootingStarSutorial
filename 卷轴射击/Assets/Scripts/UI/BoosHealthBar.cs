public class BoosHealthBar : StatesBar_HUD
{
    protected override void SetPercentText()
    {
        percentText.text = targetFillAmount.ToString("P");
    }
}
