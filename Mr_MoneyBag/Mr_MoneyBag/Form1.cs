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
        public static int x = 30, y = 17;
        public PictureBox[,] map = new PictureBox[y, x];
        string[,] mapname = new string[y, x];
        Gameboard gameboard = new Gameboard();
        private bool is_space_down = false;
        
        public Form1()
        {
            for (int i = 0; i < y; i++)
                for (int j = 0; j < x; j++)
                {
                    map[i, j] = new PictureBox();
                    ((System.ComponentModel.ISupportInitialize)(map[i,j])).BeginInit();
                    //map[i, j].Image = gameboard.status[i, j].getimage();
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
            if (is_space_down == true)
                switch (e.KeyCode)
                {
                    case Keys.W:
                    case Keys.Up:
                        gameboard.player.shootup();
                        break;
                    case Keys.A:
                    case Keys.Left:
                        gameboard.player.shootleft();
                        break;
                    case Keys.S:
                    case Keys.Down:
                        gameboard.player.shootdown();
                        break;
                    case Keys.D:
                    case Keys.Right:
                        gameboard.player.shootright();
                        break;
                }
            else if (is_space_down == false)
                switch (e.KeyCode)
                {
                    case Keys.W:
                    case Keys.Up:
                        gameboard.player.moveup();
                        break;
                    case Keys.A:
                    case Keys.Left:
                        gameboard.player.moveleft();
                        break;
                    case Keys.S:
                    case Keys.Down:
                        gameboard.player.movedown();
                        break;
                    case Keys.D:
                    case Keys.Right:
                        gameboard.player.moveright();
                        break;
                }
            refresh();
        }
        public void refresh()
        {
            int x = gameboard.player.x;
            int y = gameboard.player.y;
            int x_st = x - Form1.x / 2 + 1;
            if (x_st < 0) x_st = 0; 
            int x_ed = x_st + Form1.x;
            if (x_ed > (gameboard.GetHeight() - 1)) { x_ed = gameboard.GetHeight() - 1; x_st = x_ed - Form1.x; }
            int y_st = y - Form1.y / 2;
            if (y_st < 0) y_st = 0;
            int y_ed = y_st + Form1.y;
            if (y_ed > (gameboard.GetWidth() - 1)) { y_ed = gameboard.GetWidth() - 1; y_st = y_ed - Form1.y; }

            Console.WriteLine(x + " " + y);
            Console.WriteLine(x_st + " " + x_ed + " | " + y_st + " " + y_ed);

            for (int i = y_st; i < y_ed; i++)
            {
                for (int j = x_st; j < x_ed; j++)
                {
                    int a = i - y_st;
                    int b = j - x_st;
                    //Console.WriteLine(a + " ab " + b);
                    //Console.WriteLine(i + " ij " + j);
                                        
                    string imgname = gameboard.status[j, i].GetImageName();
                    if (mapname[a, b] != imgname)
                    {
                        map[a, b].Image = gameboard.status[j, i].getimage();
                        mapname[a, b] = imgname;
                    }

                }
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
    }
}
