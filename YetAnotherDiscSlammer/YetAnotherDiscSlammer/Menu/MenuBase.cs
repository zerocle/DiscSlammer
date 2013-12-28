using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherDiscSlammer.Menu
{
   public class MenuBase
   {
      protected List<MenuItem> MenuOptions;

      protected SpriteFont MenuFont;

      protected int SelectedIndex
      {
         get
         {
            return _SelectedIndex;
         }
         set
         {
            if (value < 0)
            {
               _SelectedIndex = 0;
            }
            else if (value >= MenuOptions.Count)
            {
               _SelectedIndex = MenuOptions.Count - 1;
            }
            else
            {
               _SelectedIndex = value;
            }
         }
      }protected int _SelectedIndex = 0;

      public MenuBase()
      {

      }

      public Boolean Initialize(List<MenuItem> MenuItems)
      {
         this.MenuOptions = MenuItems;
         return true;
      }

      public void LoadContent(ContentManager content, String Font = "Font/LargeFont")
      {
         this.MenuFont = content.Load<SpriteFont>(Font);
      }

      protected KeyboardState previousState;
      protected GamePadState previousControllerState;
      protected float timeBetweenThumbstick = 0.5f;
      protected float elapsedTimeSinceMovement = 0.0f;
      public void Update(GameTime gameTime)
      {
         KeyboardState state = Keyboard.GetState();
         GamePadState controllerState = GamePad.GetState(PlayerIndex.One);

         elapsedTimeSinceMovement += (float)gameTime.ElapsedGameTime.TotalSeconds;

         if (previousState != null)
         {
            if ((state.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up)) ||
               (controllerState.ThumbSticks.Left.Y > 0.5f && (elapsedTimeSinceMovement > timeBetweenThumbstick || previousControllerState.ThumbSticks.Left.Y < 0.5f)))
            {
               elapsedTimeSinceMovement = 0.0f;
               SelectedIndex--;
            }
            else if ((state.IsKeyDown(Keys.Down) && previousState.IsKeyUp(Keys.Down)) ||
               (controllerState.ThumbSticks.Left.Y < -0.5f && (elapsedTimeSinceMovement > timeBetweenThumbstick || previousControllerState.ThumbSticks.Left.Y > -0.5f)))
            {
               elapsedTimeSinceMovement = 0.0f;
               SelectedIndex++;
            }
            else if ((state.IsKeyDown(Keys.Enter) && previousState.IsKeyUp(Keys.Enter)) || (state.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space)) ||
                     (controllerState.IsButtonDown(Buttons.A) && previousControllerState.IsButtonUp(Buttons.A)))
            {
               MenuOptions[SelectedIndex].Callback();
            }
         }
         previousControllerState = controllerState;
         previousState = state;

      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Vector2 position = Vector2.Zero;
         for (int a = 0; a < MenuOptions.Count; a++)
         {
            Vector2 offset = Vector2.Zero;
            Color drawColor = Color.White;
            if (a == SelectedIndex)
            {
               if (MenuOptions[a].IsEnabled)
               {
                  offset = new Vector2(20, 0);
                  drawColor = Color.Black;
               }
               else
               {
                  drawColor = Color.Gray;
               }
            }
            else
            {
               if (!MenuOptions[a].IsEnabled)
               {
                  drawColor = Color.DarkGray;
               }
            }
            spriteBatch.DrawString(MenuFont, MenuOptions[a].DisplayValue, position + offset, drawColor);
            position.Y += MenuFont.LineSpacing + 5;
         }
      }
   }
}
