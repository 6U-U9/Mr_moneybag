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
        public const int blocksize = 30;
        public static int y = 30, x = 17;
        public PictureBox[,] background = new PictureBox[x, y];
        public PictureBox[,] map = new PictureBox[x, y];
        string[,] mapname = new string[x, y];
        Gameboard gameboard = new Gameboard();
        private bool is_space_down = false;
        private bool arrow_key_locked = false;
        
        

        public Form1()
        {
            
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    map[i, j] = new PictureBox();
                    ((System.ComponentModel.ISupportInitialize)(map[i,j])).BeginInit();
                    map[i, j].Location = new System.Drawing.Point(blocksize * j, blocksize * i);
                    map[i, j].Size = new System.Drawing.Size(blocksize, blocksize);
                    this.Controls.Add(map[i, j]);
                    ((System.ComponentModel.ISupportInitialize)(map[i,j])).EndInit();
                }
            
            refresh();
            InitializeComponent();
            DoubleBuffered = true;
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
            refresh();
        }
        public void refresh()
        {
            int x = gameboard.player.x;
            int y = gameboard.player.y;
            int x_st = x - Form1.x / 2;
            if (x_st < 0) x_st = 0; 
            int x_ed = x_st + Form1.x;
            if (x_ed > (gameboard.GetHeight() )) { x_ed = gameboard.GetHeight() ; x_st = x_ed - Form1.x; }
            int y_st = y - Form1.y / 2;
            if (y_st < 0) y_st = 0;
            int y_ed = y_st + Form1.y;
            if (y_ed > (gameboard.GetWidth() )) { y_ed = gameboard.GetWidth() ; y_st = y_ed - Form1.y; }

            //Console.WriteLine(x + " " + y);
            //Console.WriteLine(x_st + " " + x_ed + " | " + y_st + " " + y_ed);
            
            for (int i = x_st; i < x_ed; i++)
            {
                for (int j = y_st; j < y_ed; j++)
                {
                    int a = i - x_st;
                    int b = j - y_st;
                    //Console.WriteLine(a + " ab " + b);
                    //Console.WriteLine(i + " ij " + j);
                                        
                    string imgname = gameboard.status[i, j].GetImageName();
                    //Console.WriteLine(imgname);

                    if (mapname[a, b] != imgname || (Math.Abs((x - x_st) - a) <= 1 || Math.Abs((y - y_st) - b) <= 1))
                    {
                        map[a, b].Image = gameboard.status[i, j].getimage();
                        mapname[a, b] = imgname;
                    }
                }
            }
            //Console.WriteLine(x + " " + y);
            map[x - x_st, y - y_st].Image = gameboard.player.getimage();
            Console.WriteLine("Money:"+gameboard.player.hp+" Limit"+gameboard.player.moneylimit+" CoinsOnFloor"+gameboard.coinsonfloor+" NewRedGen"+gameboard.newredgen+" RedNoticeDist"+gameboard.rednoticedist+" sight"+gameboard.sight+" damage"+gameboard.player.attack);

        }
        /*private Image GetShowImage(Gameboard gameboard, int x, int y)
        {
            if (gameboard.player.x == x && gameboard.player.y == y)
                return Properties.Resources.player;
            GameObject gameObject = gameboard.status[x, y];
            int dist = Math.Abs(x - gameboard.player.x) + Math.Abs(y - gameboard.player.y);
            if (dist <= gameboard.sight)
                if ((x+y)%2==1)
                    return UniteImage(Properties.Resources.back1, gameboard.status[x, y].getimage());
                else
                    return UniteImage(Properties.Resources.back0, gameboard.status[x, y].getimage());
            if (gameObject.seen == true)
                return UniteImage(Properties.Resources.Seen, gameboard.status[x, y].getimage());
            if (gameObject.NearlySeen == true)
                return Properties.Resources.NearlySeen;
            return Properties.Resources.Unseen;
        }*/
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
                    arrow_key_locked=false;
                    break;
                default:
                    return;
            }
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
        public Image UniteImage(Image img1, Image img2,int width=blocksize, int height=blocksize )
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
