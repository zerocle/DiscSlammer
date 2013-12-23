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
   public class ScoreZone : Entity
   {
      protected Court _court;
      protected Vector2 _Size;
      protected Rectangle _BoundingRectangle;
      public PlayerIndex PlayerIndex { get; protected set; }
      public int Score { get; set; }
      public override Rectangle BoundingRectangle
      {
         get
         {
            return _BoundingRectangle;
         }
      }

      public ScoreZone(Court court, Vector2 Position, Vector2 Size, PlayerIndex index)
         :base(Position, "ScoreZone")
      {
         this._court = court;
         this._Size = Size;
         this.PlayerIndex = index;
         this._BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)_Size.X, (int)_Size.Y);
         Outline.Colori = Color.Red;
      }

      public void AddScore()
      {
         this.Score++;
         _court.ResetDisc(PlayerIndex.One);
      }

      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         this.DrawString(spriteBatch, Score.ToString());
         base.Draw(gameTime, spriteBatch);
      }
   }
}
