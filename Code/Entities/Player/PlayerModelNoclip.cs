using nameless.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public partial class PlayerModel
{
    public void Up() => _verticalVelocity = -1000;
    public void StopVertical() => _verticalVelocity = 0;
    public void Down() => _verticalVelocity = 1000;

    public void Right() => _horizontalVelocity = 1000;
    public void StopHorizontal() => _horizontalVelocity = 0;
    public void Left() => _horizontalVelocity = -1000;

    public void UpdateNoclip()
    {

    }

    
}
