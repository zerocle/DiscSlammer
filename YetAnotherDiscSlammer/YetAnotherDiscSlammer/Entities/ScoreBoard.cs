using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using YetAnotherDiscSlammer.Common;

namespace YetAnotherDiscSlammer.Entities
{
   public class ScoreBoard
   {
      public int PlayerOneScore;
      public int PlayerTwoScore;

      protected Vector2 Position;
      protected FontReader Writer;
      protected Texture2D Background;
      protected const float overallScale = 0.5f;

      public String PlayerOneName { get; protected set; }
      public String PlayerTwoName { get; protected set; }

      public ScoreBoard(Vector2 Position, String PlayerOneName, String PlayerTwoName)
      {
         this.PlayerOneName = PlayerOneName;
         if (PlayerOneName.Length > 8)
         {
            this.PlayerOneName = PlayerOneName.Substring(0, 8);
         }
         else
         {
            this.PlayerOneName = PlayerOneName.PadRight(8, ' ');
         }
         if (PlayerTwoName.Length > 8)
         {
            this.PlayerTwoName = PlayerTwoName.Substring(0, 8);
         }
         else
         {
            this.PlayerTwoName = PlayerTwoName.PadRight(8, ' ');
         }
         this.Position = Position;
         Writer = new FontReader();
         if (Writer.Init(48, 56))
         {

         }
      }

      public void LoadContent(ContentManager content)
      {
         Background = content.Load<Texture2D>("Sprites\\ScoreBoard\\ScoreBoardBackground");
         Writer.LoadContent(content, "Font\\ScoreBoardFont");
      }

      public void Update(GameTime gameTime)
      {
         Writer.Update(gameTime);
      }

      public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
      {
         Vector2 center= new Vector2(Background.Bounds.Width / 2, Background.Bounds.Height / 2);
         spriteBatch.Draw(Background, Position, null, Color.White, 0.0f, Vector2.Zero, new Vector2(overallScale, overallScale), SpriteEffects.None, 0);
         Writer.SetText(PlayerOneName);
         Writer.Draw(gameTime, spriteBatch, Position + new Vector2(34, 55) * overallScale, 0.5f * overallScale);
         Writer.SetText(PlayerTwoName);
         Writer.Draw(gameTime, spriteBatch, Position + new Vector2(284, 55) * overallScale, 0.5f * overallScale);
         Writer.SetText(PlayerOneScore.ToString().PadLeft(2, '0'));
         Writer.Draw(gameTime, spriteBatch, Position + new Vector2(73, 95) * overallScale, 1.0f * overallScale);
         Writer.SetText(PlayerTwoScore.ToString().PadLeft(2, '0'));
         Writer.Draw(gameTime, spriteBatch, Position + new Vector2(332, 95) * overallScale, 1.0f * overallScale);
      }
   }
}
