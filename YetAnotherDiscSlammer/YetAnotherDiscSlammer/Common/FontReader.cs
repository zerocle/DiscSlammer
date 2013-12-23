using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherDiscSlammer.Common
{
   public class FontReader
   {
      protected int characterW;
      protected int characterH;
      protected String Text;
      protected Texture2D FontSprite;

      public FontReader()
      {

      }

      public Boolean Init(int characterWidth, int characterHeight)
      {
         if (characterWidth <= 0 || characterHeight <= 0)
         {
            return false;
         }
         this.characterW = characterWidth;
         this.characterH = characterHeight;

         return true;
      }

      public void LoadContent(ContentManager Content, String FontFile)
      {
         FontSprite = Content.Load<Texture2D>(FontFile);
      }

      public void Update(GameTime gameTime)
      {

      }

      public void SetText(String Text)
      {
         this.Text = Text;
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float scale)
      {
         int width = (int)Math.Floor((decimal)(characterW * scale));
         int height = (int)Math.Floor((decimal)(characterH * scale));
         foreach (char character in Text.ToUpper())
         {
            int index = 0;
            if(Regex.IsMatch(character.ToString(), "[0-9]"))
            {
               index = (int)character - 48;
               index += 30;
            }
            else if (Regex.IsMatch(character.ToString(), "[A-Z]"))
            {
               index = (int)character - 65;
            }
            else if (character == ' ')
            {
               index = 26;
            }
            else if (character == '.')
            {
               index = 27;
            }
            else if (character == '!')
            {
               index = 28;
            }
            else if (character == '-')
            {
               index = 29;
            }
            else
            {
               index = 26;
            }
            int column = index % 5;
            int row = (int)Math.Floor((decimal)index / 5);
            spriteBatch.Draw(FontSprite, new Rectangle((int)position.X, (int)position.Y, width, height), new Rectangle(column * characterW, row * characterH, characterW, characterH), Color.White);
            position.X += width;
         }
      }

   }
}
