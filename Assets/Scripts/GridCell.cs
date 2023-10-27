using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Action<GridCell> OnCellAlive;
    public Action<GridCell> OnCellDead;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private bool _isAlive = false;

    public bool IsAlive => _isAlive;
    public Vector2 SpriteBounds => _renderer.sprite.bounds.size;

    private List<GridCell> _neighbours = new List<GridCell>(8);

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

    public void AddNeighbour(GridCell neighbour)
    {
        _neighbours.Add(neighbour);
    }

    private void ToggleCellState(bool isAlive)
    {
        _isAlive = isAlive;
        UpdateColor();

        if (isAlive) OnCellAlive?.Invoke(this);
        else OnCellDead?.Invoke(this);
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
