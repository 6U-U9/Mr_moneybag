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
        public const int blocksize = 32;//�����������ش�С
        public static int y = 30, x = 17;//������ʾ��С
        public const double animespeed = 0.4; //�����˶��ٶ�
        public const int framespeed = 30;//֡Ƶ
        public PictureBox map = new PictureBox();
        GameBoard gameboard = new GameBoard();
        private bool is_space_down = false;//�ո��״̬
        private bool is_tab_down = false;//Tab״̬
        private bool is_showing_topnotice = false;//������ʾ��Ϣ״̬
        private const int notice_starty=-40, notice_endy=10;
        private const int notice_shopstopframe = 10,notice_enemystopframe = 5;
        private float noticespeed = 10.0F;
        private float notice_frame = 0;
        private float notice_x,notice_y;
        private bool arrow_key_locked = false;//�����״̬
        private double x_position, y_position;//��¼����������λ��

        Font font = new Font("UD Digi Kyokasho NK-B", 24);
        Brush money = new SolidBrush(Color.FromArgb(251, 242, 54));
        Brush level = new SolidBrush(Color.FromArgb(255, 40, 40));
        Brush moneylimit = new SolidBrush(Color.FromArgb(209, 163, 164));
        Brush damage = new SolidBrush(Color.FromArgb(63, 138, 110));
        Brush sight = new SolidBrush(Color.FromArgb(251, 192, 100));
        Brush rednoticedist = new SolidBrush(Color.FromArgb(158, 123, 103));
        Brush newredgen = new SolidBrush(Color.FromArgb(118, 184, 186));
        Brush coinsonfloor = new SolidBrush(Color.FromArgb(151, 161, 173));
        Brush diamond = new SolidBrush(Color.FromArgb(94, 182, 95));


        public Form1()
        {
            
            ((System.ComponentModel.ISupportInitialize)(map)).BeginInit();
            map.Location = new System.Drawing.Point(0, 0);
            map.Size = new System.Drawing.Size(blocksize * y, blocksize * x);
            this.Controls.Add(map);
            ((System.ComponentModel.ISupportInitialize)(map)).EndInit();
            map.BringToFront();

            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(blocksize * y, blocksize * x);
            DoubleBuffered = true;
            InitNumbers();
            RefreshBoard();//ˢ�»���
            timerFresh.Interval = framespeed; //timerFresh ���ƻ������
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
        }//���㶯������λ�õ�����λ��

        public void InitNumbers() //��ʼ����ʾ����λ��
        {   
            (x_position, y_position) = GetXYst();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Space)
                is_space_down = true;
            if (e.KeyCode == Keys.Tab)
                is_tab_down = true;
            if (!arrow_key_locked)
            {
                if (is_space_down == true)
                    switch (e.KeyCode)
                    {
                        case Keys.W:
                        case Keys.Up:
                            gameboard.player.ShootUp();
                            arrow_key_locked = true;
                            break;
                        case Keys.A:
                        case Keys.Left:
                            gameboard.player.ShootLeft();
                            arrow_key_locked = true;
                            break;
                        case Keys.S:
                        case Keys.Down:
                            gameboard.player.ShootDown();
                            arrow_key_locked = true;
                            break;
                        case Keys.D:
                        case Keys.Right:
                            gameboard.player.ShootRight();
                            arrow_key_locked = true;
                            break;
                    }
                else if (is_space_down == false&&is_tab_down==false)
                    switch (e.KeyCode)
                    {
                        case Keys.W:
                        case Keys.Up:
                            gameboard.player.MoveUp();
                            arrow_key_locked = true;
                            break;
                        case Keys.A:
                        case Keys.Left:
                            gameboard.player.MoveLeft();
                            arrow_key_locked = true;
                            break;
                        case Keys.S:
                        case Keys.Down:
                            gameboard.player.MoveDown();
                            arrow_key_locked = true;
                            break;
                        case Keys.D:
                        case Keys.Right:
                            gameboard.player.MoveRight();
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
            if (e.KeyCode == Keys.Tab)
                is_tab_down = false;
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
        public void RefreshBoard()//ÿ֡��ˢ��
        {
            if (gameboard.is_newlevel)//�µ�һ��ˢ�³�ʼ����λ��
                InitNumbers();
            double x_st, y_st;
            (x_st, y_st) = GetXYst();
            //Console.WriteLine(x_st+" "+y_st);

            //���¶���λ��
            Image image;
            if (Math.Abs(x_st - x_position) > animespeed)
            {
                if (x_st - x_position > 0)
                    x_position = Math.Min(x_position + animespeed, x_st);
                if (x_st - x_position < 0)
                    x_position = Math.Max(x_position - animespeed, x_st);
            }
            else
                x_position = x_st;
            if (Math.Abs(y_st - y_position) > animespeed)
            {
                if (y_st - y_position > 0)
                    y_position = Math.Min(y_position + animespeed, y_st);
                if (y_st - y_position < 0)
                    y_position = Math.Max(y_position - animespeed, y_st);
            }
            else
                y_position = y_st;

            gameboard.IfNextLevel();//�ж��Ƿ�ﵽ��һ�أ�Ϊ����ҵ���¥�ݵĶ���Ч���ڴ��жϣ�
            gameboard.FreshBullets();//�ӵ���ʵʱ�������ڴ˸���
            image = GetFullImage(gameboard, x_position, y_position, Form1.x, Form1.y);
            map.Image = image;

        }
        private Image GetShowImage(GameBoard gameboard, int x, int y)//�����ķ�����û��ʵ�ֶ���֮ǰʹ�ã�
        {
            GameObject gameObject = gameboard.status[x, y];
            double dist = gameObject.Distance(gameboard.player.x, gameboard.player.y);
            if (dist <= gameboard.sight)
            {

                if ((x + y) % 2 == 1)
                {
                    if (gameboard.player.x == x && gameboard.player.y == y)
                        return UniteImage(Properties.Resources.Back001, gameboard.player.GetImage());
                    foreach (Enemy enemy in gameboard.enemies)
                    {
                        if (enemy.x == x && enemy.y == y) return UniteImage(Properties.Resources.Back001, enemy.GetImage());
                    }
                    return UniteImage(Properties.Resources.Back001, gameboard.status[x, y].GetImage());
                }
                else
                {
                    if (gameboard.player.x == x && gameboard.player.y == y)
                        return UniteImage(Properties.Resources.Back002, gameboard.player.GetImage());
                    foreach (Enemy enemy in gameboard.enemies)
                    {
                        if (enemy.x == x && enemy.y == y) return UniteImage(Properties.Resources.Back002, enemy.GetImage());
                    }
                    return UniteImage(Properties.Resources.Back002, gameboard.status[x, y].GetImage());
                }
            }
            /*if (gameObject is Space)
                Console.WriteLine(gameObject.seen);*/
            if (gameObject.seen == true)
                return UniteImage(Properties.Resources.Seen, gameboard.status[x, y].GetImage());
            if (gameObject.nearlyseen == true)
                return Properties.Resources.Nearlyseen;
            return Properties.Resources.Unseen;
        }
        private Image GetUnmoveableImage(GameBoard gameboard, int x, int y)//��ȡx,yλ�õĸ��ӵ�ͼ��
        {
            GameObject gameObject = gameboard.status[x, y];
            //if (gameObject is Gate) Console.WriteLine("G" + x + " " + y);
            double dist = gameObject.Distance(gameboard.player.x, gameboard.player.y);
            if (dist <= gameboard.sight)//��Ұ��
            {
                if ((x + y) % 2 == 1)
                    return UniteImage(Properties.Resources.Back001, gameboard.status[x, y].GetImage());
                else
                    return UniteImage(Properties.Resources.Back002, gameboard.status[x, y].GetImage());
            }
            if (gameObject.seen == true)//��������
                return UniteImage(Properties.Resources.Seen, gameboard.status[x, y].GetImage());
            if (gameObject.nearlyseen == true)//��Ұ��Ե
                return Properties.Resources.Nearlyseen;
            return Properties.Resources.Unseen;//��Ұ��
        }
        private Image GetTopNotice(GameBoard gameboard, int x_len, int y_len)
        {
            if (gameboard.noticelist.Count == 0)
                return new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize); ;
            System.Drawing.Image img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            var (notice, type) = gameboard.noticelist[0];
            int notice_stopframe = notice_enemystopframe;
            if (type == typeof(Shop))
                notice_stopframe = notice_shopstopframe;
            Brush brush = Brushes.Red;
            if (type == typeof(MoneyLimit_Shop))
                brush = moneylimit;
            else if (type == typeof(Damage_Shop))
                brush = damage;
            else if (type == typeof(Sight_Shop))
                brush = sight;
            else if (type == typeof(RedNoticeDist_Shop))
                brush = rednoticedist;
            else if (type == typeof(NewRedGen_Shop))
                brush = newredgen;
            else if (type == typeof(CoinOnFloor_Shop))
                brush = coinsonfloor;
            if (is_showing_topnotice)
            {
                if (notice_y == notice_endy && notice_frame < notice_stopframe)
                    notice_frame++;
                else if (notice_y == notice_endy && notice_frame >= notice_stopframe)
                {
                    notice_frame = 0;
                    notice_y += noticespeed;
                }
                else
                    notice_y += noticespeed;
                if (notice_y > notice_endy)
                {
                    notice_y = notice_endy;
                    noticespeed = -noticespeed;
                }
                else if (notice_y < notice_starty)
                {
                    notice_y = notice_starty;
                    noticespeed = -noticespeed;
                    is_showing_topnotice = false;
                    gameboard.noticelist.RemoveAt(0);
                }
            }
            else
            {
                notice_y = notice_starty;
                is_showing_topnotice = true;
                SizeF sizeF = g.MeasureString(notice, font);
                notice_x = (y_len * blocksize - sizeF.Width) / 2;
            }
            g.DrawString(notice, font, brush, notice_x, notice_y);
            return img;
        }
        private Image GetNotice(GameBoard gameboard, int x_len, int y_len)
        {
            System.Drawing.Image img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            int stringheight = 40;
            int nums_x = 15, nums_y = x_len * blocksize - 45,tab_x=45;
            int money_width = 26, money_height = 4;
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            //Font font = new Font("Bauhaus 93", 24);
            
            g.DrawString(gameboard.level.ToString("00"), font, level, y_len * blocksize - 60, x_len * blocksize - 45);
            if (is_tab_down) g.DrawString("Floor - ", font, level, y_len * blocksize - 180, x_len * blocksize - 45);
            g.DrawString(gameboard.player.hp.ToString("00"), font, money, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Many Coins You Have", font, money, nums_x + tab_x, nums_y);
            nums_y += -3;
            for (int i = 0; i < gameboard.player.hp; i++)
            {
                nums_y -= money_height+1;
                g.FillRectangle(money, nums_x+14, nums_y, money_width, money_height); 
            }
            for (int i = 0; i < gameboard.player.moneylimit-gameboard.player.hp; i++)
            {
                nums_y -= money_height + 1;
            }
            nums_y -= 6;
            g.FillRectangle(moneylimit, nums_x+10, nums_y, 34, 2);
            nums_y -= 35;
            g.DrawString(gameboard.player.moneylimit.ToString("00"), font, moneylimit, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Many Coins You Can Carry", font, moneylimit, nums_x + tab_x, nums_y);
            nums_y -= stringheight;
            g.DrawString(gameboard.player.attack.ToString("00"), font, damage, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Much Damage Each Coin Does", font, damage, nums_x + tab_x, nums_y);
            nums_y -= stringheight;
            g.DrawString(((int)(Math.Ceiling(gameboard.sight))).ToString("00"), font, sight, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Far Away You Can See", font, sight, nums_x + tab_x, nums_y);
            nums_y -= stringheight;
            g.DrawString(gameboard.rednoticedist.ToString("00"), font, rednoticedist, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Far Away Red Notice You", font, rednoticedist, nums_x + tab_x, nums_y);
            nums_y -= stringheight;
            g.DrawString(gameboard.newredgen.ToString("00"), font, newredgen, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Often New Red Arrives", font, newredgen, nums_x + tab_x, nums_y);
            nums_y -= stringheight;
            g.DrawString(gameboard.coinsonfloor.ToString("00"), font, coinsonfloor, nums_x, nums_y);
            if (is_tab_down) g.DrawString(" - How Much Coins On Each Floor", font, coinsonfloor, nums_x + tab_x, nums_y);
            //g.DrawString(gameboard.diamonds.ToString("00"), font, brush, nums_x, nums_y);
            //Console.WriteLine("diaoy");
            return img;
        }
        private Image GetFullImage(GameBoard gameboard, double x_st, double y_st, int x_len, int y_len)//��ȡ��ʾ�Ļ���
        {
            System.Drawing.Image img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
            System.Drawing.Image notice = GetNotice(gameboard, x_len, y_len);
            //��û��ƿ�ʼ�ĸ��Ӻͻ��ƴ�С
            //������˶��У����Ȼ��ƴ�һȦ��ͼ��Ȼ����ʾ��һ����
            int x, y;
            x = Math.Max((int)Math.Floor(x_st), 0);
            y = Math.Max((int)Math.Floor(y_st), 0);
            if (x_st > x)
                x_len++;
            if (y_st > y)
                y_len++;
            //Console.WriteLine(x_st + "  " + x + "  " + y_st + "  " + y);
            //���ƾ�ֹ����
            System.Drawing.Image whole_img = new System.Drawing.Bitmap(y_len * blocksize, x_len * blocksize);
            System.Drawing.Graphics whole_g = System.Drawing.Graphics.FromImage(whole_img);
            for (int i = x; i < x + x_len; i++)
                for (int j = y; j < y + y_len; j++)
                {
                    whole_g.DrawImage(GetUnmoveableImage(gameboard, i, j), blocksize * (j - y), blocksize * (i - x), blocksize, blocksize);

                }
            //���ƻ��棨�˶�����ȡ��һ���֣�
            whole_g.DrawImage(gameboard.player.GetImage(), (int)(blocksize * (gameboard.player.y_drawposition - y)), (int)(blocksize * (gameboard.player.x_drawposition - x)), blocksize, blocksize);
            //���Ƶ���
            foreach (Enemy enemy in gameboard.enemies)
                if (enemy.Distance(gameboard.player.x, gameboard.player.y) <= gameboard.sight)
                    whole_g.DrawImage(enemy.GetImage(), (int)(blocksize * (enemy.y_drawposition - y)), (int)(blocksize * (enemy.x_drawposition - x)), blocksize, blocksize);
                else
                    enemy.FreshDrawPosition();
            //�����ӵ�
            foreach (Bullet bullet in gameboard.bullets)
                if (gameboard.player.Distance((int)bullet.x, (int)bullet.y) <= gameboard.sight)
                    whole_g.DrawImage(bullet.GetImage(), (int)(blocksize * (bullet.y_drawposition - y)), (int)(blocksize * (bullet.x_drawposition - x)), blocksize, blocksize);
            g.DrawImage(whole_img, (int)(-(y_st - y) * blocksize), (int)(-(x_st - x) * blocksize));
            g.DrawImage(notice,0,0);
            g.DrawImage(GetTopNotice(gameboard,x_len,y_len), 0, 0);
            return img;
        }

        private void timerFresh_Tick(object sender, EventArgs e)
        {
            RefreshBoard();
            //Console.WriteLine("Tick");
        }

        protected override bool ProcessDialogKey(Keys keycode)//��÷�����Ϳո����
        {
            switch (keycode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Space:
                case Keys.Tab:
                    return false;
                default:
                    return base.ProcessDialogKey(keycode);
            }
        }

        public Image UniteImage(Image img1, Image img2, int width = blocksize, int height = blocksize)//��õ��ӵ�ͼƬ
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
