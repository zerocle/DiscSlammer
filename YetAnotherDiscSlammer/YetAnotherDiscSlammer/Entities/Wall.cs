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
   public class Wall : Entity
   {
      protected Court _court;
      protected Vector2 _Size;
      protected Rectangle _BoundingRectangle;
      public override Rectangle BoundingRectangle
      {
         get
         {
            return _BoundingRectangle;
         }
      }

      public Wall(Court court, Vector2 Position, Vector2 Size)
         :base(Position, "Wall")
      {
         this._court = court;
         this._Size = Size;
         this._BoundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)_Size.X, (int)_Size.Y);
         Outline.Colori = Color.YellowGreen;
      }
   }
}
