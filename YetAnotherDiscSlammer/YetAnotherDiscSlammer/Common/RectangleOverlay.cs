using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherDiscSlammer.Common
{
   public class RectangleOverlay
   {
      Texture2D dummyTexture;
      public Color Colori;

      public RectangleOverlay(Color colori)
      {
         Colori = colori;
         //Colori.A = (byte)0x10;
      }

      public void LoadContent(IServiceProvider ServiceProvider)
      {
         GameServiceContainer services = ServiceProvider as GameServiceContainer;
         dummyTexture = new Texture2D((GraphicsDevice)services.GetService(typeof(GraphicsDevice)), 1, 1);
         dummyTexture.SetData(new Color[] { Color.White });
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle BoundingRectangle)
      {
         Vector2 origin = new Vector2(dummyTexture.Width / 2.0f, dummyTexture.Height / 2.0f);
         spriteBatch.Draw(dummyTexture, BoundingRectangle, Colori*0.3f);
      }
   }
}
