using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    public Action OnGenerationStart;
    public Action OnGenerationUpdate;
    public UnityEvent<int> OnGenerationEnd;     // Passes in the current generation number. For UI purposes only.

    [Header("References")]
    [SerializeField] private Camera _gridCamera;
    [SerializeField] private Canvas _cellWorldCanvas;
    [SerializeField] private GridCell _cellPrefab;
    [Header("Grid Settings")]
    [SerializeField, Min(3)] private int _gridRows = 3;
    [SerializeField, Min(3)] private int _gridColumns = 3;
    [SerializeField, Min(0.01f)] private float _gridUpdateIntervalInSeconds = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _randomAliveCellChance = 0f;
    [SerializeField] private bool _gridUpdatesEnabled = false;      // Generations per update. Toggled via GUI.

    private GridCell[,] _grid;
    private GameObject _gridParent;
    private float _cameraHeight = 0f;
    private float _cameraWidth = 0f;
    private Vector2 _spriteBounds;
    private Vector2 _cellOffset;
    private Vector2 _gridStartPosition;
    private int _currentGeneration = 0;

    public int CurrentGeneration => _currentGeneration;

    // Timers
    private float _gridUpdateTimer;

    private void Awake()
    {
        // Camera height and width in unity units. These will serve as the grid area.
        _cameraHeight = 2f * _gridCamera.orthographicSize;
        _cameraWidth = _cameraHeight * _gridCamera.aspect;
    }

    private void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        if (_gridUpdatesEnabled)
        {
            if (_gridUpdateTimer > 0f)
            {
                _gridUpdateTimer -= Time.deltaTime;
            }
            else
            {
                // Update grid using events.
                // Note to self: it's more painful to manage and track the previous generation state of each cell,
                // which is required to get the next state of each cell in the next generations, via manual iteration through a list.
                // Therefore, these events are here to solve that bullshit.
                _currentGeneration++;
                OnGenerationStart?.Invoke();
                OnGenerationUpdate?.Invoke();
                OnGenerationEnd?.Invoke(_currentGeneration);
                _gridUpdateTimer = _gridUpdateIntervalInSeconds;
            }
        }
    }

    public void InitializeGrid()
    {
        CalculateGridSize();
        GenerateGrid();
        AssignNeighbours();
        _currentGeneration = 0;
        OnGenerationEnd?.Invoke(_currentGeneration);
    }

    public void ToggleGridUpdates(bool isEnabled)
    {
        _gridUpdatesEnabled = isEnabled;
        if (_gridUpdatesEnabled) _gridUpdateTimer = _gridUpdateIntervalInSeconds;
    }

    public void EditRows(string newRowsValue)
    {
        if (int.TryParse(newRowsValue, out int newValue))
            _gridRows = newValue;
    }

    public void EditColumns(string newColumnsValue)
    {
        if (int.TryParse(newColumnsValue, out int newValue))
            _gridColumns = newValue;
    }

    public void EditAliveCellChance(float newChance)
    {
        _randomAliveCellChance = Mathf.Clamp01(newChance);
    }

    public void EditGridUpdateInterval(string newInterval)
    {
        if (float.TryParse(newInterval, out float newValue))
            _gridUpdateIntervalInSeconds = Mathf.Max(0.01f, newValue);
    }

    private void CalculateGridSize()
    {
        _gridStartPosition = (Vector2)_gridCamera.transform.position - new Vector2(_cameraWidth / 2f, _cameraHeight / 2f);
        _spriteBounds = new Vector2(_cellPrefab.SpriteBounds.x, _cellPrefab.SpriteBounds.y);
        _spriteBounds.x = _cameraWidth / (_gridColumns * _spriteBounds.x);
        _spriteBounds.y *= _cameraHeight / (_gridRows * _spriteBounds.y);
        _cellOffset = _spriteBounds / 2f;
    }

    private void GenerateGrid()
    {
        // Reset previous grid, if any.
        if (_gridParent) Destroy(_gridParent);
        _gridParent = new GameObject("Grid Parent");
        OnGenerationStart = null;
        OnGenerationUpdate = null;

        _grid = new GridCell[_gridRows, _gridColumns];
        for (int row = 0; row < _gridRows; row++)
        {
            for (int column = 0; column < _gridColumns; column++)
            {
                // Spawn cell, set cell's state, then store cell's current state.
                GridCell newCell = Instantiate(_cellPrefab, transform.position, Quaternion.identity);
                if (UnityEngine.Random.Range(0f, 1f) <= _randomAliveCellChance) newCell.ToggleCellState(true);
                else newCell.ToggleCellState(false);
                OnGenerationStart += newCell.UpdateCellState;
                OnGenerationUpdate += newCell.ApplyNextState;


                // Edit position, name, scale, and parent.
                newCell.transform.name = new string($"Cell_R{row}_C{column}");
                Vector2 newPosition = new Vector2(_spriteBounds.x * column, _spriteBounds.y * row) + _cellOffset + _gridStartPosition;
                newCell.transform.position = newPosition;
                newCell.transform.localScale = new Vector3(_spriteBounds.x, _spriteBounds.y, 1f);
                newCell.transform.SetParent(_gridParent.transform, true);

                // Edit cell's button's parent.
                newCell.CellToggleButton.SetParent(_cellWorldCanvas.transform, true);

                _grid[row, column] = newCell;
            }
        }
    }

    private void AssignNeighbours()
    {
        for (int row = 0; row < _gridRows; row++)
        {
            for (int column = 0; column < _gridColumns; column++)
            {
                // Check row above cell; 3 neighbouring cells.
                // Check cell's current row, 2 neighbouring cells at adjacent sides.
                // Check row below cell; 3 neighbouring cells.
                for (int localRow = -1; localRow < 2; localRow++)
                {
                    for (int localColumn = -1; localColumn < 2; localColumn++)
                    {
                        if (localRow == 0 && localColumn == 0) continue;
                        //Vector2Int neighbourIndex = new Vector2Int(row + localRow, column + localColumn);
                        int neighbourIndexRow = row + localRow;
                        int neighbourIndexColumn = column + localColumn;
                        if (ValidGridIndex(neighbourIndexRow, neighbourIndexColumn))
                        {
                            _grid[row, column].AddNeighbour(_grid[neighbourIndexRow, neighbourIndexColumn]);
                        }
                    }
                }
            }
        }
    }

    private bool ValidGridIndex(int row, int column)
    {
        if (row >= 0 && row <= _gridRows - 1)
        {
            if (column >= 0 && column <= _gridColumns - 1)
            {
                return true;
            }
        }
        return false;
    }
}
