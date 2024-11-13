using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    internal class MeshCreator
    {
        private readonly HexGridXZ<CellSprite> _grid;
        private readonly Mesh _mesh;
        private readonly IReadOnlyDictionary<CellSprite, UVCoordinates> _spriteUVDictionary;

        internal MeshCreator(HexGridXZ<CellSprite> grid, Mesh mesh, IReadOnlyDictionary<CellSprite, UVCoordinates> dictionary)
        {
            _grid = grid != null ? grid : throw new ArgumentNullException(nameof(grid));
            _mesh = mesh != null ? mesh : throw new ArgumentNullException(nameof(_mesh));
            _spriteUVDictionary = dictionary != null ? dictionary : throw new ArgumentNullException(nameof(_spriteUVDictionary));
        }

        internal void UpdateMesh()
        {
            MeshData meshData = MeshUtils.CreateEmptyMeshArrays(_grid.Width * _grid.Height);
            Vector3 quadSize = new Vector3(1, 0, 1) * _grid.CellSize;

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    int index = (x * _grid.Height) + y;
                    CellSprite cell = _grid.GetGridObject(x, y);

                    MeshUtils.AddToMeshArraysXZ(meshData, index, _grid.GetCellWorldPosition(x, y), 0f, quadSize, _spriteUVDictionary[cell]);
                }
            }

            _mesh.vertices = meshData.Vertices;
            _mesh.triangles = meshData.Triangles;
            _mesh.uv = meshData.UVs;
        }
    }
}