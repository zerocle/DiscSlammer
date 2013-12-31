using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   public abstract class ChoiceShifter
   {
      protected ControlDevice PlayerControlDevice;

      public ChoiceShifter(ControlDevice Device)
      {
         this.PlayerControlDevice = Device;
      }
      public abstract int GetShift(GameTime gameTime);
   }
}
