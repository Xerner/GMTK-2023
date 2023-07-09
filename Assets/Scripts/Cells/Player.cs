using System;
using System.Collections;
using Assets.Scripts.Cells;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    static public Player Instance;

    private Cell _controlledCell;

    [SerializeField]
    private Cell _playerCell;

    public Cell ControlledCell { get => _controlledCell; }
    public bool IsAssimilating { get => _controlledCell != _playerCell; }
    bool justCameOutOfAssimilate = false;
    public Action<Cell> OnCellSwap;

    public Vector3 GetPosition() => _controlledCell.transform.position;

    #region Unity Methods

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        this.EnsureHasReference(ref _playerCell);

        if (_controlledCell == null)
        {
            SwapControl(_playerCell);
        }

        // For snappier movement, the player has a high speed with high drag
        _playerCell.BaseSpeed = 60f;
        _playerCell.GetComponent<Rigidbody2D>().drag = 10f;
    }

    void FixedUpdate()
    {
        // TODO: feed controls to _controlledCell
        // TODO: Use input actions to allow us to use controllers too
        if (IsAssimilating)
            _playerCell.transform.position = _controlledCell.transform.position;
        if (justCameOutOfAssimilate)
        {
            _controlledCell.Dash();
            justCameOutOfAssimilate = false;
        }

        var mouseScreenPosition = Mouse.current.position.ReadValue();
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        _controlledCell.FaceToward(mouseWorldPosition);

        var movementVector = new Vector2(
            Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
            Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
        ).normalized;
        _controlledCell?.Move(movementVector);
    }

    #endregion

    #region Input Callbacks

    public void AcceptMovement(CallbackContext context)
    {
        // Currently empty, instead we're accessing movement input directly inside FixedUpdate
    }

    public void AcceptAttack(CallbackContext context)
    {
        if (context.started)
        {
            shootOnDelayEnumerator = StartCoroutine(ShootOnDelay());
        }
        else if (context.canceled)
        {
            if (shootOnDelayEnumerator != null)
                StopCoroutine(shootOnDelayEnumerator);
        }
    }

    public void AcceptPoint(CallbackContext context)
    {
        // Currently empty, instead we're accessing mouse position directly inside FixedUpdate
    }

    public void AcceptDash(CallbackContext context)
    {
        if (context.started)
        {
            _controlledCell.Dash();
        }
    }

    #endregion 

    public void SwapControl(Cell newCell)
    {
        // TODO: call disable action on cell
        // For player cell this should disable
        // For non player cell destroy?
        if (_controlledCell != null)
            _controlledCell.Suspend();
        newCell.Resume();
        _controlledCell = newCell;
        _controlledCell.BaseSpeed = _playerCell.BaseSpeed;
        _controlledCell.ControllingPlayer = this;
        OnCellSwap?.Invoke(_controlledCell);
    }

    public void RelieveControl()
    {
        _playerCell.transform.rotation = (_controlledCell.transform.rotation);
        SwapControl(_playerCell);
        justCameOutOfAssimilate = true;
    }

    private Coroutine shootOnDelayEnumerator;

    IEnumerator ShootOnDelay()
    {
        while (true)
        {
            _controlledCell.Shoot();
            yield return _shootDelay;
        }
    }

    private static readonly WaitForSeconds _shootDelay = new(.25f);
}
