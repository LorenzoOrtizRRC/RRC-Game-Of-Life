using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _gridCamera;
    [SerializeField] private GridCell _cellPrefab;
    [Header("Grid Settings")]
    [SerializeField, Min(1)] private int _gridRows = 1;
    [SerializeField, Min(1)] private int _gridColumns = 1;

    private GridCell[,] _grid;
    private Vector2 _spriteScaling;// = 0f;
    private Vector2 _spriteBounds;
    private Vector2 _cellOffset;
    private Vector2 _gridStartPosition;

    private void Awake()
    {
        // Camera height and width in unity units.
        float cameraHeight = 2f * _gridCamera.orthographicSize;
        float cameraWidth = cameraHeight * _gridCamera.aspect;
        _gridStartPosition = (Vector2)_gridCamera.transform.position - new Vector2(cameraWidth / 2f, cameraHeight / 2f);
        //_spriteScaling = cameraHeight / Mathf.Max(_gridRows, _gridColumns);
        _spriteBounds = new Vector2(_cellPrefab.SpriteBounds.x, _cellPrefab.SpriteBounds.y);
        //_spriteScaling = new Vector2(cameraWidth / _spriteBounds.x, cameraHeight / _spriteBounds.y);
        _spriteBounds.x = cameraWidth / (_gridColumns * _spriteBounds.x);
        _spriteBounds.y *= cameraHeight / (_gridRows * _spriteBounds.y);
        _cellOffset = _spriteBounds / 2f;
    }

    private void Start()
    {
        GenerateGrid();
        AssignNeighbours();
    }

    public void GenerateGrid()
    {
        _grid = new GridCell[_gridRows, _gridColumns];
        for (int row = 0; row < _gridRows; row++)
        {
            for (int column = 0; column < _gridColumns; column++)
            {
                GridCell newCell = Instantiate(_cellPrefab);
                Vector2 newPosition = new Vector2(_spriteBounds.x * column, _spriteBounds.y * row) + _cellOffset + _gridStartPosition;
                newCell.transform.position = newPosition;
                //newCell.transform.localScale = _spriteScaling;
                newCell.transform.localScale = new Vector3(_spriteBounds.x, _spriteBounds.y, 1f);
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
                        Vector2Int neighbourIndex = new Vector2Int(row + localRow, column + localColumn);
                        print(neighbourIndex);
                        print(ValidGridIndex(row + localRow, column + localColumn));
                        if (ValidGridIndex(row + localRow, column + localColumn))
                        {
                            _grid[row, column].AddNeighbour(_grid[neighbourIndex.x, neighbourIndex.y]);
                        }
                    }
                }
            }
        }
    }

    private bool ValidGridIndex(int row, int column)
    {
        if (row > 0 && row < _gridRows - 1)
        {
            if (column > 0 && column < _gridColumns - 1)
            {
                return true;
            }
        }
        return false;
    }
}
