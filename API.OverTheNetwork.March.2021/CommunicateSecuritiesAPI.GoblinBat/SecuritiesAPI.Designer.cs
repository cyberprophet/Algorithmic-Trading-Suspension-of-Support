using System.Windows.Forms;

namespace ShareInvest
{
    partial class SecuritiesAPI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                if (reference != null)
                {
                    reference.Dispose();
                    reference = null;
                }
                if (exit != null)
                {
                    exit.Dispose();
                    exit = null;
                }
                if (Controls.Count > 0)
                {
                    foreach (Control con in Controls)
                        if (con.Controls.Count > 0)
                            con.Dispose();

                    Controls.Clear();
                }
                if (strip != null)
                {
                    strip.Dispose();
                    strip = null;
                }
                if (notifyIcon != null)
                {
                    if (notifyIcon.Icon != null)
                    {
                        notifyIcon.Icon.Dispose();
                        notifyIcon.Icon = null;
                    }
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecuritiesAPI));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.strip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reference = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.ContextMenuStrip = this.strip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Algorithmic Trading";
            // 
            // strip
            // 
            this.strip.AllowMerge = false;
            this.strip.AutoSize = false;
            this.strip.DropShadowEnabled = false;
            this.strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reference,
            this.exit});
            this.strip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.strip.Name = "strip";
            this.strip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.strip.ShowImageMargin = false;
            this.strip.ShowItemToolTips = false;
            this.strip.Size = new System.Drawing.Size(48, 47);
            this.strip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StripItemClicked);
            // 
            // reference
            // 
            this.reference.Name = "reference";
            this.reference.Size = new System.Drawing.Size(73, 22);
            this.reference.Text = "연결";
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(73, 22);
            this.exit.Text = "종료";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // SecuritiesAPI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(239, 0);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SecuritiesAPI";
            this.Opacity = 0.15D;
            this.Text = "Algorithmic Trading";
            this.Resize += new System.EventHandler(this.SecuritiesResize);
            this.strip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        NotifyIcon notifyIcon;
        ContextMenuStrip strip;
        ToolStripMenuItem reference;
        ToolStripMenuItem exit;
        Timer timer;
    }
}