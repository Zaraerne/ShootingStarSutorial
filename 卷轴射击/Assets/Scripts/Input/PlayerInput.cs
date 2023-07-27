using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, 
    PlayerInputActions.IGamePlayActions, 
    PlayerInputActions.IPauseMenuActions,
    PlayerInputActions.IGameOverScreenActions
{
    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };

    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };

    public event UnityAction onDodge = delegate { };

    public event UnityAction onOverdrive = delegate { };

    public event UnityAction onPause = delegate { };

    public event UnityAction onUnpause = delegate { };

    public event UnityAction onLaunchMissile = delegate { };

    public event UnityAction onConfirmGameOver = delegate { };

    PlayerInputActions playerInputActions;

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        // 有新的动作表时，需要更新回调函数
        playerInputActions.GamePlay.SetCallbacks(this);
        playerInputActions.PauseMenu.SetCallbacks(this);
        playerInputActions.GameOverScreen.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }


    void SwitchActionMap(InputActionMap actionMap,bool isUIInput)
    {
        playerInputActions.Disable();
        actionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }


    public void SwitchToDynamicUpdateMode()=> InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    public void SwitchToFixedUpdateMode()=> InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    public void DisableAllInputs() => playerInputActions.Disable();




    public void EnableGamePlayInput() => SwitchActionMap(playerInputActions.GamePlay, false);

    public void EnablePauseMenuInput() => SwitchActionMap(playerInputActions.PauseMenu, true);

    public void EnableGameOverScreenInput() => SwitchActionMap(playerInputActions.GameOverScreen, false);




    public void OnMove(InputAction.CallbackContext context)
    {
        // 按住
        if (context.performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        // 松开
        if(context.canceled)
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onFire.Invoke();
        }

        if(context.canceled)
        {
            onStopFire.Invoke();
        }

    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }

    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }

    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onUnpause.Invoke();
        }   
    }

    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnConfirmGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
            onConfirmGameOver.Invoke();
    }
}
