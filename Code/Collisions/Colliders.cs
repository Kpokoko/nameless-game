using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Collisions;

public class Colliders
{
    public List<Collider> colliders = new List<Collider>();
    public Vector2 Position {
        get { return colliders[0].Position; }
        set { for (int i = 0; i < colliders.Count; i++) { colliders[i].Position = value; } }
    }

    public void Add(Collider collider)
    {
        colliders.Add(collider);
    }

    public void Remove(Collider collider)
    {
        colliders.Remove(collider);
        collider.RemoveCollider();
    }

    public void RemoveAll()
    {
        for (int i = 0;i < colliders.Count;i++) 
        { colliders[i].RemoveCollider(); }
        colliders.Clear();
    }

    public void RemoveTriggerHitboxes()
    {

    }

    public Collider this[int index]
    {
        get { return colliders[index]; }
    }
}
