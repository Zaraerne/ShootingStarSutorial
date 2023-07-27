using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas HUDCanvas;
    [SerializeField] AudioData confirmGameOverSound;

    int exitStateID = Animator.StringToHash("GameOverScreenExit");

    Canvas canvas;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();

        canvas.enabled = false;
        animator.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.onGameOver += OnGameOver;

        playerInput.onConfirmGameOver += OnConfirmGameOver;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= OnGameOver;

        playerInput.onConfirmGameOver -= OnConfirmGameOver;
    }

    private void OnConfirmGameOver()
    {
        AudioManager.Instance.PlayerSFX(confirmGameOverSound);
        playerInput.DisableAllInputs();
        animator.Play(exitStateID);
        SceneLoader.Instance.LoadScoringScene();
    }

    void OnGameOver()
    {
        HUDCanvas.enabled = false;
        canvas.enabled = true;
        animator.enabled = true;
        playerInput.DisableAllInputs();
        

    }

    void EnableGameOverScreenInput()
    {
        playerInput.EnableGameOverScreenInput();
    }


}
