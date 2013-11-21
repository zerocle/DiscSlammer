using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace YetAnotherDiscSlammer.Helpers
{
   public class InputState
   {
      public Vector2 MovementStickDirection;
      public Boolean IsDiveButtonDown;
      public Boolean WasDiveButtonDown;
      public InputState()
      {
         MovementStickDirection = Vector2.Zero;
         IsDiveButtonDown = false;
         WasDiveButtonDown = false;
      }
   }
}
