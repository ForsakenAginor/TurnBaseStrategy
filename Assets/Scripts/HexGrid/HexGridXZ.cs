using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    public class HexGridXZ<TGridObject>
    {
        private const float VerticalOffsetMultiplier = 0.75f;

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly TGridObject[,] _gridArray;
        private Vector3 _originPosition;
        private Dictionary<Vector2Int, IEnumerable<Vector2Int>> _cashedNeghbours;

        public HexGridXZ(int width, int height, float cellSize, Vector3 originPosition)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;
            _gridArray = new TGridObject[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                    _gridArray[x, z] = default;

            MakeCasheNeighbours();
        }

        public event Action<Vector2Int> GridObjectChanged;

        public int Width => _width;

        public int Height => _height;

        public float CellSize => _cellSize;

        public Vector3 OriginPosition => _originPosition;

        public IReadOnlyDictionary<Vector2Int, IEnumerable<Vector2Int>> CashedNeighbours => _cashedNeghbours;

        public static IEnumerable<Vector2Int> GetNeighbours(Vector2Int cell)
        {
            bool oddRow = cell.y % 2 == 1;
            List<Vector2Int> neighbours = new ()
        {
            cell + new Vector2Int(-1, 0),
            cell + new Vector2Int(+1, 0),
            cell + new Vector2Int(oddRow ? +1 : -1, +1),
            cell + new Vector2Int(+0, +1),
            cell + new Vector2Int(oddRow ? +1 : -1, -1),
            cell + new Vector2Int(+0, -1),
        };
            return neighbours;
        }

        public Vector3 GetCellWorldPosition(int x, int z)
        {
            float cellSizeFactor = _cellSize * 0.5f;
            bool isOddRow = Mathf.Abs(z) % 2 == 1;

            return
                (new Vector3(x, 0, 0) * _cellSize) +
                (_cellSize * VerticalOffsetMultiplier * new Vector3(0, 0, z)) +
                (isOddRow ? new Vector3(1, 0, 0) * cellSizeFactor : Vector3.zero) +
                _originPosition;
        }

        public Vector3 GetCellWorldPosition(Vector2Int cellCoordinates)
        {
            return GetCellWorldPosition(cellCoordinates.x, cellCoordinates.y);
        }

        public void SetGridObject(int x, int z, TGridObject value)
        {
            if (IsValidGridPosition(x, z))
            {
                _gridArray[x, z] = value;
                GridObjectChanged?.Invoke(new Vector2Int(x, z));
            }
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            GetXZ(worldPosition, out int x, out int z);
            SetGridObject(x, z, value);
        }

        public TGridObject GetGridObject(int x, int z)
        {
            if (IsValidGridPosition(x, z))
                return _gridArray[x, z];
            else
                return default;
        }

        public TGridObject GetGridObject(Vector2Int coordinates)
        {
            return GetGridObject(coordinates.x, coordinates.y);
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            GetXZ(worldPosition, out int x, out int z);
            return GetGridObject(x, z);
        }

        public bool IsValidGridPosition(Vector2Int cell)
        {
            return IsValidGridPosition(cell.x, cell.y);
        }

        public bool IsValidGridPosition(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _height)
                return true;
            else
                return false;
        }

        public Vector2Int GetXZ(Vector3 worldPosition)
        {
            GetXZ(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            int roughX = Mathf.RoundToInt((worldPosition - _originPosition).x / _cellSize);
            int roughZ = Mathf.RoundToInt((worldPosition - _originPosition).z / _cellSize / VerticalOffsetMultiplier);

            Vector2Int roughXZ = new (roughX, roughZ);
            Vector2Int closestXZ = roughXZ;
            IEnumerable<Vector2Int> neighbours;

            if (_cashedNeghbours.ContainsKey(roughXZ))
                neighbours = _cashedNeghbours[roughXZ];
            else
                neighbours = GetNeighbours(roughXZ);

            foreach (Vector2Int neighbour in neighbours)
            {
                if (Vector3.Distance(worldPosition, GetCellWorldPosition(neighbour.x, neighbour.y)) <
                    Vector3.Distance(worldPosition, GetCellWorldPosition(closestXZ.x, closestXZ.y)))
                {
                    closestXZ = neighbour;
                }
            }

            x = closestXZ.x;
            z = closestXZ.y;
        }

        private void MakeCasheNeighbours()
        {
            _cashedNeghbours = new Dictionary<Vector2Int, IEnumerable<Vector2Int>>();

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                {
                    Vector2Int cell = new (x, z);
                    IEnumerable<Vector2Int> neighbours = GetNeighbours(cell);
                    _cashedNeghbours.Add(cell, neighbours);
                }
            }
        }
    }
}