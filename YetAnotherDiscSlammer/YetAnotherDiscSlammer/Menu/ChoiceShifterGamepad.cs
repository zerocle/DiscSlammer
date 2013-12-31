using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   class ChoiceShifterGamepad : ChoiceShifter
   {
      protected GamePadState LastState;

      public ChoiceShifterGamepad(ControlDevice Device)
         :base(Device)
      {
         if (Device != ControlDevice.ControllerOne &&
            Device != ControlDevice.ControllerTwo &&
            Device != ControlDevice.ControllerThree &&
            Device != ControlDevice.ControllerFour)
         {
            throw new ArgumentException("Gamepad shift chooser's can only be intialized with gamepad control devices");
         }
      }

      public override int GetShift(GameTime gameTime)
      {
         int shift = 0;
         GamePadState gs;
         switch (PlayerControlDevice)
         {
            case ControlDevice.ControllerOne:
               gs = GamePad.GetState(PlayerIndex.One);
               break;
            case ControlDevice.ControllerTwo:
               gs = GamePad.GetState(PlayerIndex.One);
               break;
            case ControlDevice.ControllerThree:
               gs = GamePad.GetState(PlayerIndex.One);
               break;
            case ControlDevice.ControllerFour:
               gs = GamePad.GetState(PlayerIndex.One);
               break;
            default:
               throw new ArgumentOutOfRangeException("Cannot get a gamepad state for non-controller devices", new Exception());
         }
         //Only Released Right
         if (gs.IsButtonDown(Buttons.LeftThumbstickRight) && LastState.IsButtonUp(Buttons.LeftThumbstickRight))
         {
            shift = 1;
         }
         else if (gs.IsButtonDown(Buttons.LeftThumbstickLeft) && LastState.IsButtonUp(Buttons.LeftThumbstickLeft))
         {
            shift = -1;
         }

         LastState = gs;

         return shift;
      }
   }
}
