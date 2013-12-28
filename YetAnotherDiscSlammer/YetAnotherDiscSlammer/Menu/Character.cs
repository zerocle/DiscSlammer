using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherDiscSlammer.Menu
{
   public class Character
   {
      public Texture2D Thumbnail;

      public Character()
      {

      }

      public bool Initialize(ContentManager Content, String name)
      {
         Thumbnail = Content.Load<Texture2D>("Sprites/Players/" + name + "/" + name);
         return true;
      }
   }
}
