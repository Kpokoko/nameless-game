using nameless.Interfaces;
using nameless.UI.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class UIScene
{
    private List<UIElement> _elements { get; set; } = new();
    public UIScenes Name { get; protected set; }

    public void Clear()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Remove();
        }
        _elements.Clear();
    }

    public void AddElements(params UIElement[] elements)
    {
        _elements = _elements.Concat(elements).ToList();
    }

    public void RemoveElements(params UIElement[] elements)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i] == null) continue;
            _elements.Remove(elements[i]);
            elements[i].Remove();
        }
    }
}
