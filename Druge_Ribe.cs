using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Druge_Ribe : Sprite
    {
        protected bool pliva;
        public bool Pliva
        {
            get { return pliva; }
            set { pliva = value; }
        }

        protected int vrijednost_Bodova;
        public int Vrijednost_Bodova
        {
            get { return vrijednost_Bodova; }
            set { vrijednost_Bodova = value; }
        }        

        public Druge_Ribe(string putanja, int x, int y)
            : base(putanja, x, y)
        {
            this.X = x;
            this.Y = y;
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
                    Random r = new Random();
                    value = GameOptions.RightEdge - this.Width;
                    x = value;
                    y = r.Next(0, GameOptions.DownEdge - this.Heigth);
                    //this.Pliva = false;
                    //this.SetVisible(false);
                }
                else
                    base.X = value;
            }
        }
        public override int Y
        {
            get
            {
                return base.Y;
            }

            set
            {
                if (value < GameOptions.UpEdge)
                {
                    value = GameOptions.UpEdge;
                    y = value;
                }
                else if (value>GameOptions.DownEdge)
                {
                    value = GameOptions.DownEdge-this.Heigth;
                    y = value;
                }
                else
                    base.y = value;
            }
        }
    }

    public class Obicna_riba : Druge_Ribe
    {
        public Obicna_riba(string putanja, int x, int y)
            :base(putanja, x, y)
        {
            this.X = x;
            this.Y = y;
            this.Vrijednost_Bodova = 15;            
        }
    }

    public class Morski_pas : Druge_Ribe
    {
        public Morski_pas(string putanja, int x, int y)
            :base (putanja, x,y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    public class Meduza : Druge_Ribe
    {

        public override int Y
        {
            get
            {
                return base.Y;
            }

            set
            {
                Random r = new Random();
                if (value < GameOptions.UpEdge)
                {
                    value = r.Next(0, GameOptions.UpEdge);
                    y = value;
                }
                else if (value > GameOptions.DownEdge)
                {
                    value = r.Next(0, GameOptions.DownEdge);
                    y = value;
                }
                else
                    base.y = value;
            }
        }
        public void MeduzaKretanje()
        {
            this.SetDirection(270);
            this.MoveSimple(2);
            this.SetDirection(180);
            this.MoveSimple(2);
        }
        public Meduza(string putanja, int x, int y)
            : base(putanja, x, y)
        {
            this.X = x;
            this.Y = y;
            this.Vrijednost_Bodova = -10;
        }
    }

    }
