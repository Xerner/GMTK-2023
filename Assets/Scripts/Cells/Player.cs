using Assets.Scripts.Cells;
using Assets.Scripts.Extensions;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell _controlledCell;

    [SerializeField] 
    private Cell _playerCell;

    // Start is called before the first frame update
    void Start()
    {
        this.EnsureHasReference(ref _playerCell);

        if (_controlledCell != null)
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

    void Update()
    {
        // TODO: feed controls to _controlledCell
    }
}
