using UnityEngine;
using System.Linq;
using System.Collections.Generic; 

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pathfinder
{
    public class CellGrid : MonoBehaviour
    {
        [SerializeField] private BaseCell cellPrefab;
        [SerializeField] private Vector2 size = Vector2.one;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private bool isDrawGizmos = false;
         
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private List<BaseCell> cells = new List<BaseCell>();

        internal int Rows { get => rows; }
        internal int Columns { get => columns; }

        private void GenerateCells()
        {
            if (cells.Count > 0) ClearGrid();

            var newCell = PrefabUtility.InstantiatePrefab(cellPrefab.gameObject) as GameObject;
            var cell = newCell.GetComponent<BaseCell>(); 

            float cellSizeX = cell.GetCollider().bounds.size.x;
            float cellSizeY = cell.GetCollider().bounds.size.y;
            float cellSizeZ = cell.GetCollider().bounds.size.z;

            DestroyImmediate(newCell); 
            if (cellSizeX == 0 || cellSizeZ == 0) return;

            rows = Mathf.FloorToInt(size.x / cellSizeX);
            columns = Mathf.FloorToInt(size.y / cellSizeZ);

            int counter = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 cellPosition = 
                        new Vector3(
                            transform.position.x + row * cellSizeZ - size.x / 2 + cellSizeX/2,
                            transform.position.y,
                            transform.position.z + col * cellSizeX - size.y / 2 + cellSizeZ/2
                        );
                    CreateCell(cellPosition, row, col, counter, cellSizeX, cellSizeY, cellSizeZ);
                    counter++;
                }
            } 
        }  
        private void ClearGrid()
        {
            var count = transform.childCount;
            for (int i = 0; i < count; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
            
            cells.Clear();
            rows = 0;
            columns = 0;
        }

        #region Mono
        private void Awake()
        {
            cells = transform.GetComponentsInChildren<BaseCell>().ToList();
        } 
        #endregion

        private void CreateCell(Vector3 cellPosition, int x, int y, int counter, float cellSizeX, float cellSizeY, float cellSizeZ)
        {
            var newCell = PrefabUtility.InstantiatePrefab(cellPrefab.gameObject) as GameObject;
            newCell.transform.position = cellPosition;
            newCell.transform.parent = this.transform;

            var cellScr = newCell.GetComponent<BaseCell>();
            if (cells.Count != 0)
            {
                cellScr.x = x; 
                cellScr.y = y;

                Vector3 checkSize = new Vector3(cellSizeX, cellSizeY, cellSizeZ);
                if (Physics.CheckBox(cellPosition, checkSize / 2f, Quaternion.identity, obstacleLayer))
                    cellScr.SetObstacle(true);
                
            }
            cells.Add(cellScr);
            newCell.name = newCell.name + counter;
        }

        internal List<BaseCell> GetCells() { return cells; }
        internal BaseCell GetNearestFreeCell(Vector3 position)
        {
            BaseCell nearestGrid = null;
            float nearestDistance = Mathf.Infinity;

            foreach (var grid in cells)
            {
                if (!grid.IsFreeCell) continue;

                float distance = Vector3.Distance(position, grid.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestGrid = grid;
                }
            }
            return nearestGrid; 
        } 
        
        private void OnDrawGizmos()
        {
            if (!isDrawGizmos) return;

            Vector3 position = transform.position;
            Vector3 halfSize = new Vector3(size.x * 0.5f, 0f, size.y * 0.5f);

            // Calculate corners of the rectangle
            Vector3 topLeft = position - halfSize;
            Vector3 topRight = position + new Vector3(halfSize.x, 0f, -halfSize.z);
            Vector3 bottomLeft = position + new Vector3(-halfSize.x, 0f, halfSize.z);
            Vector3 bottomRight = position + halfSize;

            // Draw Gizmo lines between corners
            Gizmos.color = Color.green;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}

