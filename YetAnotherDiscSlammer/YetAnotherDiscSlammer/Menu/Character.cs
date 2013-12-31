using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   public class Character
   {
      public Texture2D Thumbnail;
      public Animation RightWalk;
      public Animation LeftWalk;
      public String Name;

      public Character()
      {

      }

      public bool Initialize(String name)
      {
         this.Name = name;
         return true;
      }

      public void LoadContent(ContentManager Content)
      {
         Thumbnail = Content.Load<Texture2D>("Sprites/Players/" + Name + "/" + Name);
         Texture2D WalkTexture = Content.Load<Texture2D>("Sprites/Players/" + Name + "/" + Name + "_walk");
         SpriteSheet WalkSheet = new SpriteSheet(WalkTexture, 14, 4);

         RightWalk = new Animation(WalkSheet, 0.05f, 28, 41, true);
         LeftWalk = new Animation(WalkSheet, 0.05f, 14, 27, true);
      }
   }
}
