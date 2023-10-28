using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private RectTransform _cellToggleButton;
    [SerializeField] private bool _isAlive = false;

    public RectTransform CellToggleButton => _cellToggleButton;
    public bool IsAlive => _isAlive;
    public Vector2 SpriteBounds => _renderer.sprite.bounds.size;

    private List<GridCell> _neighbours = new List<GridCell>();

    private bool _nextState = false;        // True = Alive, False = Dead.

    public void UpdateCellState()
    {
        // Update self.
        int liveNeighbours = _neighbours.Count((neighbour) => neighbour.IsAlive);
        if (_isAlive == false)
        {
            if (liveNeighbours == 3)
            {
                _nextState = true;      // Reproduction.
            }
        }
        else
        {
            if (liveNeighbours < 2 || liveNeighbours > 3) _nextState = false;      // Overpopulation / underpopulation.
            else _nextState = true;        // Stay alive.
        }
    }

    // Called upon by player inputs.
    public void ManualToggleState()
    {
        ToggleCellState(!IsAlive);
    }

    public void ToggleCellState(bool newAliveState)
    {
        _isAlive = newAliveState;
        UpdateColor();
    }

    public void AddNeighbour(GridCell neighbour)
    {
        _neighbours.Add(neighbour);
    }

    public void ApplyNextState()
    {
        ToggleCellState(_nextState);
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
