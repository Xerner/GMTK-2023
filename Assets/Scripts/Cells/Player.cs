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

    void Awake()
    {
        Instance = this;
        _playerCell.Speed = 4f;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.EnsureHasReference(ref _playerCell);

        if (_controlledCell == null)
        {
            SwapControl(_playerCell);
        }
    }

    void SwapControl(Cell newCell)
    {
        // TODO: call disable action on cell
        // For player cell this should disable
        // For non player cell destroy?

        _controlledCell = _playerCell;
        _controlledCell.ControllingPlayer = this;
    }

    public void AcceptMovement(CallbackContext context) => _controlledCell?.Move(context.ReadValue<Vector2>());
    public void AcceptAttack(CallbackContext _) => _controlledCell.Shoot();
    public void AcceptPoint(CallbackContext context)
    {
        // Currently empty, instead we're accessing mouse position directly
    }

    void Update()
    {
        // TODO: feed controls to _controlledCell
        var mouseScreenPosition = Mouse.current.position.ReadValue();
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        _controlledCell.FaceToward(mouseWorldPosition);
    }
}
