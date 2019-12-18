using System;
using System.Drawing;

namespace Mr_MoneyBag
{
    class GameObject
    {
        public Gameboard gameboard;
        public int x, y,hp;
        public Image[] image;
        public bool seen;
        public bool isblocked=true;
        public bool NearlySeen;
        public int imageindex=0;

        public GameObject(Gameboard gameboard,int x,int y)
        {
            this.gameboard = gameboard;
            this.x = x;
            this.y = y;
            this.image = new Image[] { Properties.Resources.Space };
        }
        virtual public void damaged(int n)
        {
            hp -= n;
            if (hp <= 0) dead();
        }
        virtual public void dead()
        { 
            freshboard(x, y, new Space(gameboard,x,y,this.seen,this.NearlySeen));
        }
        //更新对象地图
        virtual public void freshboard(int x, int y, GameObject gameobject)
        {
            gameboard.status[x, y] = gameobject;
        }

        virtual public Image GetImage()
        {
            imageindex++;
            if (imageindex >= image.Length)
                imageindex = 0;
            return image[imageindex];
        }
        virtual public double distance(int x, int y)
        { return Math.Sqrt((this.x - x)*(this.x - x) + (this.y - y)*(this.y - y)); }
    }
    class Space : GameObject
    {
        public Space(Gameboard gameboard, int x, int y,bool seen=false, bool NearlySeen=false) : base(gameboard,x,y)
        {   this.isblocked = false;
            this.seen = seen;
            this.NearlySeen = NearlySeen;
            this.image =new Image[] { Properties.Resources.Space };
        }
    }
    class MoveableObject : GameObject
    {
        public Image[] movingimage;
        public bool is_moveing=false;
        public double x_position, y_position;
        public int attack;
        public MoveableObject(Gameboard gameboard, int money, int x, int y) : base(gameboard,x,y)
        {
            this.hp = money;
            this.image = new Image[] { Properties.Resources.Space };
            this.movingimage = new Image[] { Properties.Resources.Space };
            this.x_position = x;
            this.y_position = y;
        }
        virtual public void moveto(int x, int y)
        {
            if (moveable(x,y) <= 0)
                return;
            var pastx = this.x;
            var pasty = this.y;
            this.x = x;
            this.y = y;
            freshboard(pastx, pasty, new Space(gameboard,pastx,pasty));
            freshboard(x, y, this);
            is_moveing = true;
        }
        virtual public void shoot(int dx, int dy)
        {
            int cnt = 1;
            var x = this.x + dx;
            var y = this.y + dy;
            while (gameboard.status[x, y] is Space && !gameboard.HasEnemy(x, y) && cnt <= gameboard.shootrange)
            {
                if (x == gameboard.GetHeight() - 1 || x == 0 || y == gameboard.GetWidth() - 1 || y == 0)
                    break;
                x = x + dx;
                y = y + dy;
                cnt += 1;
            }
            bool doDamage = true;
            if (gameboard.HasEnemy(x, y))
                gameboard.GetEnemy(x, y).damaged(attack);
            else if (!(gameboard.status[x, y] is Space))
                gameboard.status[x, y].damaged(attack);
            else
                doDamage = false;
            if (doDamage)
                Console.WriteLine(x + "," + y + "damaged");
            is_moveing = true;
        }
        virtual public int moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return 0;
            if (gameboard.status[x, y].isblocked)
                return 0;
            if (gameboard.HasEnemy(x, y))
                return -1;
            return 1;
        }
        override public Image GetImage()
        {
            
            if (is_moveing)
            {   double animespeed = Form1.animespeed;
                imageindex++;
                if (Math.Abs(x - x_position) > animespeed)
                {
                    if (x - x_position > 0)
                        x_position = Math.Min(x_position + animespeed, x);
                    if (x - x_position < 0)
                        x_position = Math.Max(x_position - animespeed, x);
                }
                else
                    x_position = x;

                if (Math.Abs(y - y_position) > animespeed)
                {
                    if (y - y_position > 0)
                        y_position = Math.Min(y_position + animespeed, y);
                    if (y - y_position < 0)
                        y_position = Math.Max(y_position - animespeed, y);
                }
                else
                    y_position = y;

                if (imageindex >= movingimage.Length && x == x_position && y == y_position)
                { imageindex = 0; is_moveing = false; }
                else if (imageindex >= movingimage.Length)
                { imageindex = 0; }
                //Console.WriteLine("ismoving");
                return movingimage[imageindex];
            }
            else
            {
                imageindex++;
                if (imageindex >= image.Length)
                    imageindex = 0;
                return image[imageindex];
            }
        }
    }
    class Player : MoveableObject
    {
        public int moneylimit;
        public Player(Gameboard gameboard,int money,int x,int y,int moneylimit,int attack=1): base(gameboard,money,x,y)
        {
            this.image = new Image[] { Properties.Resources.Player001 };
            this.movingimage = new Image[] { Properties.Resources.Player001, Properties.Resources.Player002, Properties.Resources.Player003 };
            this.moneylimit = moneylimit;
            this.attack = attack;
        }
        /*public override int moveable(int x, int y)
        {
            if (x > gameboard.GetHeight() - 1 || x < 0 || y > gameboard.GetWidth() - 1 || y < 0)
                return false;
            if (gameboard.status[x, y] is Enemy) this.damaged(((Enemy)gameboard.status[x, y]).attack);
            return base.moveable(x, y);
        }*/
        override public void moveto(int x, int y)
        {
            int canmove = moveable(x, y);
            if (canmove == 0) return;
            if (canmove == -1) { gameboard.IncreaseTimer(); return; }
            
            Console.WriteLine("Player: " + x + ", " + y);

            this.x = x;
            this.y = y;
            
            //判断是否到达Gate
            if (gameboard.status[this.x, this.y] is Gate)
            {
                gameboard.level += 1;
                Level.GenRandomLevel(gameboard, gameboard.level);
            }
            //判断是否获得Money
            if (gameboard.status[this.x, this.y] is Money&&this.hp<this.moneylimit)
            {
                getmoney(((Money)gameboard.status[this.x, this.y]).hp);
                freshboard(this.x,this.y, new Space(gameboard, this.x, this.y,true));
            }
            gameboard.IncreaseTimer();

            foreach (GameObject gameObject in gameboard.status)
            {
                if (gameObject.distance(this.x, this.y) < gameboard.sight)
                    gameObject.seen = true;
                else if (gameObject.distance(this.x, this.y) < gameboard.sight + 1)
                    gameObject.NearlySeen = true;
            }
            is_moveing = true;
        }
        public override void damaged(int n)
        {
            hp -= n;
            if (hp < 0) dead();
        }

        public override void dead()
        {
            
            base.dead();
            gameboard.restart();
            Console.WriteLine("Player Dead! New Game!");

        }
        public void getmoney(int money)
        { this.hp += money; }
        public void moveup()
        {moveto(this.x - 1, this.y); }
        public void movedown()
        { moveto(this.x+ 1, this.y ); }
        public void moveleft()
        { moveto(this.x, this.y-1); }
        public void moveright()
        { moveto(this.x, this.y+1); }
        public void shootup()
        { if (hp > 0) { this.hp--; shoot(-1, 0); } }
        public void shootdown()
        { if (hp > 0) { this.hp--; shoot(1, 0); } }
        public void shootleft()
        {if (hp > 0) { this.hp--; shoot(0, -1); } }
        public void shootright()
        {if (hp > 0) { this.hp--; shoot(0, 1); } }

    }
    class Enemy : MoveableObject
    {
        private string status = "walk";
        static int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
        public Enemy(Gameboard gameboard,int money, int x, int y,int attack=1): base(gameboard,money,x,y)
        {
            this.hp = money;
            this.attack = attack;
            this.isblocked = true;
        }
        public void move()
        {
            if (this.distance(gameboard.player.x, gameboard.player.y) > gameboard.rednoticedist) return;
            TryAttack();

            Node go = DistanceUtility.GetNextStep(gameboard.player, this, gameboard);
            Console.WriteLine("Enemy at [" + x + "," + y + "] to " + go.x + "," + go.y);
            this.x = go.x;
            this.y = go.y;

        }

        public void TryAttack()
        {
            for(int i = 0; i < 4; i++)
            {
                int nx = x + dir[i, 0];
                int ny = y + dir[i, 1];
                if ((nx > 0 && nx < gameboard.GetWidth() - 1 && ny > 0 && ny < gameboard.GetHeight() - 1) 
                    && gameboard.player.x == nx && gameboard.player.y == ny)
                {
                    gameboard.player.damaged(attack);
                    Console.WriteLine("Enemy at [" + x + "," + y + "] Attacked Player");
                }
            }
        }

        public override void dead()
        {
            gameboard.enemies.Remove(this);
        }

        public override Image GetImage()
        {
            switch(hp)
            {
                case 1:
                    return Properties.Resources.Enemy1_01;
                case 2:
                    return Properties.Resources.Enemy2_01;
                case 3:
                    return Properties.Resources.Enemy3_01;
            }
            return Properties.Resources.Enemy1_01;

        }
    }
    class Shop : GameObject
    {
        public int gain;
        
        public Shop(Gameboard gameboard,int money, int x, int y): base(gameboard,x,y)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
            this.isblocked = true;
        }

        public override void damaged(int n)
        {
            //Console.WriteLine("shop get money remain "+hp);
            base.damaged(1);
        }

        virtual public void notice() // notice the player if they are in range
        {
            Console.WriteLine("Shop here at" + x + ", " + y);
        }
    }
    class CoinOnFloor_Shop : Shop
    {
        public CoinOnFloor_Shop(Gameboard gameboard, int money, int x, int y,int gain=6) : base(gameboard,money,x,y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_CoinOnFloor};
        }
        public override void dead()
        {
            gameboard.coinsonfloor += gain;
            base.dead();
        }
        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to gain " + gain + " more coins on each floor");
        }
    }
    class NewRedGen_Shop : Shop
    {
        public NewRedGen_Shop(Gameboard gameboard, int money, int x, int y,int gain=10) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Time };
        }
        public override void dead()
        {
            gameboard.newredgen += gain;
            base.dead();
        }

        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to delay enemy spawn time by " + gain + " steps");
        }
    }
    class RedNoticeDist_Shop : Shop
    {
        public RedNoticeDist_Shop(Gameboard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_NoticeDistance };
        }
        public override void dead()
        {
            gameboard.rednoticedist -= gain;
            base.dead();
        }

        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to reduce enemy notice distance by " + gain + " block");
        }
    }
    class Sight_Shop : Shop
    {
        public Sight_Shop(Gameboard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Sight };
        }
        public override void dead()
        {
            gameboard.sight += gain;
            base.dead();
        }

        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to increase your sight by " + gain + " block");
        }
    }
    class Damage_Shop : Shop
    {
        public Damage_Shop(Gameboard gameboard, int money, int x, int y, int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Damage };
        }
        public override void dead()
        {
            gameboard.player.attack+= gain;
            base.dead();
        }


        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to increase your damage by " + gain + "");
        }
    }
    class MoneyLimit_Shop : Shop
    {
        public MoneyLimit_Shop(Gameboard gameboard, int money, int x, int y, int gain=2) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Heart};
        }
        public override void dead()
        {
            gameboard.player.moneylimit += gain;
            base.dead();
        }
        public override void notice()
        {
            Console.WriteLine("Give me " + hp + " money to increase money limit by " + gain + "");
        }
    }
    class Wall : GameObject
    {
        public Wall(Gameboard gameboard,int x, int y,int hp=1 ): base(gameboard,x,y)
        {
            this.hp = hp;
            this.isblocked = true;
            this.image = new Image[] { Properties.Resources.Wall};
        }
        public override void damaged(int n)
        {
            base.damaged(1);
        }
    }
    class UnbreakableWall : Wall
    {
        public UnbreakableWall(Gameboard gameboard, int x, int y, int hp = 1) : base(gameboard,x,y,hp)
        { }
        public override void damaged(int n)
        { }
    }
    class Gate : Space
    {
        public Gate(Gameboard gameboard,int x,int y) : base(gameboard,x,y)
        { this.image = new Image[] { Properties.Resources.Stair }; }
    }
    class Money : Space
    { 
        public Money(Gameboard gameboard,int x,int y,int money=1): base(gameboard,x,y)
        {
            this.hp = money;
            this.image = new Image[] { Properties.Resources.Coin };
        }
    }
}
