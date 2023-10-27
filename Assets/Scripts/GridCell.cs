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
    [SerializeField] private Color _aliveColor;
    [SerializeField] private Color _deadColor;
    [SerializeField] private bool _isAlive = false;

    public int LiveNeighbourCount = 0;

    // Check to make sure cell is only updated once when grid updates. This is required because cells update neighbouring cells.
    public bool CellUpdated = false;
    public bool PreviouslyAlive = false;

    public bool IsAlive => _isAlive;
    public Vector2 SpriteBounds => _renderer.sprite.bounds.size;

    private bool _previouslyAlive;
    private List<GridCell> _neighbours = new List<GridCell>(8);

    // Used by player inputs.
    public void ManualToggleCellState()
    {
        ToggleCellState(!IsAlive);
    }

    // Used automatically. Only runs if: is alive and updated by the grid, or if is dead and updated by a neighbouring alive cell.
    public void UpdateCellStateOld()
    {
        //if (IsAlive && CellUpdated == true) return;

        // Update self.
        int liveNeighbours = _neighbours.Count((neighbour) => neighbour.PreviouslyAlive);
        LiveNeighbourCount = liveNeighbours;
        
        //print(liveNeighbours);
        if (!IsAlive)
        {
            //Debug.LogError("OMG ITS ALIVE ALIVE NEIGHBOUR ALIVE!");
            if (liveNeighbours == 3) ToggleCellState(true);
        }
        else
        {/*
            if (_previouslyAlive)
            {
                //print("ENABLING DEAD NEIGHBOURS");
                List<GridCell> deadNeighbours = new();// = _neighbours.FindAll((cell) => cell.IsAlive == false);
                foreach (GridCell cell in _neighbours) { if (cell.IsAlive == false) deadNeighbours.Add(cell); }
                foreach (GridCell cell in deadNeighbours)
                {
                    //Debug.LogWarning("TURNING ON DEAD NEIGHBOURS!!!");
                    cell.UpdateCellState();
                }
            }*/
            if (liveNeighbours < 2 || liveNeighbours > 3) ToggleCellState(false);
            else ToggleCellState(true);
            // Update dead neighbours.
        }
        _previouslyAlive = _isAlive;
        //if (_previouslyAlive != false && _isAlive != false) CellUpdated = true;
    }

    public void UpdateCellState()
    {
        // Update self.
        _previouslyAlive = _isAlive;
        int liveNeighbours = _neighbours.Count((neighbour) => neighbour.PreviouslyAlive);
        LiveNeighbourCount = liveNeighbours;
        if (!IsAlive)
        {
            if (liveNeighbours == 3) ToggleCellState(true);
        }
        else
        {
            if (liveNeighbours < 2 || liveNeighbours > 3) ToggleCellState(false);
            else ToggleCellState(true);
        }
    }

    public void ToggleCellState(bool isAlive)
    {
        _isAlive = isAlive;
        UpdateColor();

        //if (isAlive) OnCellAlive.Invoke(this);
        //else OnCellDead.Invoke(this);
    }

    public void AddNeighbour(GridCell neighbour)
    {
        _neighbours.Add(neighbour);
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
