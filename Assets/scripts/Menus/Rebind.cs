using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebind : MonoBehaviour
{
    [SerializeField] private PlayerInputActions playerInputActions;

    public void OnRebindMove(InputAction.CallbackContext context)
    {
        playerInputActions.Player.Disable();
        playerInputActions.Player.Move.PerformInteractiveRebinding()
            .OnComplete(callback =>
            {
                Debug.Log(callback);
                callback.Dispose();
            })
            .Start();
    }
}
