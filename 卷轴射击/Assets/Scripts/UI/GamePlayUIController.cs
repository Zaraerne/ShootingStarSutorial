using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    [Header("=== PLAYER INPUT ===")]
    [SerializeField] PlayerInput inputActions;
    [Header("=== AUDIO DATA ===")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;
    [Header("=== CANVAS ===")]
    [SerializeField] Canvas huDCanvas;
    [SerializeField] Canvas menusCanvas;
    [Header("=== PLAYER INPUT ===")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable()
    {
        inputActions.onPause += Pause;
        inputActions.onUnpause += OnUnpause;

        ButtonPressedBehaviour.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
    }

    void OnDisable()
    {
        inputActions.onPause -= Pause;
        inputActions.onUnpause -= OnUnpause;

        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }


    void Pause()
    {
        huDCanvas.enabled = false;
        menusCanvas.enabled = true;
        TimeController.Instance.Pause();
        GameManager.GameState = GameState.Paused;
        inputActions.EnablePauseMenuInput();
        inputActions.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);

        AudioManager.Instance.PlayerSFX(pauseSFX);
    }

    void OnUnpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);

        AudioManager.Instance.PlayerSFX(unpauseSFX);
    }

    void OnResumeButtonClick()
    {
        huDCanvas.enabled = true;
        menusCanvas.enabled = false;
        TimeController.Instance.UnPause();
        GameManager.GameState = GameState.Playing;
        inputActions.EnableGamePlayInput();
        inputActions.SwitchToFixedUpdateMode();
    }

    void OnOptionsButtonClick()
    {
        UIInput.Instance.SelectUI(optionsButton);
        inputActions.EnablePauseMenuInput();
    }

    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }

}



