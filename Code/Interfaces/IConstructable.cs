using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IConstructable
{
    public void UpdateConstructor();

    public event Action OnSelection;
    public bool IsSelected { get; set; }
    public bool IsEnableToPlayer { get; set; }
    public int Layer { get; set; }

}
