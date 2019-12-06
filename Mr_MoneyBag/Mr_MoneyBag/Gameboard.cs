using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Mr_MoneyBag
{
    class Gameboard
    {
        public const int width=99, height=99;
        public int level;
        public int turn = 0;
        public GameObject[,] status = new GameObject[height, width];
        
        public Player player;
        public Gameboard()
        {
            level = 1;
            player= new Player(this,3, 10, 10);
            //genlevel(level); 
            Level.GenRandomLevel(this, level);
        }
        public void genlevel(int level)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    status[i, j] = new Space(this);
            status[10, 10] = player;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}
