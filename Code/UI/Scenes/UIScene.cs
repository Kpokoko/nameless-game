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
    private List<IUI> _elements { get; set; } = new();
    public UIScenes Name { get; protected set; }

    public void Clear()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Remove();
        }
        _elements.Clear();
    }

    protected void AddElements(params IUI[] elements)
    {
        _elements = _elements.Concat(elements).ToList();
    }
}
