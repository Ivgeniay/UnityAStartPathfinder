using System;
using UnityEngine; 

namespace Pathfinder
{
    public class BaseCell : MonoBehaviour
    {
        [SerializeField] protected Collider col;
        [SerializeField] protected Renderer rend;
        [SerializeField] protected int movementCost;
        [SerializeField] protected bool isObstacle;

        internal bool IsFreeCell { get => Owner == null; }
        internal int MovementCost { get => movementCost; } 
        internal int fCost { get { return gCost + hCost; } }
        internal int hCost { get; set; }
        internal int gCost { get; set; }
        internal int x { get; set; }
        internal int y { get; set; }
        internal bool IsObstacle { get => isObstacle; }
        internal BaseCell parent;
        internal object Owner;


        internal Collider GetCollider() => col;
        

        internal void SetParent(BaseCell cell) => parent = cell; 
        internal void SetOwner(object owner) => Owner = owner; 
        internal void SetObstacle(bool value) => isObstacle = value;
        internal void Enabled(bool v) => rend.enabled = v;
    }
}
