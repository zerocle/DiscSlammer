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
   public class SpriteSheet
   {
      /// <summary>
      /// All frames in the animation arranged horizontally.
      /// </summary>
      public Texture2D Texture
      {
         get { return texture; }
      }
      Texture2D texture;

      /// <summary>
      /// Gets the number of frames in the animation.
      /// </summary>
      public int FrameCount
      {
         get { return Texture.Width / FrameWidth * Texture.Height / FrameHeight; }
      }

      /// <summary>
      /// Gets the width of a frame in the animation.
      /// </summary>
      public int FrameWidth
      {
         // Assume square frames.
         get { return _frameWidth; }
      }int _frameWidth;

      /// <summary>
      /// Gets the height of a frame in the animation.
      /// </summary>
      public int FrameHeight
      {
         get { return _frameHeight; }
      }int _frameHeight;

      // This constructor should be used when using a one row, square animation
      public SpriteSheet(Texture2D texture)
         : this(texture, texture.Width / texture.Height, 1)
      {
      }
      // This should be used with a one row, rectangular animation
      public SpriteSheet(Texture2D texture, int framesWide)
         : this(texture, framesWide, 1)
      {
      }

      // This should be used with a multi-row animation
      public SpriteSheet(Texture2D texture, int framesWide, int framesHigh)
      {
         this.texture = texture;
         this._frameWidth = texture.Width / framesWide;
         this._frameHeight = texture.Height / framesHigh;
      }

      public Rectangle GetFrame(int FrameIndex)
      {
         int column = FrameIndex % (Texture.Width / FrameWidth);
         int row = (int)Math.Floor((decimal)FrameIndex / (Texture.Width / FrameWidth));

         Rectangle temp = new Rectangle(column * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
         return temp;
      }
   }
}
