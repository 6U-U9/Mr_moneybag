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
        public const int width=79, height=79;
        static Random rnd = new Random();
        public int level;
        public int turn = 0;
        public int coinsonfloor = 24, newredgen = 6, rednoticedist = 10, sight = 6, InitPlayerMoneyLimit = 5;
        public int[] shop_amount = new int[] { 2, 2, 2, 2, 2, 2 }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 
        public GameObject[,] status = new GameObject[height, width];
        public const int initial_x = 40;
        public const int initial_y = 40;
        public Player player;
        public int timer = 0;
        public List<Enemy> enemies = new List<Enemy>();

        public Gameboard()
        {
            level = 1;
            player= new Player(this, 3, initial_x, initial_y, InitPlayerMoneyLimit);
            //genlevel(level); 
            Level.GenRandomLevel(this, level);
        }
        public void genlevel(int level)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    status[i, j] = new Space(this,i,j);
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

        public void IncreaseTimer()
        {
            timer += 1;
            if (timer == newredgen)
            {
                timer = 0;
                SpawnEnemy();
            }
            if(timer % 2 == 0)
            foreach (Enemy enemy in enemies)
            {
                enemy.move();
            }
        }

        public void SpawnEnemy()
        {
            //if (enemies.Count >= 1) return;
            bool success = false;
            while (!success)
            {
                int x = rnd.Next(1, height - 1);
                int y = rnd.Next(1, width - 1);

                bool duplicate = false;
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.x == x && enemy.y == y)
                    { 
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate) continue;
                if (status[x, y] is Space && player.x != x && player.y != y)
                {
                    enemies.Add(new Enemy(this, 1, x, y));
                    success = true;
                    Console.WriteLine("Enemy Spawn at " + x + ", " + y);
                }
                
            }
            
        }

        public bool HasEnemy(int x, int y)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.x == x && enemy.y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public Enemy GetEnemy(int x, int y)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.x == x && enemy.y == y)
                {
                    return enemy;
                }
            }
            return null;
        }
    }
}
