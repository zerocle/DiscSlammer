using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   public class ChoiceShifterKeyboard: ChoiceShifter
   {
      protected KeyboardState LastKeyboardState;

      public ChoiceShifterKeyboard(ControlDevice Device)
         :base(Device)
      {
         if (Device != ControlDevice.Keyboard)
         {
            throw new ArgumentException("Keyboard shift choosers can only be initialized for keyboard control devices.");
         }
      }
      public override int GetShift(GameTime gameTime)
      {
         int shift = 0;
         KeyboardState ks = Keyboard.GetState();
         if (ks.IsKeyDown(Keys.Right) && LastKeyboardState.IsKeyUp(Keys.Right) ||
            ks.IsKeyDown(Keys.D) && LastKeyboardState.IsKeyUp(Keys.D))
         {
            shift = 1;
         }
         else if (ks.IsKeyDown(Keys.Left) && LastKeyboardState.IsKeyUp(Keys.Left) ||
            ks.IsKeyDown(Keys.A) && LastKeyboardState.IsKeyUp(Keys.A))
         {
            shift = -1;
         }
         return shift;
      }
      protected KeyboardState GetKeyboardState()
      {
         return Keyboard.GetState();
      }
   }
}
