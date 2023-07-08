using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public static class PlayerInputExtensions
{
    public static InputAction GetAction(this PlayerInput playerInput, string actionMap, string action)
    {
        return playerInput.actions.FindActionMap(actionMap).FindAction(action);
    }

    public static UnityEvent<InputAction.CallbackContext> GetEvent(this PlayerInput playerInput, string actionMap, string action, GameObject gameObject = null) {
        InputAction inputAction = playerInput.GetAction(actionMap, action);
        if (inputAction == null)
            Debug.LogError($"PlayerInput does not contain the action map {actionMap}", gameObject);

        return playerInput.GetEvent(inputAction, gameObject);
    }

    public static UnityEvent<InputAction.CallbackContext> GetEvent(this PlayerInput playerInput, InputActionReference inputActionReference, GameObject gameObject = null)
    {
        InputAction action = inputActionReference.action;
        return playerInput.GetEvent(action, gameObject);
    }

    public static UnityEvent<InputAction.CallbackContext> GetEvent(this PlayerInput playerInput, InputAction action, GameObject gameObject = null)
    {
        if (action == null)
            return null;
        var event_ = playerInput.actionEvents.FirstOrDefault((actionEvent) => actionEvent.actionId == action.id.ToString());
        if (event_ == null)
            Debug.LogError($"PlayerInput does not contain the action {action.name}", gameObject);

        return event_;
    }
}
