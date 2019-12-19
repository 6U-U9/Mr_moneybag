using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Mr_MoneyBag
{
    class GameObject
    {
        public GameBoard gameboard;
        public int x,y,hp;//位置和血量
        public Image[] image;//图像序列
        public bool seen;//是否被看到过（只对静止对象有效）
        public bool isblocked=true;//玩家能否与其重合
        public bool nearlyseen;//是否在视野边缘（只对静止对象有效）
        public int imageindex=0;//当前显示的图像序号

        public GameObject(GameBoard gameboard,int x,int y)
        {
            this.gameboard = gameboard;
            this.x = x;
            this.y = y;
            this.image = new Image[] { Properties.Resources.Space };
        }
        virtual public void Damaged(int n)//受伤
        {
            hp -= n;
            if (hp <= 0) Dead();
        }
        virtual public void Dead()//死亡
        { 
            FreshBoard(x, y, new Space(gameboard,x,y,this.seen,this.nearlyseen));
        }
        virtual public void FreshBoard(int x, int y, GameObject gameobject)//更新对象地图
        {
            gameboard.status[x, y] = gameobject;
        }
        virtual public Image GetImage()//获得当前显示图像
        {
            imageindex++;
            if (imageindex >= image.Length)
                imageindex = 0;
            return image[imageindex];
        }
        virtual public double Distance(int x, int y)//计算对x,y位置的距离
        { return Math.Sqrt((this.x - x)*(this.x - x) + (this.y - y)*(this.y - y)); }
    }
    class Space : GameObject //空地
    {
        public Space(GameBoard gameboard, int x, int y,bool seen=false, bool NearlySeen=false) : base(gameboard,x,y)
        {   this.isblocked = false;
            this.seen = seen;
            this.nearlyseen = NearlySeen;
            this.image =new Image[] { Properties.Resources.Space };
        }
    }
    class MoveableObject : GameObject //会运动的类
    {
        public Image[] movingimage;//运动中的图像序列
        public bool is_moving=false;//是否显示movingimage
        public double x_drawposition, y_drawposition;//绘制位置
        public double animespeed = 0.4;//动画速度
        public int attack;//攻击力
        public MoveableObject(GameBoard gameboard, int money, int x, int y) : base(gameboard,x,y)
        {
            this.hp = money;
            this.image = new Image[] { Properties.Resources.Space };
            this.movingimage = new Image[] { Properties.Resources.Space };
            this.x_drawposition = x;
            this.y_drawposition = y;
        }
        virtual public void Move(int x, int y)
        {
        }
        virtual public void Shoot(int dx, int dy)//射击
        {
            gameboard.bullets.Add(new Bullet(gameboard,this.attack,this.x,this.y,dx,dy));
            /*int cnt = 1;
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
                Console.WriteLine(x + "," + y + "damaged");*/
            is_moving = true;
        }
        virtual public int Moveable(int x, int y)//判断是否能移动到x,y
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
            if (is_moving)
            {   
                imageindex++;
                //更新绘图位置
                if (Math.Abs(x - x_drawposition) > animespeed)
                {
                    if (x - x_drawposition > 0)
                        x_drawposition = Math.Min(x_drawposition + animespeed, x);
                    if (x - x_drawposition < 0)
                        x_drawposition = Math.Max(x_drawposition - animespeed, x);
                }
                else
                    x_drawposition = x;

                if (Math.Abs(y - y_drawposition) > animespeed)
                {
                    if (y - y_drawposition > 0)
                        y_drawposition = Math.Min(y_drawposition + animespeed, y);
                    if (y - y_drawposition < 0)
                        y_drawposition = Math.Max(y_drawposition - animespeed, y);
                }
                else
                    y_drawposition = y;

                if (imageindex >= movingimage.Length && x == x_drawposition && y == y_drawposition)//动画位置与当前位置重合且动画播放完毕则动画停止显示
                { imageindex = 0; is_moving = false; }
                else if (imageindex >= movingimage.Length)
                { imageindex = 0; }
                //Console.WriteLine(is_moving+" "+imageindex+"  "+x_position + "  " + y_position);
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
        public Player(GameBoard gameboard,int money,int x,int y,int moneylimit,int attack=1): base(gameboard,money,x,y)
        {
            this.image = new Image[] { Properties.Resources.Player001 };
            this.movingimage = new Image[] { Properties.Resources.Player001, Properties.Resources.Player002, Properties.Resources.Player003 };
            this.moneylimit = moneylimit;
            this.attack = attack;
        }
        public void InitPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.x_drawposition = x;
            this.y_drawposition = y;
        }
        override public void Move(int x, int y)
        {
            int canmove = Moveable(x, y);
            if (canmove == 0) return;
            if (canmove == -1) { Console.WriteLine("bump into enemy"); gameboard.NextTurn(); return; }//撞到敌人会进行下一回合
            
            Console.WriteLine("Player: " + x + ", " + y);

            this.x = x;
            this.y = y;
            is_moving = true;//开始动画
            
            //判断是否获得Money
            if (gameboard.status[this.x, this.y] is Money&&this.hp<this.moneylimit)
            {
                GetMoney(((Money)gameboard.status[this.x, this.y]).hp);
                FreshBoard(this.x,this.y, new Space(gameboard, this.x, this.y,true));
            }
            
            //更新视野
            foreach (GameObject gameObject in gameboard.status)
            {
                if (gameObject.Distance(this.x, this.y) < gameboard.sight)
                    gameObject.seen = true;
                else if (gameObject.Distance(this.x, this.y) < gameboard.sight + 1)
                    gameObject.nearlyseen = true;
            }
            gameboard.NextTurn();//刷新回合
        }
        public override void Damaged(int n)
        {
            hp -= n;
            if (hp < 0) Dead();
        }
        public override void Dead()
        {
            
            base.Dead();
            gameboard.Restart();
            Console.WriteLine("Player Dead! New Game!");

        }
        public void GetMoney(int money)
        { this.hp += money; }
        public void MoveUp()
        {Move(this.x - 1, this.y); }
        public void MoveDown()
        { Move(this.x+ 1, this.y ); }
        public void MoveLeft()
        { Move(this.x, this.y-1); }
        public void MoveRight()
        { Move(this.x, this.y+1); }
        public void ShootUp()
        { if (hp > 0) { this.hp--; Shoot(-1, 0); } }
        public void ShootDown()
        { if (hp > 0) { this.hp--; Shoot(1, 0); } }
        public void ShootLeft()
        {if (hp > 0) { this.hp--; Shoot(0, -1); } }
        public void ShootRight()
        {if (hp > 0) { this.hp--; Shoot(0, 1); } }

    }
    class Enemy : MoveableObject
    {
        private bool is_attacking = false;//是否即将发起攻击
        static int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
        public Enemy(GameBoard gameboard,int money, int x, int y,int attack=1): base(gameboard,money,x,y)
        {
            this.hp = money;
            this.attack = attack;
            this.isblocked = true;
            this.x_drawposition = x;
            this.y_drawposition = y;
            GetImageList();
        }
        public void Move()//敌人移动
        {
            if (this.Distance(gameboard.player.x, gameboard.player.y) > gameboard.rednoticedist) return;
            Attack();

            Node go = DistanceUtility.GetNextStep(gameboard.player, this, gameboard);
            Console.WriteLine("Enemy at [" + x + "," + y + "] to " + go.x + "," + go.y);
            this.x = go.x;
            this.y = go.y;
            is_moving = true;

        }
        public void ReadytoAction()//攻击前提示
        { is_attacking = true;}
        public void Attack()//向四周一格攻击
        {
            for(int i = 0; i < 4; i++)
            {
                int nx = x + dir[i, 0];
                int ny = y + dir[i, 1];
                if ((nx > 0 && nx < gameboard.GetWidth() - 1 && ny > 0 && ny < gameboard.GetHeight() - 1) 
                    && gameboard.player.x == nx && gameboard.player.y == ny)
                {
                    gameboard.player.Damaged(attack);
                    Console.WriteLine("Enemy at [" + x + "," + y + "] Attacked Player");
                }
            }            
            is_attacking = false;
        }
        public void GetImageList()//依据当前hp获取图片序列
        {
            switch (hp)
            {
                case 1:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy1_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy1_01, Properties.Resources.Enemy1_02, Properties.Resources.Enemy1_03 };
                        break;
                    }
                case 2:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy2_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy2_01, Properties.Resources.Enemy2_02, Properties.Resources.Enemy2_03 };
                        break;
                    }
                case 3:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy3_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy3_01, Properties.Resources.Enemy3_02, Properties.Resources.Enemy3_03 };
                        break;
                    }
                case 4:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy4_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy4_01, Properties.Resources.Enemy4_02, Properties.Resources.Enemy4_03 };
                        break;
                    }
                case 5:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy5_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy1_01, Properties.Resources.Enemy5_02, Properties.Resources.Enemy5_03 };
                        break;
                    }
                case 6:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy6_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy6_01, Properties.Resources.Enemy6_02, Properties.Resources.Enemy6_03 };
                        break;
                    }
                case 7:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy7_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy7_01, Properties.Resources.Enemy7_02, Properties.Resources.Enemy7_03 };
                        break;
                    }
                case 8:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy8_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy8_01, Properties.Resources.Enemy8_02, Properties.Resources.Enemy8_03 };
                        break;
                    }
                case 9:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy9_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy9_01, Properties.Resources.Enemy9_02, Properties.Resources.Enemy9_03 };
                        break;
                    }
                default:
                    {
                        this.image = new Image[] { Properties.Resources.Enemy1_01 };
                        this.movingimage = new Image[] { Properties.Resources.Enemy1_01, Properties.Resources.Enemy1_02, Properties.Resources.Enemy1_03 };
                        break;
                    }
            }
        }
        public override Image GetImage()
        {
            if (!is_moving&&is_attacking)
            {
                imageindex++;
                if (imageindex >= movingimage.Length)
                    imageindex = 0;
                //Console.WriteLine("Attack");
                return movingimage[imageindex];
            }
            else
                return base.GetImage();
        }
        public void FreshDrawPosition()//在玩家视野外的时候刷新显示位置
        {
            x_drawposition = x;
            y_drawposition = y;
        }
        public override void Damaged(int n)
        {
            hp -= n;
            if (hp <= 0) Dead();
            else GetImageList();//刷新图像序列
            Console.WriteLine("receive damage");
        }
        public override void Dead()
        {
            gameboard.enemies.Remove(this);
        }

    }
    class Shop : GameObject //商店基类
    {
        public int gain; //购买收益
        
        public Shop(GameBoard gameboard,int money, int x, int y): base(gameboard,x,y)
        {
            this.hp = money;
            this.x = x;
            this.y = y;
            this.isblocked = true;
        }

        public override void Damaged(int n)
        {
            //Console.WriteLine("shop get money remain "+hp);
            base.Damaged(1);
        }

        virtual public void notice() // notice the player if they are in range
        {
            Console.WriteLine("Shop here at" + x + ", " + y);
        }
    }
    class CoinOnFloor_Shop : Shop //增加每关生成的金币数量
    {
        public CoinOnFloor_Shop(GameBoard gameboard, int money, int x, int y,int gain=6) : base(gameboard,money,x,y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_CoinOnFloor};
        }
        public override void Dead()
        {
            gameboard.coinsonfloor += gain;
            base.Dead();
        }
        public override void notice()
        {
            string msg = "Give me " + hp + " money to gain " + gain + " more coins on each floor";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class NewRedGen_Shop : Shop //增加敌人刷新的间隔
    {
        public NewRedGen_Shop(GameBoard gameboard, int money, int x, int y,int gain=10) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Time };
        }
        public override void Dead()
        {
            gameboard.newredgen += gain;
            base.Dead();
        }

        public override void notice()
        {
            string msg = "Give me " + hp + " money to delay enemy spawn time by " + gain + " steps";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class RedNoticeDist_Shop : Shop //增加敌人发现玩家的距离
    {
        public RedNoticeDist_Shop(GameBoard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_NoticeDistance };
        }
        public override void Dead()
        {
            gameboard.rednoticedist -= gain;
            base.Dead();
        }

        public override void notice()
        {
            string msg = "Give me " + hp + " money to reduce enemy notice distance by " + gain + " block";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class Sight_Shop : Shop //增加视野
    {
        public Sight_Shop(GameBoard gameboard, int money, int x, int y,int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Sight };
        }
        public override void Dead()
        {
            gameboard.sight += gain;
            base.Dead();
        }

        public override void notice()
        {
            string msg = "Give me " + hp + " money to increase your sight by " + gain + " block";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class Damage_Shop : Shop //增加子弹伤害
    {
        public Damage_Shop(GameBoard gameboard, int money, int x, int y, int gain=1) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Damage };
        }
        public override void Dead()
        {
            gameboard.player.attack+= gain;
            base.Dead();
        }


        public override void notice()
        {
            string msg = "Give me " + hp + " money to increase your damage by " + gain + "";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class MoneyLimit_Shop : Shop //增加金币携带上限
    {
        public MoneyLimit_Shop(GameBoard gameboard, int money, int x, int y, int gain=2) : base(gameboard, money, x, y)
        {
            this.gain = gain;
            this.image = new Image[] { Properties.Resources.Shop_Heart};
        }
        public override void Dead()
        {
            gameboard.player.moneylimit += gain;
            base.Dead();
        }
        public override void notice()
        {
            string msg = "Give me " + hp + " money to increase money limit by " + gain + "";
            Console.WriteLine(msg);
            gameboard.AddNotice(msg, GetType());
        }
    }
    class Wall : GameObject //可破坏的墙
    {
        public Wall(GameBoard gameboard,int x, int y,int hp=1 ): base(gameboard,x,y)
        {
            this.hp = hp;
            this.isblocked = true;
            this.image = new Image[] { Properties.Resources.Wall};
        }
        public override void Damaged(int n)
        {
            base.Damaged(1);
        }
    }
    class UnbreakableWall : Wall //不可破坏的墙
    {
        public UnbreakableWall(GameBoard gameboard, int x, int y, int hp = 1) : base(gameboard,x,y,hp)
        { }
        public override void Damaged(int n)
        { }
    }
    class Gate : Space //门(楼梯) 下一关的入口
    {
        public Gate(GameBoard gameboard,int x,int y) : base(gameboard,x,y)
        { this.image = new Image[] { Properties.Resources.Stair }; }
    }
    class Money : Space //钱
    { 
        public Money(GameBoard gameboard,int x,int y,int money=1): base(gameboard,x,y)
        {
            this.hp = money;
            this.image = new Image[] { Properties.Resources.Coin };
        }
    }
    class Bullet : MoveableObject //子弹 
    {
        //攻击力即为hp
        private double speed;//动画速度（运动速度）
        private double range;//最大距离
        private int dx, dy;//方向
        private int start_x, start_y;//起始位置
        public Bullet(GameBoard gameboard, int money, int x, int y, int dx, int dy, double speed = 0.7,double range=10.0) : base(gameboard, money, x, y)
        {
            this.dx = dx;
            this.dy = dy;
            this.speed = speed;
            this.image = new Image[] { Properties.Resources.Coin_Shoot };
            this.x_drawposition = x;
            this.y_drawposition = y;
            this.start_x = x;
            this.start_y = y;
            this.range = range;
        }
        public void Move()
        {
            x_drawposition += dx * speed;
            y_drawposition += dy * speed;
            if (dx > 0)
            { x = (int)Math.Floor(x_drawposition); }
            else if (dx < 0)
            { x = (int)Math.Ceiling(x_drawposition); }
            if (dy > 0)
            { y = (int)Math.Floor(y_drawposition); }
            else if (dy < 0)
            { y = (int)Math.Ceiling(y_drawposition); }

            if (gameboard.HasEnemy(x, y))
            { gameboard.GetEnemy(x, y).Damaged(hp); gameboard.bullets.Remove(this); Console.WriteLine("deal demage" + hp); }
            else if (!(gameboard.status[x, y] is Space))
            { gameboard.status[x, y].Damaged(hp); gameboard.bullets.Remove(this); }
            else if (Distance(start_x, start_y) > range)
            { gameboard.bullets.Remove(this); Console.WriteLine(" out of range"+Distance(start_x, start_y)); }
            //Console.WriteLine("B  " + x + " " + y);
        }
    }
}
