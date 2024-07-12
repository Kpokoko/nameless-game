using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics;

public class SpriteAnimationFrame
{
    private Sprite _sprite;
    public Sprite Sprite 
    {
        get => _sprite;
        set
        {
            if (value == null) 
                throw new ArgumentNullException("value", "Sprite == null");
            _sprite = value;
        }
    }
    public float TimeStamp { get; }

    public SpriteAnimationFrame(Sprite sprite, float timeStamp)
    {
        Sprite = sprite;
        TimeStamp = timeStamp;
    }
}
