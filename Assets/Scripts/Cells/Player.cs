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

    public Vector3 GetPosition() => _controlledCell.transform.position;


    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.EnsureHasReference(ref _playerCell);

        if (_controlledCell == null)
        {
            SwapControl(_playerCell);
        }

        // For snappier movement, the player has a high speed with high drag
        _playerCell.Speed = 60f;
        _playerCell.GetComponent<Rigidbody2D>().drag = 10f;
    }

    void SwapControl(Cell newCell)
    {
        // TODO: call disable action on cell
        // For player cell this should disable
        // For non player cell destroy?

        _controlledCell = _playerCell;
        _controlledCell.ControllingPlayer = this;
    }

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
            StopCoroutine(shootOnDelayEnumerator);
        }
    }

    public void AcceptPoint(CallbackContext context)
    {
        // Currently empty, instead we're accessing mouse position directly inside FixedUpdate
    }

    void FixedUpdate()
    {
        // TODO: feed controls to _controlledCell
        // TODO: Use input actions to allow us to use controllers too
        var mouseScreenPosition = Mouse.current.position.ReadValue();
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        _controlledCell.FaceToward(mouseWorldPosition);

        var movementVector = new Vector2(
            Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
            Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
        ).normalized;
        _controlledCell?.Move(movementVector);
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
