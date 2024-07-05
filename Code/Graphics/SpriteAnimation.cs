using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics
{
    public class SpriteAnimation
    {
        private List<SpriteAnimationFrame> _frames = new List<SpriteAnimationFrame>();

        public SpriteAnimationFrame CurrentFrame 
        {
            get
            {
                return _frames
                    .Where(f => f.TimeStamp <= PlaybackProgress)
                    .OrderBy(f => f.TimeStamp)
                    .LastOrDefault();

            }
        }
        public SpriteAnimationFrame this[int index]
        {
            get
            {
                if (index < 0 || index >= _frames.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _frames[index];
            }
        }

        public float Duration 
        { 
            get 
            {
                if (!_frames.Any())
                    return 0;
                return _frames.Max(f => f.TimeStamp);
            } 
        }

        public bool ShouldLoop { get; set; } = true;

        public bool IsPlaying { get; private set; }

        public float PlaybackProgress { get; private set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            SpriteAnimationFrame frame = CurrentFrame;

            if (frame != null)
                frame.Sprite.Draw(spriteBatch, position);
        }

        public void AddFrame(Sprite sprite, float timeStamp)
        {
            SpriteAnimationFrame frame = new SpriteAnimationFrame(sprite, timeStamp);
            
            _frames.Add(frame);
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying)
            {
                PlaybackProgress += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (PlaybackProgress > Duration)
                {
                    if (ShouldLoop)
                        PlaybackProgress -= Duration;
                    else
                        Stop();
                }
            }

        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            PlaybackProgress = 0;
        }

        public void Clear()
        {
            Stop();
            _frames.Clear();
        }
    }
}
