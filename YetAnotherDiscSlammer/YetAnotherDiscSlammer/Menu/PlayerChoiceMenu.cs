using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherDiscSlammer.Menu
{
   public class PlayerChoiceMenu
   {
      protected List<Character> Characters;

      public PlayerChoiceMenu()
      {

      }

      public bool Init(ContentManager Content)
      {
         try
         {
            Characters = new List<Character>();
            Character Guile = new Character();
            if (Guile.Initialize(Content, "Guile"))
            {
               Characters.Add(Guile);
            }
            Character Kid = new Character();
            if (Kid.Initialize(Content, "Kid"))
            {
               Characters.Add(Kid);
            }
            Character Serge = new Character();
            if (Serge.Initialize(Content, "Serge"))
            {
               Characters.Add(Serge);
            }
            Character Viper = new Character();
            if (Viper.Initialize(Content, "Viper"))
            {
               Characters.Add(Viper);
            }
         }
         catch (Exception err)
         {

         }
         return true;
      }

      public void Update(GameTime gameTime)
      {

      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Vector2 offset = Vector2.Zero;
         foreach (Character c in Characters)
         {
            spriteBatch.Draw(c.Thumbnail, new Vector2(100, 100) + offset, Color.White);
            offset.X += c.Thumbnail.Width;
         }
      }

   }
}
