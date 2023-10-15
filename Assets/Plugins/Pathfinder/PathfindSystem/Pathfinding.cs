using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UIElements;
using System;

namespace Pathfinder
{
    public static class Pathfinding 
    {
        public static List<BaseCell> FindPath(CellGrid cellGrid, BaseCell startNode, BaseCell targetNode)
        {
            List<BaseCell> path = new List<BaseCell>();

            List<BaseCell> openSet = new List<BaseCell>();
            HashSet<BaseCell> closedSet = new HashSet<BaseCell>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                BaseCell currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    path = RetracePath(startNode, targetNode);
                    break;
                }

                List<BaseCell> neighbors = GetNeighbors(currentNode, MakeArray(cellGrid));
                foreach (BaseCell neighbor in neighbors)
                {
                    if (closedSet.Contains(neighbor) || neighbor.IsObstacle || neighbor.Owner != null)
                        continue;

                    int newCostToNeighbor = currentNode.gCost + GetDistanceEuclidean(currentNode, neighbor);

                    if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistanceEuclidean(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return path;
        } 

        public static int GetDistanceEuclideanWorldPosition(BaseCell nodeA, BaseCell nodeB)
        {
            Vector3 posA = nodeA.transform.position;
            Vector3 posB = nodeB.transform.position;

            int distX = (int)Mathf.Abs(posA.x - posB.x);
            int distZ = (int)Mathf.Abs(posA.z - posB.z);

            return distX + distZ;
        }

        public static int GetDistanceEuclidean(BaseCell nodeA, BaseCell nodeB)
        {
            int distX = Mathf.Abs(nodeA.x - nodeB.x);
            int distZ = Mathf.Abs(nodeA.y - nodeB.y);

            return distX + distZ;
        }

        public static int GetOrtoDistance(BaseCell nodeA, BaseCell nodeB)
        {
            int distX = Mathf.Abs(nodeA.x - nodeB.x);
            int distZ = Mathf.Abs(nodeA.y - nodeB.y);
            return distX > distZ ? distX : distZ;
        }
        private static List<BaseCell> GetNeighbors(BaseCell node, BaseCell[,] cells2)
        {
            List<BaseCell> neighbors = new List<BaseCell>();

            int nodeX = -1;
            int nodeY = -1;

            for (int x = 0; x < cells2.GetLength(0); x++)
            {
                for (int y = 0; y < cells2.GetLength(1); y++)
                {
                    if (cells2[x, y] == node)
                    {
                        nodeX = x;
                        nodeY = y;
                        break;
                    }
                }

                if (nodeX != -1 && nodeY != -1)
                    break;
            }

            if (nodeX == -1 || nodeY == -1)
                return neighbors;

            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int checkX = nodeX + xOffset;
                    int checkY = nodeY + yOffset;

                    if (checkX >= 0 && checkX < cells2.GetLength(0) &&
                        checkY >= 0 && checkY < cells2.GetLength(1) &&
                        (checkX != nodeX || checkY != nodeY))
                    {
                        neighbors.Add(cells2[checkX, checkY]);
                    }
                }
            } 
            return neighbors;
        }

        private static List<BaseCell> RetracePath(BaseCell startNode, BaseCell endNode)
        {
            List<BaseCell> path = new List<BaseCell>();
            BaseCell currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }


        private static BaseCell[,] MakeArray(CellGrid cellGrid)
        {
            var cells2 = new BaseCell[cellGrid.Rows, cellGrid.Columns];
            var cells = cellGrid.GetCells();

            int counter = 0;
            for (int row = 0; row < cellGrid.Rows; row++)
            {
                for (int col = 0; col < cellGrid.Columns; col++)
                {
                    cells2[row, col] = cells[counter];
                    counter++;
                }
            }
            return cells2;
        }
    }
}
