using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        _playerInputActions.Player.Move.started += OnMove;
        _playerInputActions.Player.Rotate.started += OnRotate;
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 direction = callbackContext.ReadValue<Vector2>();
        Debug.Log("PlayerInput: Move " + direction);
        if (direction.x < 0 && direction.y == 0)
        {
            PlayerManager.Instance.Move(DungeonData.WEST);
        }else if (direction.x == 0 && direction.y < 0)
        {
            PlayerManager.Instance.Move(DungeonData.SOUTH);
        }else if (direction.x > 0 && direction.y == 0)
        {
            PlayerManager.Instance.Move(DungeonData.EAST);
        }else if (direction.x == 0 && direction.y > 0)
        {
            PlayerManager.Instance.Move(DungeonData.NORTH);
        }
    }

    private void OnRotate(InputAction.CallbackContext callbackContext)
    {
        int direction = (int)callbackContext.ReadValue<float>();
        Debug.Log("PlayerInput: Rotate " + direction);
        PlayerManager.Instance.Rotate(direction);
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Move.started -= OnMove;
        _playerInputActions.Player.Rotate.started -= OnRotate;
    }
}
