namespace Mr_MoneyBag
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lbllevel = new System.Windows.Forms.Label();
            this.moneycount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbllevel
            // 
            this.lbllevel.AutoSize = true;
            this.lbllevel.Location = new System.Drawing.Point(833, 461);
            this.lbllevel.Name = "lbllevel";
            this.lbllevel.Size = new System.Drawing.Size(17, 12);
            this.lbllevel.TabIndex = 0;
            this.lbllevel.Text = "01";
            // 
            // moneycount
            // 
            this.moneycount.AutoSize = true;
            this.moneycount.Font = new System.Drawing.Font("宋体", 20F);
            this.moneycount.ForeColor = System.Drawing.Color.Yellow;
            this.moneycount.Location = new System.Drawing.Point(370, 227);
            this.moneycount.Name = "moneycount";
            this.moneycount.Size = new System.Drawing.Size(96, 27);
            this.moneycount.TabIndex = 1;
            this.moneycount.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(884, 501);
            this.Controls.Add(this.moneycount);
            this.Controls.Add(this.lbllevel);
            this.Name = "Form1";
            this.Text = "Mr. Moneybag";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbllevel;
        private System.Windows.Forms.Label moneycount;
    }
}

