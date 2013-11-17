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
   public abstract class Entity
   {
      /// <summary>
      /// Entity types are used in collision calculations.
      /// </summary>
      public String EntityType { get; protected set; }
      public Vector2 Position { get; set; }
      public virtual Rectangle BoundingRectangle
      {
         get;
      }

      public Entity(Vector2 Position, String EntityType)
      {
         this.EntityType = EntityType;
         this.Position = Position;
      }
   }
}
