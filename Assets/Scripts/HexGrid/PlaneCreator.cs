using System;
using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    [RequireComponent(typeof(MeshFilter))]
    public class PlaneCreator : MonoBehaviour
    {
        public void Init(HexGridXZ<CellSprite> grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            UVCoordinates uvCoordinates = new ()
            {
                UV00 = Vector2.zero,
                UV11 = Vector2.one,
            };
            MeshFilter filter = GetComponent<MeshFilter>();

            UpdateMesh(grid, filter.mesh, uvCoordinates);
        }

        private void UpdateMesh(HexGridXZ<CellSprite> grid, Mesh mesh, UVCoordinates uv)
        {
            MeshData meshData = MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height);
            Vector3 quadSize = new Vector3(1, 0, 1) * grid.CellSize;
            Vector3 offset = new (0, -0.05f, 0);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    int index = (x * grid.Height) + y;
                    MeshUtils.AddToMeshArraysXZ(meshData, index, grid.GetCellWorldPosition(x, y) + offset, 0f, quadSize, uv);
                }
            }

            mesh.vertices = meshData.Vertices;
            mesh.triangles = meshData.Triangles;
            mesh.uv = meshData.UVs;
        }
    }
}