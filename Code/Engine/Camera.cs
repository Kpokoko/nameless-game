using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        public Camera()
        {
            Zoom = 1.0f;
            Rotation = 0.0f;
            Position = Vector2.Zero;
        }

        public void Update()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(Zoom) *
                        Matrix.CreateTranslation(new Vector3
                            (Globals.Engine.GraphicsDevice.Viewport.Width * 0.5f, Globals.Engine.GraphicsDevice.Viewport.Height * 0.5f, 0));
        }
    }

}
