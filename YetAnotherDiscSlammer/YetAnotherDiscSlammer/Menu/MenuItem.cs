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

      public MenuItem()
      {

      }

      public Boolean Initialize(String DisplayValue, Action Callback, Boolean IsEnabled = true)
      {
         if (String.IsNullOrWhiteSpace(DisplayValue) || Callback == null)
         {
            return false;
         }
         this.DisplayValue = DisplayValue;
         this.Callback = Callback;
         this.IsEnabled = IsEnabled;

         return true;

      }
   }
}
