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
    public SceneInfo SceneInfo { get => Globals.Map[(int)Location.X, (int)Location.Y]; }
    private Container _label;

    public Minimap(Vector2 location, EntityTypeEnum[,] array) :
        base(LocationToAbsolutePosition(location) + Globals.Center, LocationToAbsolutePosition(Vector2.One).X, LocationToAbsolutePosition(Vector2.One).Y)
    {
        Location = location;
        _mapArray = array;
        
        if (!Globals.UIManager.Minimaps.Select(m => m.Location).Contains(location))
            Globals.UIManager.Minimaps.Add(this);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.Transparent;
        bool opaque = false;
        if (Hovered)
        {
            spriteBatch.DrawRectangle(Bounds.RectangleF, Color.Brown, TileSize);
            spriteBatch.FillRectangle(Bounds.RectangleF, Color.Blue * 0.1f);
            opaque = true;
        }
        if ( Globals.SceneManager.CurrentLocation == Location)
        {
            spriteBatch.FillRectangle(Bounds.RectangleF, Color.Yellow * 0.1f);
            opaque = true;
        }

        for (var i = 0; i < _mapArray.GetLength(0);i++)
            for (var j = 0; j < _mapArray.GetLength(1); j++)
            {
                var entity = _mapArray[i,j];
                if (entity is EntityTypeEnum.None || entity is EntityTypeEnum.Pivot) continue;
                var rect = new Rectangle(new Point(i * TileSize + (int)Bounds.RectangleF.Position.X, j * TileSize + (int)Bounds.RectangleF.Position.Y), new Point(TileSize, TileSize));

                switch (entity)
                {
                    case EntityTypeEnum.EditorBlock: color = Color.Purple; break;
                    case EntityTypeEnum.InventoryBlock: color = Color.Green; break;
                    case EntityTypeEnum.Block: color = Color.Brown; break;
                    case EntityTypeEnum.Platform: color = Color.Green; rect.Height = 2; break;
                }

                spriteBatch.FillRectangle(rect, opaque ? color : color * 0.6f);//, TileSize/2 + 1);
            }
        
    }

    public override void Remove()
    {
        Globals.UIManager.Minimaps.Remove(this);
    }

    public void Update()
    {
        if (!Globals.IsDeveloperModeEnabled)
            return;    
        
        if (!Hovered || MouseInputController.OnUIElement)
        {
            HideLabel();
            return;
        }
        ShowLabel();
        if (MouseInputController.LeftButton.IsJustPressed && Location != Globals.SceneManager.CurrentLocation)
        {
            Globals.SceneManager.LoadScene(Location);
        }
            
    }

    public void HideLabel()
    {

        if (_label == null)
            return;
        _label.Remove();
        _label = null;
    }

    public void ShowLabel()
    {
        if (_label != null)
            return;
        var label = new Label(Vector2.Zero, SceneInfo.FullName);
        _label = new Container(AbsolutePosition - LocationToAbsolutePosition(new Vector2(0,0.8f)), (int)label.Bounds.Width, (int)label.Bounds.Height);
        _label.AddElements(label);
    } 

    public static Vector2 LocationToAbsolutePosition(Vector2 location)
    {
        return location * new Vector2(Storage.StorageWidth, Storage.StorageHeight) * TileSize;
    }
}
