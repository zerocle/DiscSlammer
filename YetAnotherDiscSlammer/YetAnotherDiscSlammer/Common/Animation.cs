using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YetAnotherDiscSlammer.Common
{
   public class Animation
   {
      public SpriteSheet SpriteSheet
      {
         get { return _SpriteSheet; }
      }SpriteSheet _SpriteSheet;
      public int FrameStart
      {
         get { return _FrameStart; }
      }int _FrameStart;
      public int FrameEnd
      {
         get { return _FrameEnd; }
      }int _FrameEnd;
      public int FrameCount
      {
         get { return FrameEnd - FrameStart; }
      }
      public float FrameTime
      {
         get { return _FrameTime; }
      }float _FrameTime;
      public Boolean IsLooping
      {
         get { return _IsLooping; }
      }Boolean _IsLooping;

      public Animation(SpriteSheet spriteSheet, float FrameTime, int FrameStart, int FrameEnd, Boolean IsLooping)
      {
         this._SpriteSheet = spriteSheet;
         this._FrameTime = FrameTime;
         this._FrameStart = FrameStart;
         this._FrameEnd = FrameEnd;
         this._IsLooping = IsLooping;
      }
   }
}
