using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IBreakable
{
    public bool Broken { get; set; }
    public void Break() { }
}
