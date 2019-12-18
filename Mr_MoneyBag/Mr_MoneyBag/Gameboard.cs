using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Mr_MoneyBag
{
    class GameBoard
    {
        public const int width=39, height=39;
        readonly int default_cof = 24, default_nrg = 20, default_rnd = 20;
        readonly int default_sr = 3;
        readonly double default_st = 5.9;
        readonly int[] default_sa = { 2, 2, 2, 2, 2, 2 }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 
        readonly int init_level = 2;

        static Random rnd = new Random();
        public int level;
        public double shopnoticedist = 2.1;
        public int turn = 0;
        public int coinsonfloor = 24, newredgen = 3, rednoticedist = 20,  InitPlayerMoneyLimit = 5000;
        public int shootrange = 3;
        public double sight = 5.9;
        public int[] shop_amount = new int[] { 2, 2, 2, 2, 2, 2 }; // coinonfloor, newredgen, rednoticedist, sight, damage, moneylimit, 
        public GameObject[,] status = new GameObject[height, width];
        public const int initial_x = 20;
        public const int initial_y = 20;
        public Player player;
        public int timer = 0;
        public List<Enemy> enemies = new List<Enemy>();
        public List<Bullet> bullets = new List<Bullet>();
        public bool is_newlevel = false;

        public GameBoard()
        {
            level = init_level;
            player= new Player(this, InitPlayerMoneyLimit, initial_x, initial_y, InitPlayerMoneyLimit);
            
            GenRandomLevel(level);
        }
        public void restart()
        {
            level = init_level;
            player = new Player(this, InitPlayerMoneyLimit, initial_x, initial_y, InitPlayerMoneyLimit);
            coinsonfloor = default_cof; newredgen = default_nrg; rednoticedist = default_rnd; shootrange = default_sr;
            sight = default_st;
            default_sa.CopyTo(shop_amount, 0);
            enemies = new List<Enemy>();
            timer = 0;
            status = new GameObject[height, width];
            GenRandomLevel(level);
        }

        public void NextLevel()
        {
            level = level + 1;
            enemies = new List<Enemy>();
            timer = 0;
            status = new GameObject[height, width];
            player.x = initial_x;
            player.y = initial_y;
            GenRandomLevel(level);
            is_newlevel=true;

        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public void FreshBullets()
        {
            if (bullets.Count > 0)
                for (int i=0; i< bullets.Count;i++ )
                    bullets[i].Move();
        }
        public void IncreaseTimer()
        {
            timer += 1;
            if (timer % newredgen==0)
            {
                SpawnEnemy();
            }
            
            foreach (Enemy enemy in enemies)
            {if (timer % 2 == 0)
                    enemy.Move();
                else
                    enemy.ReadytoAction();
            }

            Console.WriteLine("Money: " + player.hp + " Limit " + player.moneylimit + " CoinsOnFloor " + coinsonfloor + " NewRedGen " + newredgen + " RedNoticeDist " + rednoticedist + " sight " + sight + " damage " + player.attack);

            // check shops and notice
            DisplayShopNotice(player.x, player.y);
        }

        public void DisplayShopNotice(int x, int y) // check the status around x and y and notice the nearest shop
        {
            int xst = x - (int)shopnoticedist;
            if (xst < 0) xst = 0;
            int xed = x + (int)shopnoticedist;
            if (xed > height - 1) xed = height - 1;
            int yst = y - (int)shopnoticedist;
            if (yst < 0) yst = 0;
            int yed = y + (int)shopnoticedist;
            if (yed > width - 1) yed = width - 1;

            double mindist = shopnoticedist;
            int minx = 0, miny = 0;

            for (int i = xst; i <= xed; i++)
            {
                for (int j = yst; j <= yed; j++)
                {
                    if (!(status[i, j] is Shop)) continue;
                    double temp = status[i, j].distance(x, y);
                    if (temp < mindist)
                    {
                        Console.WriteLine("d: " + temp + " from " + i + "," + j + " to " + x + "," + y);
                        mindist = temp;
                        minx = i; miny = j;
                    }
                }
            }
            //Console.WriteLine("aaa");
            if (mindist >= shopnoticedist) return;
            //Console.WriteLine("bbb");

            try
            {
                ((Shop)(status[minx, miny])).notice();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Cannot convert to shop!", e);
            }

        }

        public void SpawnEnemy()
        {
            //if (enemies.Count >= 3) return;
            
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
                    enemies.Add(new Enemy(this, rnd.Next(0, level + 1), x, y));
                    success = true;
                    Console.WriteLine("Enemy Spawn at " + x + ", " + y);
                }
                
            }
            DisplayEnemy();
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

        public void DisplayEnemy()
        {
            Console.Write("Current Enemies: ");
            foreach (Enemy enemy in enemies)
            {
                Console.Write("(" + enemy.x + ", " + enemy.y + ") ");
                
            }
            Console.WriteLine();
        }

        public void GenRandomLevel(int lv)
        {
            GenBasicMap();
            AddStair();
            AddShop(lv);
            AddMoney();
            
        }

        public void AddStair()
        {
            bool success = false;
            while (!success)
            {
                int x = rnd.Next(0, height);
                int y = rnd.Next(0, width);

                if (status[x, y] is Space && player.x != x && player.y != y)
                {
                    status[x, y] = new Gate(this, x, y);
                    success = true;
                }

            }
        }

        public void AddShop(int lv)
        {
            for (int i = 0; i < shop_amount.Length; i++)
            {
                for (int j = 0; j < shop_amount[i]; j++)
                {
                    bool success = false;
                    while (!success)
                    {
                        int x = rnd.Next(1, height - 1);
                        int y = rnd.Next(1, width - 1);

                        if (status[x, y] is Space && player.x != x && player.y != y &&
                            !(status[x - 1, y].isblocked && status[x + 1, y].isblocked) &&
                            !(status[x, y - 1].isblocked && status[x, y + 1].isblocked)
                            )
                        {
                            switch (i)
                            {
                                case 0:
                                    status[x, y] = new CoinOnFloor_Shop(this, rnd.Next(1, 6), x, y);
                                    break;
                                case 1:
                                    status[x, y] = new NewRedGen_Shop(this, rnd.Next(1, 6), x, y);
                                    break;
                                case 2:
                                    status[x, y] = new RedNoticeDist_Shop(this, rnd.Next(1, 6), x, y);
                                    break;
                                case 3:
                                    status[x, y] = new Sight_Shop(this, rnd.Next(1, 6), x, y);
                                    break;
                                case 4:
                                    status[x, y] = new Damage_Shop(this, rnd.Next(1, 6), x, y);
                                    break;
                                case 5:
                                    status[x, y] = new MoneyLimit_Shop(this, rnd.Next(1, 6), x, y);
                                    break;

                            }
                            success = true;
                        }

                    }
                }

            }
        }

        public void AddMoney()
        {
            for (int i = 0; i < coinsonfloor; i++)
            {
                bool success = false;
                while (!success)
                {
                    int x = rnd.Next(0, height);
                    int y = rnd.Next(0, width);

                    if (status[x, y].GetType() == typeof(Space) && player.x != x && player.y != y)
                    {
                        status[x, y] = new Money(this, x, y);
                        success = true;
                    }

                }

            }
        }


        public void GenBasicMap()
        {

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || j == 0 || i == height - 1 || j == width - 1)
                        status[i, j] = new UnbreakableWall(this, i, j);
                    else
                        status[i, j] = new Wall(this, i, j);
                }
            }

            bool[,] vis = new bool[height, width];
            GenBasicMapHelper(vis, initial_x, initial_y, 0);
            //status[initial_x, initial_y] = board.player;
        }


        private void GenBasicMapHelper(bool[,] vis, int x, int y, int step)
        {

            if (x > width - 2 || x < 1 || y > height - 2 || y < 1) return;
            
            if (vis[y, x]) return;
            if (status[y, x].GetType() == typeof(Space)) return;

            vis[y, x] = true;
            //Console.WriteLine(x + " -xy- " + y);
            status[y, x] = new Space(this, y, x);

            //int[] rndorder = order.OrderBy(t => rnd.Next()).ToArray();
            //int next = rnd.Next(0, 4);
            //GenBasicMapHelper(board, vis, x + dir[next, 0], y + dir[next, 1], step + 1);
            int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
            int[,] parameter = new int[,] { { 4, 0 }, { 5, 0 }, { 255, 1 }, { 505, 1 } };

            int prev = -1;
            for (int i = 0; i < 4; i++)
            {

                if (rnd.Next(0, (step / parameter[i, 0]) + parameter[i, 1]) == 0)
                {
                    int next = rnd.Next(0, 4);
                    if (i == 0) prev = next;
                    else if (next == prev)
                    {
                        while (next == prev)
                            next = rnd.Next(0, 4);
                    }
                    GenBasicMapHelper(vis, x + dir[next, 0], y + dir[next, 1], step + 1);
                }
            }
        }

    }
}
