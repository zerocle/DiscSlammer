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
   public enum Character
   {
      Scorpion,
      Ogre,
      Troll,
   }
   public class Player : Entity
   {
      #region Debug Stuff
      private RectangleOverlay BoundingOverlay;
      private RectangleOverlay PlayAreaOverlay;
      #endregion

      private Animation idleAnimation;
      private Animation moveAnimation;
      private Animation diveAnimation;
      private Character character = Character.Ogre;

      private AnimationPlayer sprite;
      public Boolean HasDisc { get; set; }
      protected Boolean _HadDisc = false;

      public Rectangle PlayArea { get; protected set; }
      public Court Court { get; protected set; }
      public Vector2 Velocity { get; protected set; }


      // Input configuration
      private const float MoveStickScale = 1.0f;
      private const float MoveAcceleration = 13000.0f;
      private const float MaxMoveSpeed = 1750.0f;
      private const float DiveAcceleration = 102000.0f;
      private const float MaxDiveSpeed = 12050.0f;
      private const float GroundDragFactor = 0.48f;
      private const float MaxDiveTime = 0.15f; // in seconds
      private Buttons DiveButton = Buttons.A;
      private Boolean MovementStickLeft = true;

      /// <summary>
      /// Gets whether or not the player's diving
      /// </summary>
      public bool IsDiving { get; protected set; }

      /// <summary>
      /// Current user movement input.
      /// </summary>
      private Vector2 movement;

      // Jumping state
      private float diveTime;

      private float angle = 0.0f;

      //Collision stuff
      private Rectangle localBounds;
      /// <summary>
      /// Gets a rectangle which bounds this player in world space.
      /// </summary>
      public override Rectangle BoundingRectangle
      {
         get
         {
            Matrix toWorldSpace = Matrix.CreateTranslation(new Vector3(-sprite.Origin, 0.0f)) *
                                 Matrix.CreateScale(1) *
                                 Matrix.CreateRotationZ(this.angle) *
                                 Matrix.CreateTranslation(new Vector3(this.Position, 0.0f));

            return CalculateTransformedBoundingBox(new Rectangle(0, 0, 64, 64), toWorldSpace);
         }
      }
      public Vector2 RightHandPosition
      {
         get
         {
            Vector2 HandOffset = new Vector2(20, -30);
            Matrix toWorldSpace = Matrix.CreateTranslation(new Vector3(HandOffset, 0.0f)) * 
                                  Matrix.CreateRotationZ(this.angle) *
                                  Matrix.CreateTranslation(new Vector3(this.Position, 0.0f));
            return CalculateTranslatedPosition(toWorldSpace);
         }
      }

      public PlayerIndex Index { get; protected set; }


      public Player(Court court, Vector2 position, Character character, PlayerIndex index, Rectangle PlayArea, float InitialDirection = 0.0f)
         :base(position, "Player")
      {
         this.Index = index;
         this.character = character;
         this.Court = court;
         this.angle = MathHelper.ToRadians(InitialDirection) + MathHelper.ToRadians(90);
         this.PlayArea = PlayArea;

         #region Debug stuff
         if (Index ==  PlayerIndex.One)
         {
            // This is all just a debuggey thing to use the 
            // right thumbstick for player 2
            // If two controllers are being used, then this 
            // isn't necessary
            this.MovementStickLeft = true;
            this.DiveButton = Buttons.A;
         }
         else
         {
            this.DiveButton = Buttons.B;
            this.MovementStickLeft = false;
         }
         // We also hard code the player index so that 
         // we can use one controller for testing.
         Index = PlayerIndex.One;
         #endregion
      }

      /// <summary>
      /// Loads the player sprite sheet and sounds.
      /// </summary>
      public override void LoadContent(ContentManager content)
      {
         base.LoadContent(content);
         String characterString = "/" + character.ToString();
         idleAnimation = new Animation(content.Load<Texture2D>("Sprites" + characterString + "/Idle"), 0.1f, true);
         moveAnimation = new Animation(content.Load<Texture2D>("Sprites" + characterString + "/move"), 0.1f, true);
         diveAnimation = new Animation(content.Load<Texture2D>("Sprites" + characterString + "/move"), 0.1f, true);

         // Calculate bounds within texture size.            
         int width = (int)(idleAnimation.FrameWidth);
         int left = 0;
         int height = (int)(idleAnimation.FrameWidth);
         int top = 0;

         localBounds = new Rectangle(left, top, width, height);
         BoundingOverlay = new RectangleOverlay(Color.Red);
         BoundingOverlay.LoadContent(Court.Content.ServiceProvider);
         PlayAreaOverlay = new RectangleOverlay(Color.Orange);
         PlayAreaOverlay.LoadContent(Court.Content.ServiceProvider);
         Reset(Position);
      }

      /// <summary>
      /// Resets the player to life.
      /// </summary>
      /// <param name="position">The position to come to life at.</param>
      public void Reset(Vector2 position)
      {
         Position = position;
         Velocity = Vector2.Zero;
         IsDiving = false;
         sprite.PlayAnimation(idleAnimation);
      }

      #region Update
      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
         GetInput(GamePad.GetState(Index), gameTime);

         ApplyPhysics(gameTime);

         if (Math.Abs(Velocity.Length()) - 0.02f > 0)
         {
            sprite.PlayAnimation(moveAnimation);
         }
         else
         {
            sprite.PlayAnimation(idleAnimation);
         }
      }

      Vector2 movementStickDirection;
      private void GetInput(GamePadState gamePadState, GameTime gameTime)
      {
         if (MovementStickLeft)
         {
            movementStickDirection = gamePadState.ThumbSticks.Left;
         }
         else
         {
            movementStickDirection = gamePadState.ThumbSticks.Right;
         }
         if (this.HasDisc)
         {
            movement = Vector2.Zero;

            Vector2 direction = movementStickDirection * MoveStickScale;// Ignore small movements to prevent running in place.
            if (Math.Abs(direction.X) < 0.25f && Math.Abs(direction.Y) < 0.25f)
            {
            }
            else
            {
               direction.Y *= -1;
               angle = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.ToRadians(90);
            }

            if (gamePadState.IsButtonDown(DiveButton))
            {
               //Throw disc
               _HadDisc = true;
               Court.ThrowDisc(this.angle);
            }
         }
         else
         {
            if (gamePadState.IsButtonDown(DiveButton))
            {
               if (diveTime < MaxDiveTime)
               {
                  diveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                  IsDiving = true;
               }
               else
               {
                  IsDiving = false;
               }
            }
            else
            {
               diveTime = 0.0f;
               IsDiving = false;

               movement = movementStickDirection * MoveStickScale;

               // Ignore small movements to prevent running in place.
               if (Math.Abs(movement.X) < 0.25f && Math.Abs(movement.Y) < 0.25f)
               {
                  movement = Vector2.Zero;
               }
               else
               {
                  movement.Y *= -1;

                  angle = (float)Math.Atan2(movement.Y, movement.X) + MathHelper.ToRadians(90);
               }
            }
         }
      }

      private void ApplyPhysics(GameTime gameTime)
      {
         float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

         Vector2 previousPosition = Position;
         Vector2 velocity = Velocity;
         if (IsDiving)
         {
            velocity += movement * DiveAcceleration * elapsed;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxDiveSpeed, MaxDiveSpeed);

            // Prevent the player from running faster than his top speed.            
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxDiveSpeed, MaxDiveSpeed);
         }
         else
         {
            velocity += movement * MoveAcceleration * elapsed;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Prevent the player from running faster than his top speed.            
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxMoveSpeed, MaxMoveSpeed);
         }
         velocity *= GroundDragFactor;


         Vector2 position = Position + velocity * elapsed;
         float newX = (float)Math.Round(position.X);
         float newY = (float)Math.Round(position.Y);

         HandleCollisions();

         // Here we check bounds, to make sure we haven't left
         // the play area
         if (BoundingRectangle.X + BoundingRectangle.Width > PlayArea.X + PlayArea.Width ||
            BoundingRectangle.X < PlayArea.X)
         {
            position.X = previousPosition.X;
            velocity.X = 0;
         }

         // Here we check bounds, to make sure we haven't left
         // the play area
         if (BoundingRectangle.Y + BoundingRectangle.Height > PlayArea.Y + PlayArea.Height ||
            BoundingRectangle.Y < PlayArea.Y)
         {
            position.Y = previousPosition.Y;
            velocity.Y = 0;
         }

         this.Position = position;

         // If the collision stopped us from moving or the new 
         // position is neglegably small, reset the velocity to zero.
         if (Position.X == previousPosition.X)
            velocity.X = 0;

         if (Position.Y == previousPosition.Y)
            velocity.Y = 0;


         this.Velocity = velocity;
      }

      protected void HandleCollisions()
      {
         if (Court.DoesCollideWithDisc(this))
         {
            if (_HadDisc)
            {
               BoundingOverlay.Colori = Color.Yellow;
               HasDisc = false;
            }
            else
            {
               BoundingOverlay.Colori = Color.Green;
               HasDisc = true;
            }
         }
         else
         {
            if (_HadDisc)
            {
               BoundingOverlay.Colori = Color.Yellow;
               _HadDisc = false;
            }
            else
            {
               BoundingOverlay.Colori = Color.Red;
               HasDisc = false;
            }
         }
      }

      private Vector2 CalculateTranslatedPosition(Matrix worldSpace)
      {
         Vector2 center = Vector2.Zero;
         Vector2.Transform(ref center, ref worldSpace, out center);
         return center;
      }
      // This will calculate the bounding box taking into account both scaling and rotation.
      private Rectangle CalculateTransformedBoundingBox(Rectangle local, Matrix toWorldSpace)
      {
         // Get all four corners in local space
         Vector2 leftTop = new Vector2(local.Left, local.Top);
         Vector2 rightTop = new Vector2(local.Right, local.Top);
         Vector2 leftBottom = new Vector2(local.Left, local.Bottom);
         Vector2 rightBottom = new Vector2(local.Right, local.Bottom);

         // Transform all four corners into work space
         Vector2.Transform(ref leftTop, ref toWorldSpace,
                          out leftTop);
         Vector2.Transform(ref rightTop, ref toWorldSpace,
                          out rightTop);
         Vector2.Transform(ref leftBottom, ref toWorldSpace,
                          out leftBottom);
         Vector2.Transform(ref rightBottom, ref toWorldSpace,
                          out rightBottom);

         // Find the minimum and maximum extents of the
         // rectangle in world space
         Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                  Vector2.Min(leftBottom, rightBottom));
         Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                  Vector2.Max(leftBottom, rightBottom));

         // Return that as a rectangle
         return new Rectangle((int)min.X, (int)min.Y,
                             (int)(max.X - min.X), (int)(max.Y - min.Y));
      }
      #endregion

      #region Draw

      /// <summary>
      /// Draws the animated player.
      /// </summary>
      public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         base.Draw(gameTime, spriteBatch);
         PlayAreaOverlay.Draw(gameTime, spriteBatch, PlayArea);
         BoundingOverlay.Draw(gameTime, spriteBatch, BoundingRectangle);
         // Draw that sprite.
         sprite.Draw(gameTime, spriteBatch, Position, angle);
         DrawString(spriteBatch, "P: " + Position.ToString());
         DrawString(spriteBatch, "A: " + angle.ToString());
         DrawString(spriteBatch, "JP: " + movementStickDirection.ToString());
      }
      #endregion

      #region Helpers
      #endregion
   }
}
