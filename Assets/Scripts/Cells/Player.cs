using Assets.Scripts.Cells;
using Assets.Scripts.Extensions;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    private Cell _controlledCell;

    [SerializeField] 
    private Cell _playerCell;

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
    }

    public void AcceptMovement(CallbackContext context) => _controlledCell?.Move(context.ReadValue<Vector2>());
    public void AcceptAttack(CallbackContext _) => _controlledCell.Shoot();
    public void AcceptPoint(CallbackContext context)
    {
        Vector2 mouseScreenPosition = context.ReadValue<Vector2>();
        Vector2 playerScreenPosition = Camera.main.WorldToViewportPoint(transform.position);

        Vector2 facingDelta = mouseScreenPosition - playerScreenPosition;
        facingDelta.Normalize();

        _controlledCell.FaceToward(facingDelta);
    }

    void Update()
    {
        // TODO: feed controls to _controlledCell
    }
}
