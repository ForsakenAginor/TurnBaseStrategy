using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polygen.HexagonGenerator
{
    //Tile selection system prototype
    public class SelectionManager : MonoBehaviour
    {
        public Transform selectionUI;

        private void Start()
        {
            selectionUI.gameObject.SetActive(true);
        }

        private void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
                {
                    Tile tile = hit.collider.GetComponentInParent<Tile>();
                    if (tile != null)
                    {
                        selectionUI.position = tile.transform.position + Vector3.up * .01f;
                    }
                }
            }
        }
    }
}
