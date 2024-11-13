namespace HexPathfinding
{
    internal class PathNodeHex
    {
        private bool _isWalkable = true;
        private PathNodeHex _cameFromNode;
        private int _gCost;
        private int _hCost;

        internal bool IsWalkable => _isWalkable;

        internal int GCost => _gCost;

        internal int FCost => _gCost + _hCost;

        internal PathNodeHex CameFromNode => _cameFromNode;

        internal void SetGCost(int value) => _gCost = value;

        internal void SetHCost(int value) => _hCost = value;

        internal void SetCameFromNode(PathNodeHex node) => _cameFromNode = node;

        internal void SetIsWalkable(bool value) => _isWalkable = value;
    }
}
