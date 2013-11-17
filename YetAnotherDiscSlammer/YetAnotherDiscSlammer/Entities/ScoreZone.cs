using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Entities
{
   public class ScoreZone
   {
      #region Debug
      protected RectangleOverlay Overlay { get; set; }
      #endregion
      protected Court _court; 
      public Rectangle ZoneBounds { get; protected set; }

      public ScoreZone(Court court, Rectangle zoneBounds)
      {
         this._court = court;
         this.ZoneBounds = zoneBounds;
         Overlay = new RectangleOverlay(Color.MediumVioletRed);
      }

      public void LoadContent()
      {
         Overlay.LoadContent(_court.Content.ServiceProvider);
      }

      public void Update(GameTime gameTime)
      {
         
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Overlay.Draw(gameTime, spriteBatch, ZoneBounds);
      }
   }
}
