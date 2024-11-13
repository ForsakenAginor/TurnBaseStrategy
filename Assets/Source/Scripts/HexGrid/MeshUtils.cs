using UnityEngine;

namespace Assets.Scripts.HexGrid
{
    public static class MeshUtils
    {
        private static readonly Quaternion[] _cachedQuaternionEulerXZArr;

        static MeshUtils()
        {
            _cachedQuaternionEulerXZArr = new Quaternion[360];

            for (int i = 0; i < 360; i++)
                _cachedQuaternionEulerXZArr[i] = Quaternion.Euler(0, i, 0);
        }

        public static MeshData CreateEmptyMeshArrays(int quadCount)
        {
            MeshData meshData = new ()
            {
                Vertices = new Vector3[4 * quadCount],
                UVs = new Vector2[4 * quadCount],
                Triangles = new int[6 * quadCount],
            };
            return meshData;
        }

        public static void AddToMeshArraysXZ(MeshData meshData, int index, Vector3 position, float rotation, Vector3 quadSize, UVCoordinates uv)
        {
            int verticleIndex = index * 4;
            int[] verticlesIndexes = new int[4];
            int currentIndex;

            for (int i = 0; i < verticlesIndexes.Length; i++)
                verticlesIndexes[i] = verticleIndex + i;

            Vector3 center = quadSize * 0.5f;
            bool skewed = center.x != center.z;

            if (skewed)
            {
                currentIndex = 0;

                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rotation) * new Vector3(-center.x, 0, center.z));
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rotation) * new Vector3(-center.x, 0, -center.z));
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rotation) * new Vector3(center.x, 0, -center.z));
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rotation) * center);
            }
            else
            {
                float rightBottomCorner = rotation + 270;
                float leftBottomCorner = rotation + 180;
                float leftTopCorner = rotation + 90;
                float rightTopCorner = rotation;

                currentIndex = 0;

                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rightBottomCorner) * center);
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(leftBottomCorner) * center);
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(leftTopCorner) * center);
                meshData.Vertices[verticlesIndexes[currentIndex++]] = position + (GetQuaternionEulerXZ(rightTopCorner) * center);
            }

            currentIndex = 0;

            meshData.UVs[verticlesIndexes[currentIndex++]] = new Vector2(uv.UV00.x, uv.UV11.y);
            meshData.UVs[verticlesIndexes[currentIndex++]] = new Vector2(uv.UV00.x, uv.UV00.y);
            meshData.UVs[verticlesIndexes[currentIndex++]] = new Vector2(uv.UV11.x, uv.UV00.y);
            meshData.UVs[verticlesIndexes[currentIndex++]] = new Vector2(uv.UV11.x, uv.UV11.y);

            int triatngleIndex = index * 6;

            meshData.Triangles[triatngleIndex++] = verticlesIndexes[0];
            meshData.Triangles[triatngleIndex++] = verticlesIndexes[3];
            meshData.Triangles[triatngleIndex++] = verticlesIndexes[1];

            meshData.Triangles[triatngleIndex++] = verticlesIndexes[1];
            meshData.Triangles[triatngleIndex++] = verticlesIndexes[3];
            meshData.Triangles[triatngleIndex++] = verticlesIndexes[2];
        }

        private static Quaternion GetQuaternionEulerXZ(float rotationFloat)
        {
            int rotation = Mathf.RoundToInt(rotationFloat);
            rotation %= 360;

            if (rotation < 0)
                rotation += 360;

            return _cachedQuaternionEulerXZArr[rotation];
        }
    }
}