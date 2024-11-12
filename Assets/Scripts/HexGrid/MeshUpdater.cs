using System;
using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(TextureAtlasReader))]
    public class MeshUpdater : MonoBehaviour
    {
        private HexGridXZ<CellSprite> _grid;
        private MeshCreator _meshUpdater;
        private bool _isNeedToUpdate = true;

        private void LateUpdate()
        {
            if (_isNeedToUpdate)
            {
                _isNeedToUpdate = false;
                _meshUpdater.UpdateMesh();
            }
        }

        private void OnDestroy()
        {
            _grid.GridObjectChanged -= OnGridObjectChanged;
        }

        public void Init(HexGridXZ<CellSprite> grid)
        {
            _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));

            Mesh mesh = new ();
            GetComponent<MeshFilter>().mesh = mesh;
            TextureAtlasReader atlasReader = GetComponent<TextureAtlasReader>();
            _meshUpdater = new (_grid, mesh, atlasReader.GetUVDictionary());

            _grid.GridObjectChanged += OnGridObjectChanged;
        }

        private void OnGridObjectChanged(Vector2Int nonmatterValue)
        {
            _isNeedToUpdate = true;
        }
    }
}