//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using MonoGame.Extended;
//using MonoGame.Extended.Collisions;
//using nameless.Interfaces;

//namespace nameless.Collisions;

//public class MainSideCollider : DynamicCollider 
//{
//    private List<Side> collisionBuffer = new List<Side>();
//    private List<SideCollider> children = new List<SideCollider>();

//    public MainSideCollider() : base()
//    {
//        //Globals.CharacterColliders.Add(this);

//        foreach (var side in new[] { Side.Left, Side.Right, Side.Top, Side.Bottom })
//        {
//            var collider = new SideCollider(this,side);
//            children.Add(collider);
//        }
//    }

//    public void AddToBuffer(Side collisionSide)
//    {
//        if (!collisionBuffer.Contains(collisionSide))
//            collisionBuffer.Add(collisionSide);
//    }

//    public override void SetCollision(IEntity gameObject, int width, int height)
//    {
//        foreach (var child in children)
//        {
//            child.SetCollision(gameObject, width, height);
//        }

//        base.SetCollision(gameObject, width, height);
//    }

//    public void UpdateCollision()
//    {
//        if (collisionBuffer.Count > 0)
//        {
//            OnOuterCollision(collisionBuffer.ToList());
//        }
//        collisionBuffer.Clear();
//    }

//    public virtual void OnOuterCollision(List<Side> collisionInfo)
//    {

//    }

//    public override void Update()
//    {
//        base.Update();

//        foreach (var child in children)
//        {
//            child.Bounds.Position = Bounds.Position + child.offset;
//        }
//    }
//}