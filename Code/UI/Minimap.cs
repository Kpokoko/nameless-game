using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Engine;
using nameless.Entity;
using nameless.GameObjects;
using nameless.Interfaces;
using nameless.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class Minimap : UIElement
{
    private EntityTypeEnum[,] _mapArray;
    public static int TileSize = 6;
    public Vector2 Location;
    public Minimap(Vector2 location, EntityTypeEnum[,] array) :
        base(LocationToAbsolutePosition(location) + Globals.Center, LocationToAbsolutePosition(Vector2.One).X, LocationToAbsolutePosition(Vector2.One).Y)
    {
        Location = location;
        _mapArray = array;

        Globals.UIManager.Minimaps.Add(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.Transparent;
        for (var i = 0; i < _mapArray.GetLength(0);i++)
            for (var j = 0; j < _mapArray.GetLength(1); j++)
            {
                var entity = _mapArray[i,j];
                if (entity is EntityTypeEnum.None || entity is EntityTypeEnum.Pivot) continue;
                var rect = new Rectangle(new Point(i * TileSize + (int)Bounds.RectangleF.Position.X, j * TileSize + (int)Bounds.RectangleF.Position.Y), new Point(TileSize, TileSize));

                switch (entity)
                {
                    case EntityTypeEnum.EditorBlock: color = Color.AliceBlue; break;
                    case EntityTypeEnum.InventoryBlock: color = Color.Green; break;
                    case EntityTypeEnum.Block: color = Color.Brown; break;
                    case EntityTypeEnum.Platform: color = Color.Green; rect.Height = 2; break;
                }

                spriteBatch.DrawRectangle(rect, Hovered ? color : color * 0.3f, TileSize/2 + 1);
            }
        if (Hovered)
        {
            spriteBatch.DrawRectangle(Bounds.RectangleF,color, TileSize);
            spriteBatch.FillRectangle(Bounds.RectangleF, Color.Blue * 0.1f);
        }
    }

    public override void Remove()
    {
        Globals.UIManager.Minimaps.Remove(this);
    }

    public void Update()
    {
        if (!Hovered)
            return;
        if (MouseInputController.LeftButton.IsJustPressed && Location != Globals.SceneManager.CurrentLocation)
        {
            Globals.SceneManager.LoadScene(Location);
        }
            
    }

    public static Vector2 LocationToAbsolutePosition(Vector2 location)
    {
        return location * new Vector2(Storage.StorageWidth, Storage.StorageHeight) * TileSize;
    }
}
