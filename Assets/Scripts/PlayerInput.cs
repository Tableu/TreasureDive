using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Restart.Disable();
    }

    private void Start()
    {
        _playerInputActions.Player.Move.started += OnMove;
        _playerInputActions.Player.Rotate.started += OnRotate;
        _playerInputActions.Player.Restart.started += OnRestart;
        StartCoroutine(LateStart());
        PlayerManager.Instance.OnDeath += OnDeath;
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        DungeonManager dungeonManager = DungeonManager.Instance;
        dungeonManager.Initialize();
        StartCoroutine(PlayerManager.Instance.OxygenTimer());
    }

    private void OnRestart(InputAction.CallbackContext callbackContext)
    {
        _playerInputActions.Player.Move.Enable();
        _playerInputActions.Player.Rotate.Enable();
        _playerInputActions.Player.Restart.Disable();
        DungeonManager.Instance.Restart();
        StartCoroutine(PlayerManager.Instance.OxygenTimer());
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
        AkSoundEngine.PostEvent("player_bubbles_event", gameObject);
    }

    private void OnRotate(InputAction.CallbackContext callbackContext)
    {
        int direction = (int)callbackContext.ReadValue<float>();
        Debug.Log("PlayerInput: Rotate " + direction);
        PlayerManager.Instance.Rotate(direction);
    }

    private void OnDeath()
    {
        _playerInputActions.Player.Move.Disable();
        _playerInputActions.Player.Rotate.Disable();
        _playerInputActions.Player.Restart.Enable();
    }
    private void OnDestroy()
    {
        _playerInputActions.Player.Move.started -= OnMove;
        _playerInputActions.Player.Rotate.started -= OnRotate;
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnDeath -= OnDeath;
        }
    }
}
