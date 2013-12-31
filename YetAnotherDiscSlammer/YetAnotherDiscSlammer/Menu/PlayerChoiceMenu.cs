using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Menu
{
   public class PlayerChoiceMenu
   {
      protected ContentManager _Content;
      protected List<Character> Characters;
      protected DeviceChooser PlayerDeviceChooser;
      protected ControlDevice PlayerOneDevice;
      protected ControlDevice PlayerTwoDevice;
      protected int PlayerOneChoiceIndex
      {
         get
         {
            return _PlayerOneChoiceIndex;
         }
         set
         {
            if (value == _PlayerOneChoiceIndex)
               return;
            _PlayerOneChoiceIndex = value;
            PlayerOnePlayer.SetAnimation(Characters[value].RightWalk);
         }
      }int _PlayerOneChoiceIndex = -1;
      protected int PlayerTwoChoiceIndex
      {
         get
         {
            return _PlayerTwoChoiceIndex;
         }
         set
         {
            if (value == _PlayerTwoChoiceIndex)
               return;
            _PlayerTwoChoiceIndex = value;
            PlayerTwoPlayer.SetAnimation(Characters[value].LeftWalk);
         }
      }int _PlayerTwoChoiceIndex = -1;
      protected Texture2D PlayerOneChooserTexture;
      protected Texture2D PlayerTwoChooserTexture;
      protected ChoiceShifter PlayerOneChoiceShifter;
      protected ChoiceShifter PlayerTwoChoiceShifter;
      #region Debug
      protected AnimationPlayer PlayerOnePlayer;
      protected AnimationPlayer PlayerTwoPlayer;
      #endregion

      public PlayerChoiceMenu()
      {

      }

      public bool Init(ContentManager Content)
      {
         try
         {
            Characters = new List<Character>();
            Character Guile = new Character();
            if (Guile.Initialize("Guile"))
            {
               Characters.Add(Guile);
            }
            //Character Kid = new Character();
            //if (Kid.Initialize("Kid"))
            //{
            //   Characters.Add(Kid);
            //}
            Character Serge = new Character();
            if (Serge.Initialize("Serge"))
            {
               Characters.Add(Serge);
            }
            Character Viper = new Character();
            if (Viper.Initialize("Viper"))
            {
               Characters.Add(Viper);
            }
            PlayerDeviceChooser = new DeviceChooser();
            if (PlayerDeviceChooser.Initialize())
            {
               PlayerOneDevice = Settings.Instance.PlayerOneController;
               PlayerTwoDevice = Settings.Instance.PlayerTwoController;
            }
         }
         catch (Exception err)
         {
            System.Console.WriteLine(err.Message);
         }
         return true;
      }

      public void LoadContent(ContentManager content)
      {
         _Content = content;
         foreach (Character character in Characters)
         {
            character.LoadContent(content);
         }
         PlayerDeviceChooser.LoadContent(content);
         PlayerOneChooserTexture = content.Load<Texture2D>("Sprites/UI/PlayerOneChooser");
         PlayerTwoChooserTexture = content.Load<Texture2D>("Sprites/UI/PlayerTwoChooser");
         PlayerOnePlayer = new AnimationPlayer();
         PlayerTwoPlayer = new AnimationPlayer();
         PlayerOneChoiceIndex = 0;
         PlayerTwoChoiceIndex = Characters.Count - 1;
      }

      public void Update(GameTime gameTime)
      {
         if (PlayerOneDevice == ControlDevice.Unselected)
         {
            PlayerOneDevice = PlayerDeviceChooser.GetDevicePressingStart(gameTime);
         }
         else if (PlayerTwoDevice == ControlDevice.Unselected)
         {
            PlayerTwoDevice = PlayerDeviceChooser.GetDevicePressingStart(gameTime, PlayerOneDevice);
         }
         else
         {
            if (PlayerOneChoiceShifter == null)
            {
               PlayerOneChoiceShifter = GetShifter(PlayerOneDevice);
            }
            if (PlayerTwoChoiceShifter == null)
            {
               PlayerTwoChoiceShifter = GetShifter(PlayerTwoDevice);
            }
            //choose characters
            PlayerOneChoiceIndex = GetNewIndex(gameTime, PlayerOneChoiceShifter, PlayerOneChoiceIndex);
            PlayerTwoChoiceIndex = GetNewIndex(gameTime, PlayerTwoChoiceShifter, PlayerTwoChoiceIndex);
         }
      }
      public int GetNewIndex(GameTime gameTime, ChoiceShifter shifter, int CurrentIndex)
      {
         int temp = CurrentIndex + shifter.GetShift(gameTime);
         if (temp >= Characters.Count)
            temp = 0;
         else if (temp < 0)
            temp = Characters.Count - 1;
         return temp;
      }
      public ChoiceShifter GetShifter(ControlDevice device)
      {
         ChoiceShifter shifter;
         if (device == ControlDevice.Keyboard)
         {
            shifter = new ChoiceShifterKeyboard(device);
         }
         else if (device == ControlDevice.AI)
         {
            shifter = new ChoiceShifterAI(device);
         }
         else
         {
            shifter = new ChoiceShifterGamepad(device);
         }
         return shifter;
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         if (PlayerOneDevice == ControlDevice.Unselected)
         {
            PlayerDeviceChooser.OutputStartPrompt(gameTime, spriteBatch, "Player ONE");
         }
         else if (PlayerTwoDevice == ControlDevice.Unselected)
         {
            PlayerDeviceChooser.OutputStartPrompt(gameTime, spriteBatch, "Player TWO");
         }
         else
         {
            int texWidth = 64;
            //choose characters
            int x = Settings.Instance.Width / 2 - Characters.Count * texWidth / 2;
            Vector2 offset = new Vector2(x, 100);

            // Draw our thumbnails
            foreach (Character c in Characters)
            {
               spriteBatch.Draw(c.Thumbnail, new Rectangle((int)offset.X, (int)offset.Y, texWidth, texWidth), null, Color.White);
               offset.X += c.Thumbnail.Width;
            }

            // Figure out the x location of the selection chevrons
            float p1Character = (float)Characters.Count / 2 - Characters.Count + PlayerOneChoiceIndex;
            int p1x = (int)(Settings.Instance.Width / 2 + (p1Character * texWidth) + texWidth / 2);

            float p2Character = (float)Characters.Count / 2 - Characters.Count + PlayerTwoChoiceIndex;
            int p2x = (int)(Settings.Instance.Width / 2 + (p2Character * texWidth) + texWidth / 2);

            // Draw the selection chevrons
            spriteBatch.Draw(PlayerOneChooserTexture, new Vector2(p1x, 220), null, Color.White, 0.0f,
               new Vector2(PlayerOneChooserTexture.Width / 2, PlayerOneChooserTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(PlayerTwoChooserTexture, new Vector2(p2x, 220), null, Color.White, 0.0f,
               new Vector2(PlayerTwoChooserTexture.Width / 2, PlayerTwoChooserTexture.Height / 2), 1.0f, SpriteEffects.None, 0);

            // Draw the selected player animations
            PlayerOnePlayer.Draw(gameTime, spriteBatch, new Vector2(200, 200), 0.0f);
            PlayerTwoPlayer.Draw(gameTime, spriteBatch, new Vector2(Settings.Instance.Width - 200, 200), 0.0f);
         }
      }

   }
}
