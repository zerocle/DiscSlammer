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
   public class Court
   {

      #region Debug stuff
      #endregion

      public Texture2D CourtBackground { get; protected set; }

      protected List<Entity> _CourtEntities;
      public Player[] Players { get; protected set; }
      public Disc GameDisc { get; protected set; }

      public int[] Scores { get; protected set; }

      public ScoreZone[] ScoreZones { get; protected set; }
      public Wall[] Walls { get; protected set; }

      public ScoreBoard scoreBoard { get; protected set; }

      public ContentManager Content { get; protected set; }

      #region Initialization
      public Court(ContentManager Content)
      {
         this.Content = Content;
         int numPlayers = 2;

         Players = new Player[numPlayers];
         Scores = new int[numPlayers];
         ScoreZones = new ScoreZone[numPlayers];
         Walls = new Wall[numPlayers];
         GameDisc = new Disc(this);
         _CourtEntities = new List<Entity>();
         scoreBoard = new ScoreBoard(new Vector2(Settings.Instance.Width / 2 - 128, 0), "BANSHEE", "Zerocle!");
         ConfigureCourt();

         LoadContent();
      }

      protected void ConfigureCourt()
      {
         int PlayerWidth = 64;

         int boundsWidth = Settings.Instance.Width / 2 - 75 - PlayerWidth;
         int boundsHeight = Settings.Instance.Height - 100 - PlayerWidth;

         // set up our players
         Players[0] = new Player(this, new Vector2(100, Settings.Instance.Height / 2),
            Character.Scorpion, PlayerControlDevice.Controller,
            new Rectangle(50 + PlayerWidth / 2, 50 + PlayerWidth / 2, boundsWidth, boundsHeight));

         Players[1] = new Player(this, 
            new Vector2(Settings.Instance.Width - 100, Settings.Instance.Height / 2),
            Character.Scorpion, PlayerControlDevice.AI,
            new Rectangle(Settings.Instance.Width - 50 - boundsWidth - PlayerWidth / 2, 
                          50 + PlayerWidth / 2, boundsWidth, boundsHeight), 180.0f);
         Players[0].HasDisc = true;

         // reset the bounds to the whole width/height
         boundsHeight = boundsHeight + PlayerWidth;
         boundsWidth = boundsWidth + PlayerWidth;
         ScoreZones[0] = new ScoreZone(this, new Vector2(25, 50), 
                                             new Vector2(25, boundsHeight),
                                             PlayerIndex.One);
         ScoreZones[1] = new ScoreZone(this, new Vector2(Settings.Instance.Width - 50, 50),
                                             new Vector2(25, boundsHeight),
                                             PlayerIndex.Two);

         Walls[0] = new Wall(this, new Vector2(25, 40), 
                                   new Vector2(Settings.Instance.Width - 50, 10));
         Walls[1] = new Wall(this, new Vector2(25, Settings.Instance.Height - 50),
                                   new Vector2(Settings.Instance.Width - 50, 10));
         _CourtEntities.Add(Players[0]);
         _CourtEntities.Add(Players[1]);
         _CourtEntities.Add(ScoreZones[0]);
         _CourtEntities.Add(ScoreZones[1]);
         _CourtEntities.Add(Walls[0]);
         _CourtEntities.Add(Walls[1]);
         _CourtEntities.Add(GameDisc);
      }

      public void LoadContent()
      {
         foreach (Player p in Players)
         {
            p.LoadContent(Content);
         }
         foreach (ScoreZone sz in ScoreZones)
         {
            sz.LoadContent(Content);
         }
         foreach (Wall w in Walls)
         {
            w.LoadContent(Content);
         }
         scoreBoard.LoadContent(Content);
      }
      #endregion


      #region Update
      /// <summary>
      /// Updates all objects in the world, performs collision between them,
      /// and handles the scoring.
      /// </summary>
      public void Update(GameTime gameTime)
      {
         if (GameDisc.IsScored)
         {
            //DoScoring
         }
         else
         {
            foreach (Player p in Players)
            {
               if (p.HasDisc)
               {
                  GameDisc.SetPosition(p.RightHandPosition);
               }
               p.Update(gameTime);
            }
            foreach (ScoreZone sz in ScoreZones)
            {
               sz.Update(gameTime);
            }
            GameDisc.Update(gameTime);
         }
         if(GameDisc.IsInPlay)
            CheckForScore(GameDisc);
         scoreBoard.Update(gameTime);
      }
      #endregion

      #region Draw
      /// <summary>
      /// Draw everything in the level from background to foreground.
      /// </summary>
      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {

         foreach (Player p in Players)
         {
            p.Draw(gameTime, spriteBatch);
         }
         foreach (ScoreZone sz in ScoreZones)
         {
            sz.Draw(gameTime, spriteBatch);
         }
         foreach (Wall w in Walls)
         {
            w.Draw(gameTime, spriteBatch);
         }
         GameDisc.Draw(gameTime, spriteBatch);
         scoreBoard.Draw(gameTime, spriteBatch);
      }
      #endregion

      #region Helpers
      protected bool canScore = true;
      public void ResetDisc(PlayerIndex PlayerToGiveTo)
      {
         if (canScore)
         {
            canScore = false;

            GameDisc.TakeOutOfPlay();
            PlayersAtDestination = 0;
            Players[0].Reset(PlayerReachedDestination);
            Players[1].Reset(PlayerReachedDestination);
         }
      }

      protected int PlayersAtDestination = 0;
      protected void PlayerReachedDestination()
      {
         lock(this)
         {
            PlayersAtDestination++;
            if (PlayersAtDestination == Players.Length)
            {
               //GameDisc.ThrowTo(Players[1]);
            }
         }
      }
      public bool CollidesWith(Entity entity, String EntityType = "")
      {
         foreach (Entity courtEntity in _CourtEntities)
         {
            if (String.IsNullOrWhiteSpace(EntityType) || String.Equals(EntityType, courtEntity.EntityType))
            {
               if (entity.BoundingRectangle.Intersects(courtEntity.BoundingRectangle))
               {
                  return true;
               }
            }
         }
         return false;
      }
      public Entity[] GetCollisionEntities(Entity entity, String EntityType = "")
      {
         List<Entity> entities = new List<Entity>();
         foreach (Entity courtEntity in _CourtEntities)
         {
            if (String.IsNullOrWhiteSpace(EntityType) || String.Equals(EntityType, courtEntity.EntityType))
            {
               if (entity.BoundingRectangle.Intersects(courtEntity.BoundingRectangle))
               {
                  entities.Add(courtEntity);
               }
            }
         }
         return entities.ToArray();
      }

      public void CheckForScore(Disc gameDisc)
      {
         
         foreach (Player player in Players)
         {
            // If a player has the disc, we don't want to score
            if (player.HasDisc)
               return;

         }
         foreach (ScoreZone zone in ScoreZones)
         {
            if (zone.BoundingRectangle.Intersects(gameDisc.BoundingRectangle))
            {
               if (zone.PlayerIndex == PlayerIndex.One)
               {
                  scoreBoard.PlayerTwoScore++;
               }
               else if (zone.PlayerIndex == PlayerIndex.Two)
               {
                  scoreBoard.PlayerOneScore++;
               }
               ResetDisc(zone.PlayerIndex);
               break;
            }
         }
      }

      public void ThrowDisc(float angle)
      {
         GameDisc.Throw(angle);
      }
      #endregion
   }
}
