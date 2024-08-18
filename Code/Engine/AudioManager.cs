using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using nameless.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class AudioManager
{
    public ContentManager Content;
    private Dictionary<SoundType, SoundEffectInstance> _soundEffects;
    private Dictionary<SoundType, int> _playingSounds = new();

    public void Initialize()
    {
        _soundEffects = new Dictionary<SoundType, SoundEffectInstance>
        {
            { SoundType.Death, ResourceManager.SoundDeath.CreateInstance() },
            { SoundType.Click, ResourceManager.SoundClick.CreateInstance() }
        };
    }

    public void PlaySound(SoundType name, float volume = 1, float pitch = 0)
    {
        var instance = _soundEffects[name];
        if (!_playingSounds.ContainsKey(name))
            _playingSounds[name] = 0;
        else if (_playingSounds[name] > 3)
        {
            instance.Stop();
            _playingSounds[name]--;
        }
        instance.Volume = volume;
        instance.Pitch = pitch;
        instance.Play();
        _playingSounds[name]++;
    }
}
