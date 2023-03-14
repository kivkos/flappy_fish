using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Skoljka : Sprite
    {
        public override int Y
        {
            get
            {
                return base.Y;
            }

            set
            {
                if (value > GameOptions.DownEdge)
                {
                    Random r = new Random();                    
                    y = 0;
                    x = r.Next(0, GameOptions.RightEdge- 200);                    
                }
                else
                    base.Y = value;
            }
        }

        public Skoljka(string putanja, int x, int y)
            : base(putanja, x, y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

