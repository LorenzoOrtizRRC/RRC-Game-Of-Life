using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private bool _isAlive = false;

    public bool IsAlive => _isAlive;

    private GridCell[] _neighbours = new GridCell[0];

    // Used by player inputs.
    public void ManualToggleCellState()
    {
        ToggleCellState(!IsAlive);
    }

    // Used automatically.
    public void UpdateCellState()
    {
        /*int liveNeighbours = 0;
        foreach (GridCell cell in _neighbours)
        {
            if (cell._isAlive) liveNeighbours++;
        }*/

        int liveNeighbours = _neighbours.Count((neighbour) => neighbour._isAlive);
        if (!IsAlive && liveNeighbours == 3) ToggleCellState(true);
        else if (liveNeighbours < 2 || liveNeighbours > 3) ToggleCellState(false);
        else ToggleCellState(true);
    }

    private void ToggleCellState(bool isAlive)
    {
        _isAlive = isAlive;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_isAlive)
        {
            _renderer.color = Color.white;
        }
        else
        {
            _renderer.color = Color.black;
        }
    }
}
