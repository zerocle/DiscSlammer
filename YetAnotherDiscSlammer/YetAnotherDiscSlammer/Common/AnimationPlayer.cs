using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherDiscSlammer.Common
{
   /// <summary>
   /// Controls playback of an AnimationTexture.
   /// </summary>
   public struct AnimationPlayer
   {
      public Animation Animation
      {
         get
         {
            return _Animation;
         }
      }Animation _Animation;

      /// <summary>
      /// Gets the index of the current frame in the animation.
      /// </summary>
      public int FrameIndex
      {
         get { return frameIndex; }
      }
      int frameIndex;

      /// <summary>
      /// The amount of time in seconds that the current frame has been shown for.
      /// </summary>
      private float time;

      /// <summary>
      /// Gets a texture origin at the center of each frame.
      /// </summary>
      public Vector2 Origin
      {
         get { return new Vector2(Animation.SpriteSheet.FrameWidth / 2.0f, Animation.SpriteSheet.FrameHeight / 2.0f); }
      }

      /// <summary>
      /// Sets the playback animation.
      /// </summary>
      public void SetAnimation(Animation animation)
      {
         // If this animation is already running, do not restart it.
         if (Animation == animation)
            return;

         // Start the new animation.
         this._Animation = animation;
         this.frameIndex = animation.FrameStart;
         this.time = 0.0f;
      }

      /// <summary>
      /// Advances the time position and draws the current frame of the animation.
      /// </summary>
      public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float angle)
      {
         if (Animation == null)
            throw new NotSupportedException("No animation is currently playing.");

         // Process passing time.
         time += (float)gameTime.ElapsedGameTime.TotalSeconds;
         while (time > Animation.FrameTime)
         {
            time -= Animation.FrameTime;

            // Advance the frame index; looping or clamping as appropriate.
            if (Animation.IsLooping)
            {
               frameIndex = Animation.FrameStart + ((frameIndex + 1 - Animation.FrameStart) % (Animation.FrameCount + 1));
            }
            else
            {
               frameIndex = Math.Min(frameIndex + 1, Animation.FrameEnd);
            }
         }

         Rectangle source = Animation.SpriteSheet.GetFrame(FrameIndex);

         // Draw the current frame.
         spriteBatch.Draw(Animation.SpriteSheet.Texture, position, source, Color.White, angle, Origin, 1.0f, SpriteEffects.None, 0.0f);
      }
   }
}
