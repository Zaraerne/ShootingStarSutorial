using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StateBar : MonoBehaviour
{
    float currentFillAmount;
    protected float targetFillAmount;

    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;
    [SerializeField] float fillSpeed = 0.1f;


    float previousFillAmount;

    Canvas canvas;
    float t;
    WaitForSeconds waitForDelayFill;
    Coroutine bufferedFillingCoroutine;

    private void Awake()
    {
        if(TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialized(float currentValue,float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateState(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if(bufferedFillingCoroutine != null) { StopCoroutine(bufferedFillingCoroutine); }

        if(currentFillAmount > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        } 
        else if(currentFillAmount < targetFillAmount)
        {
            fillImageBack.fillAmount = currentFillAmount; ;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }

    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        t = 0;
        if (delayFill)
        {
            yield return waitForDelayFill;
        }
        previousFillAmount = currentFillAmount;
        while (t < 1f)
        {
            t = Time.deltaTime + fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }

    }

}
