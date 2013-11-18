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
   public class Disc : Entity
   {
      #region Debug
      RectangleOverlay overlay;
      #endregion
      private Animation DiscSpinning;
      private AnimationPlayer sprite;
      public Court Court {get; protected set; }
      public Boolean IsScored { get; protected set; }

      public Vector2 Velocity { get; protected set; }
      protected Vector2 _Movement;
      protected const float MaxSpeed = 10.0f;

      protected float boundingRadius = 16.0f;
      public override Rectangle  BoundingRectangle
      {
	      get 
	      { 
            int left = (int)Math.Round(Position.X - boundingRadius);
            int top = (int)Math.Round(Position.Y - boundingRadius);
            int width = (int)Math.Round(boundingRadius * 2);
            return new Rectangle(left, top, width, width);
	      }
      }

      public Disc(Court court)
         :base(Vector2.Zero, "Disc")
      {
         this.Court = court;
         LoadContent();
      }

      protected void LoadContent()
      {
         DiscSpinning = new Animation(Court.Content.Load<Texture2D>("Sprites/Disc/GameDiscSheet"), 0.05f, true);
         overlay = new RectangleOverlay(Color.Red);
         overlay.LoadContent(Court.Content.ServiceProvider);
      }

      public void Update(GameTime gameTime)
      {
         sprite.PlayAnimation(DiscSpinning);
         Vector2 previousPosition = this.Position;
         this.Position += this._Movement;
         if (Court.CollidesWithWall(this))
         {
            this._Movement.Y = this._Movement.Y * -1;
            this.Position = previousPosition += this._Movement;
         }
      }

      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         overlay.Draw(gameTime, spriteBatch, BoundingRectangle);
         sprite.Draw(gameTime, spriteBatch, Position, 0);
      }

      public void Throw(float angle)
      {
         float x = (float)Math.Sin(angle);
         float y = -1 * (float)Math.Cos(angle);
         this._Movement = new Vector2(x, y) * MaxSpeed ;
      }

      public void SetPosition(Vector2 position)
      {
         this.Position = position;
      }
   }
}
