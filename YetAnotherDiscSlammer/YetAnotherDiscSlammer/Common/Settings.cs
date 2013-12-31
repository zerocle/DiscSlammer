using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YetAnotherDiscSlammer.Common
{
   public enum ControlDevice
   {
      Keyboard,
      ControllerOne,
      ControllerTwo,
      ControllerThree,
      ControllerFour,
      AI,
      None,
      Unselected
   };
   public class Settings
   {
      public static Settings Instance
      {
         get
         {
            if (_instance == null)
            {
               _instance = new Settings();
            }
            return _instance;
         }
      }
      private static Settings _instance;
      public int Height { get; set; }
      public int Width { get; set; }
      public bool ShowBoundingBox { get; set; }
      public ControlDevice PlayerOneController { get; set; }
      public ControlDevice PlayerTwoController { get; set; }
      protected Settings()
      {
         PlayerOneController = ControlDevice.ControllerOne;
         PlayerTwoController = ControlDevice.AI;
         ShowBoundingBox = false;
      }

      public void Initialize(int Height, int Width)
      {
         this.Height = Height;
         this.Width = Width;
      }
   }
}
