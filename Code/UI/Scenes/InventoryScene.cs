using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI.Scenes;

public class InventoryScene : UIScene
{

    private static Vector2 _containerPos = new Vector2(200, 700);
    public InventoryScene() 
    {
        Name = UIScenes.InventoryScene;

        var content = Globals.Inventory.GetInventory().Select(p => p.Key.ToString() + " : " + p.Value.ToString()).ToArray();
        Label[] labels = new Label[content.Length];

        var container = new Container(_containerPos, 300, labels.Length * 60, FlexDirection.Vertical, Vector2.Zero);
        container.OnDrag += () => _containerPos = container.RelativePosition;

        for (int i = 0; i < content.Length; i++)
        {
            labels[i] = new Label(Vector2.Zero, content[i]);
        }

        container.AddElements(labels);
        AddElements(container);

    }
}
