using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YetAnotherDiscSlammer.Menu
{
   public class MenuItem
   {
      public String DisplayValue { get; set; }
      public Action Callback { get; protected set; }
      public Boolean IsEnabled { get; set; }

      public MenuItem(String DisplayValue, Action Callback, Boolean IsEnabled = true)
      {
         this.DisplayValue = DisplayValue;
         this.Callback = Callback;
         this.IsEnabled = IsEnabled;
      }
   }
}
