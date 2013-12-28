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
      #region Contstants
      private float MAX_HOLD_TIME = 2.0F;
      
      #endregion
      #region Drawing Stuff
      private Texture2D DiscTexture;
      private Texture2D DiscSpinningTexture;
      private Texture2D ShadowTexture;
      private Animation DiscSpinning;
      private Animation DiscStopped;
      private AnimationPlayer sprite;
      #endregion

      public Court Court {get; protected set; }

      public Boolean IsHeld { get; set; }
      public Vector2 Velocity { get; set; }
      public Boolean IsInPlay { get; protected set; }
      public Boolean IgnoreWalls { get; protected set; }
      protected Vector2 _Movement;
      protected const float MaxSpeed = 20.0f;
      protected const float MinSpeed = 5.0f;

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
         IsInPlay = true;
         IgnoreWalls = false;
         this.Court = court;
      }

      public override void LoadContent(ContentManager Content)
      {
         DiscTexture = Content.Load<Texture2D>("Sprites/Disc/GameDisc");
         DiscSpinningTexture = Content.Load<Texture2D>("Sprites/Disc/GameDiscSheet");
         ShadowTexture = Content.Load<Texture2D>("Sprites/Disc/DiscShadow");
         DiscSpinning = new Animation(DiscSpinningTexture, 0.05f, true);
         DiscStopped = new Animation(DiscTexture, 0.05f, true);
         sprite.PlayAnimation(DiscSpinning);
         overlay = new RectangleOverlay(Color.Red);
         overlay.LoadContent(Court.Content.ServiceProvider);
      }
      protected float heldTime = 0.0f;
      public override void Update(GameTime gameTime)
      {
         if (this.IsInPlay)
         {
            if (this.IsHeld)
            {
               heldTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               sprite.PlayAnimation(DiscStopped);
            }
            else
            {
               sprite.PlayAnimation(DiscSpinning);
               Vector2 previousPosition = this.Position;
               this.Position += this._Movement;
               if (!IgnoreWalls)
               {
                  if (Court.CollidesWith(this, "Wall"))
                  {
                     this._Movement.Y = this._Movement.Y * -1;
                     this.Position = previousPosition += this._Movement;
                  }
               }
            }
         }
      }

      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         if (this.IsInPlay)
         {
            base.Draw(gameTime, spriteBatch);
            Vector2 shadowOffset = new Vector2(0, 20);
            spriteBatch.Draw(ShadowTexture, Position + shadowOffset, null, Color.White, 0.0f, new Vector2(ShadowTexture.Width / 2, ShadowTexture.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            sprite.Draw(gameTime, spriteBatch, Position, 0);
         }
      }

      public void Throw(float angle)
      {
         float x = (float)Math.Sin(angle);
         float y = -1 * (float)Math.Cos(angle);
         float time = Math.Min(heldTime, MAX_HOLD_TIME);
         float ratio = 1 - time / MAX_HOLD_TIME;
         float speedMult = (MaxSpeed - MinSpeed) * ratio + MinSpeed;
         this._Movement = new Vector2(x, y) * speedMult;
         heldTime = 0.0f;
      }

      public void ThrowTo(Vector2 ThrowToPosition)
      {
         IsInPlay = true;
         IgnoreWalls = true;
         Vector2 movementVector = ThrowToPosition - Position;
         movementVector.Normalize();
         this._Movement = movementVector * MinSpeed;
      }

      public void TakeOutOfPlay()
      {
         IsInPlay = false;
         this.Position = new Vector2(Settings.Instance.Width / 2, Settings.Instance.Height);
         this.Velocity = Vector2.Zero;
         this._Movement = Vector2.Zero;
      }

      public void BringInPlay()
      {
         this.IsInPlay = true;
         this.IgnoreWalls = false;
      }

      public void SetPosition(Vector2 position)
      {
         this.Position = position;
      }
   }
}
