using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Entities
{
   public abstract class Entity
   {
      #region Debug
      protected SpriteFont DebugFont;
      protected RectangleOverlay Outline { get; set; }
      #endregion
      /// <summary>
      /// Entity types are used in collision calculations.
      /// </summary>
      public String EntityType { get; protected set; }
      public Vector2 Position { get; protected set; }
      public virtual Rectangle BoundingRectangle
      {
         get
         {
            return new Rectangle((int)Position.X, (int)Position.Y, 1, 1);
         }
      }

      public Entity(Vector2 Position, String EntityType)
      {
         this.EntityType = EntityType;
         this.Position = Position;
         Outline = new RectangleOverlay(Color.HotPink);
      }

      public virtual void LoadContent(ContentManager content)
      {
         Outline.LoadContent(content.ServiceProvider);
         DebugFont = content.Load<SpriteFont>("Font/Standard");
      }

      public virtual void Update(GameTime gameTime)
      {

      }

      public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         StringOffset = 0;
         DrawOutline(gameTime, spriteBatch);
      }

      public void DrawOutline(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Outline.Draw(gameTime, spriteBatch, BoundingRectangle);
      }
      protected int StringOffset = 0;
      public void DrawString(SpriteBatch spriteBatch,
                              String Value)
      {
         Vector2 pos = Position;
         pos.Y += StringOffset;
         spriteBatch.DrawString(DebugFont, Value, pos, Color.White);
         StringOffset += 10;
      }
   }
}
