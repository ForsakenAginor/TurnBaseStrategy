using Assets.Scripts.HexGrid;
using System;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private HexGridCreator _gridCreator;
    [SerializeField] private MeshUpdater _meshUpdater;

    private void Awake()
    {
        _gridCreator.Init();
        _meshUpdater.Init(_gridCreator.HexGrid);
        TextureAtlasReader atlas = _meshUpdater.GetComponent<TextureAtlasReader>();
    }
}
