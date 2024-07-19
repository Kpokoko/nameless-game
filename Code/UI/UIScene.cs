using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class UIScene
{
    private List<IUI> _elements {  get; set; }
    public void Clear()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Remove();
        }
        _elements.Clear();
    }
}
