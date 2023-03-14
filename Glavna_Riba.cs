using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class Glavna_Riba : Sprite
    {
        private int bodovi;
        public int Bodovi
        {
            get { return bodovi; }
            set { bodovi = value; }
        }

        private int zivot;
        public int Zivot
        {
            get { return zivot; }
            set
            {                
                zivot = value;
            }
        }
        public override int X
        {
            get
            {
                return base.X;
            }

            set
            {
                if (value < GameOptions.LeftEdge)
                {
                    value = GameOptions.LeftEdge;
                    x = value;
                }
                else if (value > GameOptions.RightEdge - this.Heigth)
                {
                    value = GameOptions.RightEdge - this.Heigth;
                    x = value;
                }
                else
                    x = value;
            }
        }

        public override int Y
        {
            get
            {
                return y;
            }

            set
            {
                if (value < GameOptions.UpEdge)
                {
                    value = GameOptions.UpEdge;
                    y = value;
                }
                else if (value > GameOptions.DownEdge-this.Heigth)
                {
                    value = GameOptions.DownEdge-this.Heigth;
                    y = value;
                }
                else
                    y = value;
            }
        }

        public Glavna_Riba(string putanja, int x, int y)
            : base(putanja, x, y)
        {
            this.X = x;
            this.Y = y;
            this.Bodovi = 0;
            this.Zivot = 1;
        }
        
    }

}
