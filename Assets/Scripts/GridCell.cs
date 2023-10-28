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

    //public int LiveNeighbourCount = 0;

    // Check to make sure cell is only updated once when grid updates. This is required because cells update neighbouring cells.
    public bool CellUpdated = false;
    public bool IsAlive => _isAlive;
    public bool PreviouslyAlive { get; private set; } = false;
    public Vector2 SpriteBounds => _renderer.sprite.bounds.size;

    //private bool _previouslyAlive;
    private List<GridCell> _neighbours = new List<GridCell>();

    private bool _nextState = false;        // true = alive, false = dead

    // Used by player inputs.
    /*public void ManualToggleCellState()
    {
        ToggleCellState(!IsAlive);
    }*/

    // Used automatically. Only runs if: is alive and updated by the grid, or if is dead and updated by a neighbouring alive cell.
    /*public void UpdateCellStateOld()
    {
        //if (IsAlive && CellUpdated == true) return;

        // Update self.
        int liveNeighbours = _neighbours.Count((neighbour) => neighbour.PreviouslyAlive);
        //LiveNeighbourCount = liveNeighbours;
        
        //print(liveNeighbours);
        if (!IsAlive)
        {
            //Debug.LogError("OMG ITS ALIVE ALIVE NEIGHBOUR ALIVE!");
            if (liveNeighbours == 3) ToggleCellState(true);
        }
        else
        {
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
            }
            if (liveNeighbours < 2 || liveNeighbours > 3) ToggleCellState(true);
            else ToggleCellState(false);
            // Update dead neighbours.
        }
        //_previouslyAlive = _isAlive;
        //if (_previouslyAlive != false && _isAlive != false) CellUpdated = true;
    }*/

    /*public void UpdateCellState()
    {
        // Update self.
        //PreviouslyAlive = _isAlive;
        //int liveNeighbours = _neighbours.Count((neighbour) => neighbour.PreviouslyAlive);
        int liveNeighbours = 0;
        foreach (GridCell cell in _neighbours) if (cell.PreviouslyAlive == true) liveNeighbours++;
        //LiveNeighbourCount = liveNeighbours;
        if (!_isAlive == false && liveNeighbours == 3)
        {
            ToggleCellState(true);      // Reproduction.
        }
        else
        {
            if (liveNeighbours >= 2 && liveNeighbours <= 3) ToggleCellState(true);      // Keep alive.
            else ToggleCellState(false);        // Overpopulation / underpopulation.
        }
    }*/

    public void UpdateCellState()
    {
        // Update self.
        //PreviouslyAlive = _isAlive;
        //int liveNeighbours = _neighbours.Count((neighbour) => neighbour.PreviouslyAlive);
        //int liveNeighbours = 0;
        //foreach (GridCell cell in _neighbours) if (cell.IsAlive == true) liveNeighbours++;
        int liveNeighbours = _neighbours.Count((neighbour) => neighbour.IsAlive);
        //LiveNeighbourCount = liveNeighbours;
        if (_isAlive == false)
        {
            if (liveNeighbours == 3)
            {
                Debug.LogError("AAHHHH");
                GridCell[] liveOnes = _neighbours.FindAll((x) => x.IsAlive).ToArray();
                print($"DEAD CELL NOW ALIVE! name: {transform.name}, neighbours alive: {liveNeighbours}");
                foreach (var thing in liveOnes) print("name: " + transform.name + " thing name: " + thing.transform.name + " ");
                _nextState = true;      // Reproduction.
            }

        }
        else
        {
            Debug.LogError("EHEHEHEHE");
            if (liveNeighbours >= 2 && liveNeighbours <= 3) _nextState = true;      // Keep alive.
            else _nextState = false;        // Overpopulation / underpopulation.
        }
    }

    public void ToggleCellState(bool newAliveState)
    {
        _isAlive = newAliveState;
        //_nextState = _isAlive;
        UpdateColor();

        //if (isAlive) OnCellAlive.Invoke(this);
        //else OnCellDead.Invoke(this);
    }

    public void AddNeighbour(GridCell neighbour)
    {
        _neighbours.Add(neighbour);
    }

    public void SaveCurrentState()
    {
        PreviouslyAlive = _isAlive;
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
