using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _20220718Grid体积计算
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Point> Points = new List<Point>();//散点
        Stack<Point> S = new Stack<Point>();//凸包点
        GW gw = new GW();//格网
        double h0, L,V;
        Rectangle rect;

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 计算凸包点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //计算部分

            //绘图部分
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, S);
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        private void 生成格网ToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            //计算部分

            //绘图部分
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, S);
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        private void 计算体积ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //计算部分

            //报告部分
            
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG文件|*.txt";
            if (sfd.ShowDialog()!=DialogResult.OK)
            {
                return;
            }
            string path = sfd.FileName;
            sfd.Title = "示意图保存";
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            bmp.Save(path);
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 一键计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //计算部分

            //绘图部分
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, S);
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (S.Count==0)
            {
                tabControl1.SelectedIndex = 0;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing.MouseUp(e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Drawing.Mousedown(e);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Drawing.MouseMove(e);
            pictureBox1.Refresh();
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Drawing.MouseDelta(e);
            pictureBox1.Refresh();
        }

        private void pictureBox1_ClientSizeChanged(object sender, EventArgs e)
        {
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, S);
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if(gw.Cs.Count==0)
            {
                Drawing.DrawAX(g, rect);
                Drawing.DrawTB(g, rect, S, Points);
                Drawing.DrawBT(g, rect);
            }
            else
            {
                Drawing.DrawTB(g, rect, S, Points);
                Drawing.DrawBT(g, rect);
                Drawing.DrawGW(g, rect, gw);
            }
        }

       
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
