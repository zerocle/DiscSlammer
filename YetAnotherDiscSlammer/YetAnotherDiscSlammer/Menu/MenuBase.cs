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
   class MenuBase
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

      public void LoadContent(ContentManager content, String Font = "Font/Standard")
      {
         this.MenuFont = content.Load<SpriteFont>(Font);
      }

      protected KeyboardState previousState;
      public void Update(GameTime gameTime)
      {
         KeyboardState state = Keyboard.GetState();
         // Ignore previous state until it has a value
         if (previousState != null)
         {
            if (state.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up))
            {
               SelectedIndex--;
            }
            else if (state.IsKeyDown(Keys.Down) && previousState.IsKeyUp(Keys.Down))
            {
               SelectedIndex++;
            }
            else if ((state.IsKeyDown(Keys.Enter) && previousState.IsKeyUp(Keys.Enter)) || (state.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space)))
            {
               MenuOptions[SelectedIndex].Callback();
            }
         }
         previousState = state;

      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Vector2 position = Vector2.Zero;
         for (int a = 0; a < MenuOptions.Count; a++)
         {
            Color drawColor = Color.White;
            if (a == SelectedIndex)
            {
               drawColor = Color.Black;
            }

            spriteBatch.DrawString(MenuFont, MenuOptions[a].DisplayValue, position, drawColor);
            position.Y += MenuFont.LineSpacing + 5;
         }
      }
   }
}
