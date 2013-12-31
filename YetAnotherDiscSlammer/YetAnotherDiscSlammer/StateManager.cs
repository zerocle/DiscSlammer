using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using YetAnotherDiscSlammer.Common;
using YetAnotherDiscSlammer.Menu;
using YetAnotherDiscSlammer.Entities;

namespace YetAnotherDiscSlammer
{

   public enum GameStates
   {
      MainMenu,
      Options,
      Playing,
      Paused,
      PlayerChoice,
   }
   public class StateManager
   {
      protected DiscSlammerGame _game;
      protected ContentManager _Content;
      protected Court _Court;
      protected PlayerChoiceMenu _PlayerChoiceMenu;
      protected GameStates _CurrentState = GameStates.MainMenu;
      #region Menus
      protected MenuBase _mainMenu;
      protected MenuBase _numOfPlayers;
      protected MenuBase _optionsMenu;
      #endregion

      public StateManager(DiscSlammerGame game)
      {
         this._game = game;
      }

      public Boolean Initialize()
      {
         List<MenuItem> menuItems = new List<MenuItem>();
         menuItems.Add(new MenuItem("Start Game", new Action(ShowPlayerChoice)));
         menuItems.Add( new MenuItem("Options", new Action(ShowOptions)));
         menuItems.Add(new MenuItem("Quit Game", new Action(ExitGameMethod)));
         _mainMenu = new MenuBase();
         _mainMenu.Initialize(menuItems);
         return true;
      }

      public void LoadContent(IServiceProvider serviceProvider)
      {
         _Content = new ContentManager(serviceProvider, "Content");

         _mainMenu.LoadContent(_Content);
      }

      public void Update(GameTime gameTime)
      {
         switch(_CurrentState)
         {
            case GameStates.MainMenu:
               _mainMenu.Update(gameTime);
               break;
            case GameStates.Playing:
               _Court.Update(gameTime);
               break;
            case GameStates.Options:
               _optionsMenu.Update(gameTime);
               break;
            case GameStates.PlayerChoice:
               _PlayerChoiceMenu.Update(gameTime);
               break;
         }
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {

         switch (_CurrentState)
         {
            case GameStates.MainMenu:
               _mainMenu.Draw(gameTime, spriteBatch);
               break;
            case GameStates.Playing:
               _Court.Draw(gameTime, spriteBatch);
               break;
            case GameStates.Options:
               _optionsMenu.Draw(gameTime, spriteBatch);
               break;
            case GameStates.PlayerChoice:
               _PlayerChoiceMenu.Draw(gameTime, spriteBatch);
               break;
         }
      }

      protected GameStates OptionsPreviousState = GameStates.MainMenu;
      protected void ShowOptions()
      {
         _optionsMenu = new MenuBase();
         List<MenuItem> optionsItems = new List<MenuItem>();
         optionsItems.Add(new MenuItem("Do Something", new Action(holder)));
         optionsItems.Add(new MenuItem("Dont do something", new Action(holder), false));
         optionsItems.Add(new MenuItem("Back", new Action(LeaveOptions)));
         _optionsMenu.Initialize(optionsItems);
         _optionsMenu.LoadContent(_Content);
         OptionsPreviousState = _CurrentState;
         _CurrentState = GameStates.Options;
      }
      protected void LeaveOptions()
      {
         _CurrentState = OptionsPreviousState;
      }
      protected void holder(){}
      protected void ShowPlayerChoice()
      {
         _PlayerChoiceMenu = new PlayerChoiceMenu();
         if (_PlayerChoiceMenu.Init(_Content))
         {
            _PlayerChoiceMenu.LoadContent(_Content);
            _CurrentState = GameStates.PlayerChoice;
         }
      }

      protected void StartGameMethod()
      {
         _Court = new Court(_Content);
         
         _CurrentState = GameStates.Playing;
      }
      protected void ExitGameMethod()
      {
         _game.Exit();
      }
   }
}
