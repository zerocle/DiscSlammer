﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using YetAnotherDiscSlammer.Common;
using YetAnotherDiscSlammer.Helpers;

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

         InputState inputState = GetInput(GamePad.GetState(Index), gameTime);
         PhysicsState physicsState = CalculatePhysics(gameTime, inputState);
         ApplyPhysics(gameTime, physicsState);

         //Check to see if the animation should be the idle or moving
         if (Math.Abs(Velocity.Length()) - 0.02f > 0)
         {
            sprite.PlayAnimation(moveAnimation);
         }
         else
         {
            sprite.PlayAnimation(idleAnimation);
         }
      }

      GamePadState _LastGameState;
      private InputState GetInput(GamePadState gamePadState, GameTime gameTime)
      {
         InputState state = new InputState();
         if (MovementStickLeft)
         {
            state.MovementStickDirection = gamePadState.ThumbSticks.Left * MoveStickScale;
         }
         else
         {
            state.MovementStickDirection = gamePadState.ThumbSticks.Right * MoveStickScale;
         }

         state.MovementStickDirection.Y *= -1;

         state.IsDiveButtonDown = gamePadState.IsButtonDown(DiveButton);
         state.WasDiveButtonDown = _LastGameState.IsButtonDown(DiveButton);

         _LastGameState = gamePadState;

         return state;
         
      }
      private PhysicsState CalculatePhysics(GameTime gameTime, InputState inputState)
      {
         float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
         PhysicsState state = new PhysicsState();
         state.Position = Position;
         if (this.HasDisc)
         {
            // While the player has the disc, we don't want them moving
            state.Movement = Vector2.Zero;
            state.Velocity = Vector2.Zero;
            // but they can still turn
            state.Angle = GetAngleFromVector(inputState.MovementStickDirection);

            // we make sure the last state was up, so that the player doesn't throw 
            // as soon as they catch it diving.
            if (!inputState.WasDiveButtonDown && inputState.IsDiveButtonDown)
            {
               //Throw disc
               _HadDisc = true;
               Court.ThrowDisc(this.angle);
            }
         }
         else
         {
            // If the user is pressing the dive button and doesnt
            // have the disc
            if (inputState.IsDiveButtonDown)
            {
               // and they still haven't dove past the max
               // amount of time
               if (diveTime < MaxDiveTime)
               {
                  diveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                  IsDiving = true;
                  state.Movement = inputState.MovementStickDirection;
               }
               else
               {
                  IsDiving = false;
               }
            }
            else
            {
               // set the diving state if the button isn't down
               diveTime = 0.0f;
               IsDiving = false;

               Vector2 tempMovement = inputState.MovementStickDirection;

               // Ignore small movements to prevent running in place.
               if (Math.Abs(tempMovement.X) < 0.25f && Math.Abs(tempMovement.Y) < 0.25f)
               {
                  tempMovement = Vector2.Zero;
               }
               else
               {
                  angle = GetAngleFromVector(tempMovement);
               }
               state.Movement = tempMovement;
            }
            Vector2 calculatedVelocity = Vector2.Zero;

            // Apply our physics to determine our new position
            if (IsDiving)
            {
               calculatedVelocity = Velocity + state.Movement * DiveAcceleration * elapsed;

               // Prevent the player from diving faster than his top speed.            
               calculatedVelocity.X = MathHelper.Clamp(calculatedVelocity.X, -MaxDiveSpeed, MaxDiveSpeed);

               // Prevent the player from diving faster than his top speed.            
               calculatedVelocity.Y = MathHelper.Clamp(calculatedVelocity.Y, -MaxDiveSpeed, MaxDiveSpeed);
            }
            else
            {
               calculatedVelocity = Velocity + state.Movement * MoveAcceleration * elapsed;

               // Prevent the player from running faster than his top speed.            
               calculatedVelocity.X = MathHelper.Clamp(calculatedVelocity.X, -MaxMoveSpeed, MaxMoveSpeed);

               // Prevent the player from running faster than his top speed.            
               calculatedVelocity.Y = MathHelper.Clamp(calculatedVelocity.Y, -MaxMoveSpeed, MaxMoveSpeed);
            }

            // Add friction into the calculations
            calculatedVelocity *= GroundDragFactor;

            state.Velocity = calculatedVelocity;
         }
         return state;
      }
      private void ApplyPhysics(GameTime gameTime, PhysicsState physicsState)
      {
         float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

         Vector2 previousPosition = physicsState.Position;
         float oldAngle = physicsState.Angle ;

         // calculate our position after we move
         physicsState.NextPosition = Position + physicsState.Velocity * elapsed;

         // Check for collisions
         HandleCollisions();

         // Make sure our new position is within the play area.
         CheckPlayArea(physicsState);

         // Now we want to set our actual position
         this.Position = physicsState.NextPosition;

         // If the collision stopped us from moving or the new 
         // position is neglegably small, reset the velocity to zero.
         if (physicsState.NextPosition.X == physicsState.Position.X)
            physicsState.Velocity.X = 0;

         if (physicsState.NextPosition.Y == physicsState.Position.Y)
            physicsState.Velocity.Y = 0;

         this.Velocity = physicsState.Velocity;
      }

      protected void CheckPlayArea(PhysicsState state)
      {

         // Here we check bounds, to make sure we haven't left
         // the play area
         if (state.NextPosition.X > PlayArea.X + PlayArea.Width ||
            state.NextPosition.X < PlayArea.X)
         {
            state.NextPosition.X = state.Position.X;
            state.Velocity.X = 0;
         }

         // Here we check bounds, to make sure we haven't left
         // the play area
         if (state.NextPosition.Y > PlayArea.Y + PlayArea.Height ||
            state.NextPosition.Y < PlayArea.Y)
         {
            state.NextPosition.Y = state.Position.Y;
            state.Velocity.Y = 0;
         }
      }
      protected void HandleCollisions()
      {
         if (Court.CollidesWith(this, "Disc"))
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
         //DrawString(spriteBatch, "JP: " + movementStickDirection.ToString());
      }
      #endregion

      #region Helpers
      private float GetAngleFromVector(Vector2 coordinate)
      {
         float tempAngle = 0.0f;
         Vector2 direction = coordinate;// Ignore small movements to prevent running in place.
         if (Math.Abs(direction.X) > 0.25f || Math.Abs(direction.Y) > 0.25f)
         {
            tempAngle = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.ToRadians(90);
         }
         return tempAngle;
      }
      #endregion
   }
}
