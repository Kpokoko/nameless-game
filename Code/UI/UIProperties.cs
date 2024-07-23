using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

/// <summary>
/// center -- position indicates element center
/// left -- position indicates center of element left border 
/// </summary>
public enum Alignment
{
    Center,
    CenterLeft
}

public enum ButtonActivationProperty
{
    Switch,
    Click
}

public enum FlexDirection
{
    Vertical,
    Horizontal
}

public enum JustifySpaceBetween
{
    Bounds,
    Positions,
}
