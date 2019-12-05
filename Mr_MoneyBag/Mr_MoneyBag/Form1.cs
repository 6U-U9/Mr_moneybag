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
        public int x, y;
        public PictureBox[,] map = new PictureBox[30, 18];
        Gameboard gameboard = new Gameboard();
        private bool is_space_down = false;
        public Form1()
        {
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    map[i, j] = new PictureBox();
                    map[i, j].Image = gameboard.status[i, j].getimage();
                    map[i, j].Location = new System.Drawing.Point(blocksize * i, blocksize * j);
                    map[i, j].Size = new System.Drawing.Size(blocksize, blocksize);
                    this.Controls.Add(map[i, j]);
                }
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                is_space_down = true;
            if(is_space_down==true)
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
