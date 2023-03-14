using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Varijable */
        
        Morski_pas mp;
        Glavna_Riba gr;
        Obicna_riba or;
        Meduza med;
        Skoljka s;

        Random r =new Random();      

        bool padaSkoljka;

        public delegate void bodHandler();
        public event bodHandler _touchRiba;
        public event bodHandler _touchMeduza;

        public delegate void zivotHandler();
        public event zivotHandler _touchMorski;
        public event zivotHandler _touchSkoljka;
    
        /* Inicijalizacija */                  

        private void SetupGame()
        {
            //postavke pozadine
            SetStageTitle("IGRA");  
            setBackgroundPicture("backgrounds\\pozadina.jpg");            
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //dodavanje likova
            mp = new Morski_pas("sprites\\morski_pas.png", GameOptions.RightEdge, 0);
            mp.SetSize(90);
            Game.AddSprite(mp);
            
            gr = new Glavna_Riba("sprites\\glavna_riba.png", 100, 200);           
            Game.AddSprite(gr);
            gr.SetSize(80);

            or = new Obicna_riba("sprites\\obicna_riba.png", GameOptions.RightEdge,0);            
            Game.AddSprite(or);
            or.SetSize(80);

            med = new Meduza("sprites\\meduza.png", GameOptions.RightEdge, 0);
            med.SetSize(60);
            Game.AddSprite(med);

            s = new Skoljka("sprites\\skoljka.png", 100, 0);
            s.SetSize(50);
            Game.AddSprite(s);
            s.SetVisible(false);

            //event handlers
            _touchRiba += new bodHandler(ribaDodir);
            _touchMorski += new zivotHandler(morskiDodir);
            _touchSkoljka += new zivotHandler(skoljkaDodir);
            _touchMeduza += new bodHandler(meduzaDodir);

            //skripte pozivanje
            Game.StartScript(KretanjeMorski);
            Game.StartScript(KretanjeGlavna);
            Game.StartScript(KretanjeObicna);
            Game.StartScript(KretanjeMeduza);
            Game.StartScript(KretanjeSkoljka);
        }     
        
        /* Skripte */

        private int KretanjeMorski()
        {
            mp.X = GameOptions.RightEdge;
            mp.Y = r.Next(GameOptions.UpEdge, GameOptions.DownEdge-mp.Heigth);            
            mp.SetVisible(true);
            mp.Pliva = true;
            while (mp.Pliva) 
            {
                mp.SetDirection(270);
                mp.MoveSimple(5);                
                Wait(0.01);
            }
            return 0;
        }

        private int KretanjeObicna()
        {
            or.SetDirection(270);
            or.X = GameOptions.RightEdge;
            or.Y = r.Next(GameOptions.UpEdge, GameOptions.DownEdge - 150);            
            or.SetVisible(true);
            or.Pliva = true;

            while (or.Pliva)
            {                
                or.MoveSimple(2);
                Wait(0.01);
            }
            return 0;
        }

        private int KretanjeGlavna()
        {
            gr.SetDirection(0);
            while (START)
            {
                //kretanje
                gr.Y += 2;
                if (sensing.KeyPressed(Keys.Up))
                    gr.Y -= 20;
                else if (sensing.KeyPressed(Keys.Left))
                    gr.X -= 10;
                else if (sensing.KeyPressed(Keys.Right))
                    gr.X += 10;
                else if (sensing.KeyPressed(Keys.Down))
                    gr.Y += 10;                

                //dodiri s ostalima
                if (gr.TouchingSprite(or))
                {
                    or.Pliva = false;
                    _touchRiba.Invoke();
                }

                if (gr.TouchingSprite(mp))
                {
                    mp.Pliva = false;
                    _touchMorski.Invoke();
                }

                if (gr.TouchingSprite(med))
                {
                    med.Pliva = false;
                    _touchMeduza.Invoke();
                }
                
                //provjera života
                if (gr.Zivot < 0 || gr.TouchingEdge())
                {
                    ISPIS = "Bodovi: " + gr.Bodovi + "\nIGRA JE ZAVRŠENA!";                                
                    break;
                }

                Wait(0.01);
            }
            return 0;
        }

        private int KretanjeSkoljka()
        {
            int x= (r.Next(GameOptions.LeftEdge, GameOptions.RightEdge - this.Width));
            s.SetDirection(180);
            s.SetVisible(true);
            s.GotoXY(x, 0);                     
            
            padaSkoljka = true;
            while (padaSkoljka)
            {
                s.MoveSimple(8);
                Wait(0.01);

                if (s.TouchingSprite(gr))
                {
                    padaSkoljka = false;
                    _touchSkoljka.Invoke();
                    break;
                }
            }            
            return 0;
        }

        private int KretanjeMeduza()
        {
            med.X = GameOptions.RightEdge-this.Width;            
            med.Y = r.Next(GameOptions.UpEdge, GameOptions.DownEdge - med.Heigth);
            med.SetVisible(true);
            med.Pliva = true;

            while (med.Pliva)
            {               
                med.MeduzaKretanje();
                Wait(0.01);
            }                 
            
            return 0;
        }

        private void ribaDodir()
        {
            START = false;
            or.GotoXY(850, 0);
            or.SetVisible(false);

            gr.Bodovi += or.Vrijednost_Bodova;
            ISPIS = "Bodovi: " + gr.Bodovi.ToString() + "\nZivoti: " + gr.Zivot;

            Wait(0.1);
            START = true;
            Game.StartScript(KretanjeObicna);
        }

        private void morskiDodir()
        {
            START = false;
            mp.GotoXY(850, 0);
            mp.SetVisible(false);

            gr.Zivot-= 1;
            ISPIS = "Bodovi: " + gr.Bodovi.ToString()+"\nZivoti: "+gr.Zivot;                      
            
            Wait(0.1);
            START = true;
            Game.StartScript(KretanjeMorski);
        }
        private void meduzaDodir()
        {
            START = false;
            med.GotoXY(850, 0);
            med.SetVisible(false);

            gr.Bodovi += med.Vrijednost_Bodova;
            ISPIS = "Bodovi: " + gr.Bodovi.ToString() + "\nZivoti: " + gr.Zivot;

            Wait(0.1);
            START = true;
            Game.StartScript(KretanjeMeduza);
        }

        private void skoljkaDodir()
        {
            s.SetVisible(false);
            int x = r.Next(GameOptions.LeftEdge, GameOptions.RightEdge - this.Width);
            s.GotoXY(x, 0);           

            gr.Zivot += 1;
            ISPIS = "Bodovi: " + gr.Bodovi.ToString() + "\nZivoti: " + gr.Zivot;

            Wait(10);
            Game.StartScript(KretanjeSkoljka);
        }
        /* ------------ GAME CODE END ------------ */

    }
}
