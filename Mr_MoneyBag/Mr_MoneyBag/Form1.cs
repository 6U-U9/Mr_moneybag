using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mr_MoneyBag
{
    public partial class Form1 : Form
    {
        public const int blocksize = 32;
        public static int y = 30, x = 17;
        public const double animespeed = 0.4; //动画运动速度
        //public PictureBox[,] background = new PictureBox[x, y];
        public PictureBox map = new PictureBox();
        string[,] mapname = new string[x, y];

        Gameboard gameboard = new Gameboard();
        private bool is_space_down = false;
        private bool arrow_key_locked = false;
        private double x_position, y_position;//记录动画中盘面位置


        public Form1()
        {
            ((System.ComponentModel.ISupportInitialize)(map)).BeginInit();
            map.Location = new System.Drawing.Point(0, 0);
            map.Size = new System.Drawing.Size(blocksize * y, blocksize * x);
            this.Controls.Add(map);
            ((System.ComponentModel.ISupportInitialize)(map)).EndInit();
            map.BringToFront();
            //map.Image= GetFullImage(gameboard, 0, 0, Form1.x, Form1.y);
            //map.Image = GetShowImage(gameboard, 0, 0);
            InitializeComponent();
            DoubleBuffered = true;
            InitNumbers();
            RefreshBoard();
            timerFresh.Interval = 10;
            timerFresh.Start();
        }
        private ValueTuple<double, double> GetXYst()
        {
            int x = gameboard.player.x;
            int y = gameboard.player.y;
            int x_st = x - Form1.x / 2;
            if (x_st < 0) x_st = 0;
            int x_ed = x_st + Form1.x;
            if (x_ed > (gameboard.GetHeight())) { x_ed = gameboard.GetHeight(); x_st = x_ed - Form1.x; }
            int y_st = y - Form1.y / 2;
            if (y_st < 0) y_st = 0;
            int y_ed = y_st + Form1.y;
            if (y_ed > (gameboard.GetWidth())) { y_ed = gameboard.GetWidth(); y_st = y_ed - Form1.y; }
            return (x_st, y_st);
        }
        private void InitNumbers()
        {
            (x_position, y_position) = GetXYst();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Space)
                is_space_down = true;
            if (!arrow_key_locked)
            {
                if (is_space_down == true)
                    switch (e.KeyCode)
                    {
                        case Keys.W:
                        case Keys.Up:
                            gameboard.player.shootup();
                            arrow_key_locked = true;
                            break;
                        case Keys.A:
                        case Keys.Left:
                            gameboard.player.shootleft();
                            arrow_key_locked = true;
                            break;
                        case Keys.S:
                        case Keys.Down:
                            gameboard.player.shootdown();
                            arrow_key_locked = true;
                            break;
                        case Keys.D:
                        case Keys.Right:
                            gameboard.player.shootright();
                            arrow_key_locked = true;
                            break;
                    }
                else if (is_space_down == false)
                    switch (e.KeyCode)
                    {
                        case Keys.W:
                        case Keys.Up:
                            gameboard.player.moveup();
                            arrow_key_locked = true;
                            break;
                        case Keys.A:
                        case Keys.Left:
                            gameboard.player.moveleft();
                            arrow_key_locked = true;
                            break;
                        case Keys.S:
                        case Keys.Down:
                            gameboard.player.movedown();
                            arrow_key_locked = true;
                            break;
                        case Keys.D:
                        case Keys.Right:
                            gameboard.player.moveright();
                            arrow_key_locked = true;
                            break;
                    }

            }
            //RefreshBoard();
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                is_space_down = false;
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    arrow_key_locked = false;
                    break;
                default:
                    return;
            }
        }
        public void RefreshBoard()
        {
            double x_st, y_st;
            (x_st, y_st) = GetXYst();
            //Console.WriteLine(x_st+" "+y_st);
            Image image;
            if (Math.Abs(x_st - x_position) > animespeed / 2)
            {
                if (x_st - x_position > 0)
                    x_position = Math.Min(x_position + animespeed, x_st);
                if (x_st - x_position < 0)
                    x_position = Math.Max(x_position - animespeed, x_st);
            }
            else
                x_position = x_st;

            if (Math.Abs(y_st - y_position) > animespeed / 2)
            {
                if (y_st - y_position > 0)
                    y_position = Math.Min(y_position + animespeed, y_st);
                if (y_st - y_position < 0)
                    y_position = Math.Max(y_position - animespeed, y_st);
            }
            else
                y_position = y_st;
            

            image = GetFullImage(gameboard, x_position, y_position, Form1.x, Form1.y);
            map.Image = image;

        }
        private Image GetShowImage(Gameboard gameboard, int x, int y)
        {
            

            GameObject gameObject = gameboard.status[x, y];
            double dist = gameObject.distance(gameboard.player.x, gameboard.player.y);
            if (dist <= gameboard.sight)
            {
                if (gameboard.player.x == x && gameboard.player.y == y)
                    return UniteImage(Properties.Resources.Back001, Properties.Resources.Player001);
                if ((x + y) % 2 == 1)
                {
                    foreach (Enemy enemy in gameboard.enemies)
                    {
                        if (enemy.x == x && enemy.y == y) return UniteImage(Properties.Resources.Back001, enemy.getimage());
                    }
                    return UniteImage(Properties.Resources.Back001, gameboard.status[x, y].getimage());
                }
                else
                {
                    if (gameboard.player.x == x && gameboard.player.y == y)
                        return UniteImage(Properties.Resources.Back002, Properties.Resources.Player001);
                    foreach (Enemy enemy in gameboard.enemies)
                    {
                        if (enemy.x == x && enemy.y == y) return UniteImage(Properties.Resources.Back002, enemy.getimage());
                    }
                    return UniteImage(Properties.Resources.Back002, gameboard.status[x, y].getimage());
                }
            }
            /*if (gameObject is Space)
                Console.WriteLine(gameObject.seen);*/
            if (gameObject.seen == true)
                return UniteImage(Properties.Resources.Seen, gameboard.status[x, y].getimage());
            if (gameObject.NearlySeen == true)
                return Properties.Resources.Nearlyseen;
            return Properties.Resources.Unseen;
        }
        private Image GetUnmoveableImage(Gameboard gameboard, int x, int y)
        {
            GameObject gameObject = gameboard.status[x, y];
            double dist = gameObject.distance(gameboard.player.x, gameboard.player.y);
            if (dist <= gameboard.sight)
            {
                if ((x + y) % 2 == 1)
                {
                    return UniteImage(Properties.Resources.Back001, gameboard.status[x, y].getimage());
                }
                else
                {
                    return UniteImage(Properties.Resources.Back002, gameboard.status[x, y].getimage());
                }
            }
            if (gameObject.seen == true)
                return UniteImage(Properties.Resources.Seen, gameboard.status[x, y].getimage());
            if (gameObject.NearlySeen == true)
                return Properties.Resources.Nearlyseen;
            return Properties.Resources.Unseen;
        }
        private Image GetFullImage(Gameboard gameboard, double x_st, double y_st, int x_len, int y_len)
        {
            System.Drawing.Image img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            int x, y;
            x = Math.Max((int)Math.Floor(x_st),0);
            y = Math.Max((int)Math.Floor(y_st),0);
            if (x_st > x)
                x_len++;
            if (y_st > y)
                y_len++;
            Console.WriteLine(x_st + "  " + x + "  " + y_st + "  " + y);
            System.Drawing.Image whole_img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            System.Drawing.Graphics whole_g = System.Drawing.Graphics.FromImage(whole_img);
            for (int i = x; i < x + x_len; i++)
                for (int j = y; j < y + y_len; j++)
                {
                    whole_g.DrawImage(GetShowImage(gameboard, i, j), blocksize * (j - y), blocksize * (i - x), blocksize, blocksize);
                }
            g.DrawImage(whole_img, (int)(-(y_st - y) * blocksize), (int)(-(x_st - x) * blocksize));
            return img;
        }

        private void timerFresh_Tick(object sender, EventArgs e)
        {
            RefreshBoard();
            //Console.WriteLine("Tick");
        }
        protected override bool ProcessDialogKey(Keys keycode)
        {
            switch (keycode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Space:
                    return false;
                default:
                    return base.ProcessDialogKey(keycode);
            }
        }



        public Image UniteImage(Image img1, Image img2, int width = blocksize, int height = blocksize)
        {
            System.Drawing.Image img = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(img1, 0, 0, img1.Width, img1.Height);
            g.DrawImage(img2, 0, 0, img2.Width, img2.Height);
            return img;
        }
    }
}
