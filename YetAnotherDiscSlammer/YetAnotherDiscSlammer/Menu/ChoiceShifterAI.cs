using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   class ChoiceShifterAI : ChoiceShifter
   {
      protected const float shiftAfter = 2.0f;
      protected float elapsedTime;
      public ChoiceShifterAI(ControlDevice Device)
         :base(Device)
      {
         if (Device != ControlDevice.AI)
         {
            throw new ArgumentException("AI shift chooser's can only be intialized with non-ai control devices");
         }
      }

      public override int GetShift(GameTime gameTime)
      {
         int shift = 0;
         elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
         if (elapsedTime > shiftAfter)
         {
            elapsedTime = 0.0f;
            shift = new Random(DateTime.Now.Millisecond).Next(2) - 1;
         }
         return shift;
      }
   }
}
