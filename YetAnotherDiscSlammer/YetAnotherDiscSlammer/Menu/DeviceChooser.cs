using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   public class DeviceChooser
   {
      #region Last State Holders
      protected GamePadState LastPlayerOneState;
      protected GamePadState LastPlayerTwoState;
      protected GamePadState LastPlayerThreeState;
      protected GamePadState LastPlayerFourState;
      protected KeyboardState LastKeyboardState;
      #endregion

      protected SpriteFont font;

      public DeviceChooser()
      {

      }

      public bool Initialize()
      {
         // Initialize our states so we don't have any menu holdover.
         this.LastKeyboardState = Keyboard.GetState();
         this.LastPlayerOneState = GamePad.GetState(PlayerIndex.One);
         this.LastPlayerTwoState = GamePad.GetState(PlayerIndex.Two);
         this.LastPlayerThreeState = GamePad.GetState(PlayerIndex.Three);
         this.LastPlayerFourState = GamePad.GetState(PlayerIndex.Four);

         return true;
      }

      public void LoadContent(ContentManager content)
      {
         font = content.Load<SpriteFont>("Font//Standard");
      }
      #region Chose your device.
      /// <summary>
      /// This will test a controller to see if the "select" button is down. Start or A count as "Select"
      /// </summary>
      /// <param name="currentState">The controller's current state</param>
      /// <param name="lastState">The controller's last state, to prevent holding causing undesired reactions</param>
      /// <returns>Whether or not the select button is down</returns>
      protected Boolean GetSelectDown(GamePadState currentState, GamePadState lastState)
      {
         if ((currentState.IsButtonDown(Buttons.A) && lastState.IsButtonUp(Buttons.A)) ||
              (currentState.IsButtonDown(Buttons.Start) && lastState.IsButtonUp(Buttons.Start)))
         {
            return true;
         }
         return false;
      }
      public ControlDevice GetDevicePressingStart(GameTime gameTime, ControlDevice InvalidController = ControlDevice.None)
      {
         ControlDevice selectedDevice = ControlDevice.Unselected;
         KeyboardState KeyboardState = Keyboard.GetState();
         GamePadState PlayerOneState = GamePad.GetState(PlayerIndex.One);
         GamePadState PlayerTwoState = GamePad.GetState(PlayerIndex.Two);
         GamePadState PlayerThreeState = GamePad.GetState(PlayerIndex.Three);
         GamePadState PlayerFourState = GamePad.GetState(PlayerIndex.Four);

         if (InvalidController != ControlDevice.ControllerOne && GetSelectDown(PlayerOneState, LastPlayerOneState))
         {
            selectedDevice = ControlDevice.ControllerOne;
         }
         else if (InvalidController != ControlDevice.ControllerTwo && GetSelectDown(PlayerTwoState, LastPlayerTwoState))
         {
            selectedDevice = ControlDevice.ControllerTwo;
         }
         else if (InvalidController != ControlDevice.ControllerThree && GetSelectDown(PlayerThreeState, LastPlayerThreeState))
         {
            selectedDevice = ControlDevice.ControllerThree;
         }
         else if (InvalidController != ControlDevice.ControllerFour && GetSelectDown(PlayerFourState, LastPlayerFourState))
         {
            selectedDevice = ControlDevice.ControllerFour;
         }
         else if (((KeyboardState.IsKeyDown(Keys.Enter) && LastKeyboardState.IsKeyUp(Keys.Enter))  ||
                   (KeyboardState.IsKeyDown(Keys.Space) && LastKeyboardState.IsKeyUp(Keys.Space))) && 
                    InvalidController != ControlDevice.Keyboard)
         {
            selectedDevice = ControlDevice.Keyboard;
         }
         else if (KeyboardState.IsKeyDown(Keys.A))
         {
            selectedDevice = ControlDevice.AI;
         }

         // Save the state for later.
         LastKeyboardState = KeyboardState;
         LastPlayerOneState = PlayerOneState;
         LastPlayerTwoState = PlayerTwoState;
         LastPlayerThreeState = PlayerThreeState;
         LastPlayerFourState = PlayerFourState;

         return selectedDevice;
      }

      public void OutputStartPrompt(GameTime gameTime, SpriteBatch spriteBatch, String PromptName)
      {
         String Message = PromptName + " press START on your controller!";
         Vector2 size = font.MeasureString(Message);
         spriteBatch.DrawString(font, Message, new Vector2(Settings.Instance.Width / 2 - size.X / 2, Settings.Instance.Height / 2 - size.Y), Color.White);
      }
      #endregion


   }
}
