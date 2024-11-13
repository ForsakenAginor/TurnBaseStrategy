using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HexGrid;
using UnityEngine;

namespace HexPathfinding
{
    public class HexPathFinder
    {
        private readonly HexGridXZ<PathNodeHex> _grid;
        private readonly Dictionary<PathNodeHex, Vector2Int> _nodesCoordinates;
        private List<PathNodeHex> _openedList;
        private List<PathNodeHex> _closedList;

        public HexPathFinder(int width, int height, float cellSize)
        {
            _grid = new HexGridXZ<PathNodeHex>(width, height, cellSize, Vector3.zero);
            _nodesCoordinates = new Dictionary<PathNodeHex, Vector2Int>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector2Int coordinates = new (x, z);
                    PathNodeHex node = new ();
                    _grid.SetGridObject(x, z, node);
                    _nodesCoordinates.Add(node, coordinates);
                }
            }
        }

        public void MakeNodUnwalkable(Vector2Int nodCoordinates)
        {
            _grid.GetGridObject(nodCoordinates).SetIsWalkable(false);
        }

        public void MakeNodWalkable(Vector2Int nodCoordinates)
        {
            _grid.GetGridObject(nodCoordinates).SetIsWalkable(true);
        }

        public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWordlPosition)
        {
            Vector2Int startPosition = _grid.GetXZ(startWorldPosition);
            Vector2Int endPosition = _grid.GetXZ(endWordlPosition);

            List<PathNodeHex> path = MakePath(startPosition, endPosition);
            List<Vector3> vectorPath = new ();

            if (path == null)
                return null;

            foreach (PathNodeHex pathNode in path)
                vectorPath.Add(_grid.GetCellWorldPosition(_nodesCoordinates[pathNode]));

            return vectorPath;
        }

        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition)
        {
            List<PathNodeHex> path = MakePath(startPosition, endPosition);
            List<Vector2Int> vectorPath = new ();

            if (path == null)
                return null;

            foreach (PathNodeHex pathNode in path)
                vectorPath.Add(_nodesCoordinates[pathNode]);

            return vectorPath;
        }

        private List<PathNodeHex> MakePath(Vector2Int startWorldPosition, Vector2Int endWorldPosition)
        {
            PathNodeHex startNode = _grid.GetGridObject(startWorldPosition.x, startWorldPosition.y);
            PathNodeHex endNode = _grid.GetGridObject(endWorldPosition.x, endWorldPosition.y);

            if (startNode == null || endNode == null)
                return null;

            _openedList = new List<PathNodeHex> { startNode };
            _closedList = new List<PathNodeHex>();

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    PathNodeHex pathNode = _grid.GetGridObject(x, y);
                    pathNode.SetGCost(99999999);
                    pathNode.SetCameFromNode(null);
                }
            }

            startNode.SetGCost(0);
            startNode.SetHCost(CalculateDistanceCost(startWorldPosition, endWorldPosition));

            while (_openedList.Count > 0)
            {
                PathNodeHex currentNode = GetLowestFCostNode(_openedList);

                if (currentNode == endNode)
                    return CalculatePath(endNode);

                _openedList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (PathNodeHex neighbourNode in GetNeighbours(_nodesCoordinates[currentNode]))
                {
                    if (_closedList.Contains(neighbourNode))
                        continue;

                    if (neighbourNode.IsWalkable == false)
                    {
                        _closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(_nodesCoordinates[currentNode], _nodesCoordinates[neighbourNode]);

                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.SetCameFromNode(currentNode);
                        neighbourNode.SetGCost(tentativeGCost);
                        neighbourNode.SetHCost(CalculateDistanceCost(_nodesCoordinates[neighbourNode], _nodesCoordinates[endNode]));

                        if (_openedList.Contains(neighbourNode) == false)
                            _openedList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }

        private List<PathNodeHex> GetNeighbours(Vector2Int cell)
        {
            return HexGridXZ<PathNodeHex>.GetNeighbours(cell).Where(o => _grid.IsValidGridPosition(o.x, o.y)).Select(o => _grid.GetGridObject(o.x, o.y)).ToList();
        }

        private List<PathNodeHex> CalculatePath(PathNodeHex endNode)
        {
            List<PathNodeHex> path = new () { endNode };
            PathNodeHex currentNode = endNode;

            while (currentNode.CameFromNode != null)
            {
                path.Add(currentNode.CameFromNode);
                currentNode = currentNode.CameFromNode;
            }

            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(Vector2Int a, Vector2Int b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return Mathf.Min(xDistance, yDistance) + remaining;
        }

        private PathNodeHex GetLowestFCostNode(List<PathNodeHex> pathNodeList)
        {
            PathNodeHex lowestFCostNode = pathNodeList[0];

            for (int i = 1; i < pathNodeList.Count; i++)
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                    lowestFCostNode = pathNodeList[i];

            return lowestFCostNode;
        }
    }
}