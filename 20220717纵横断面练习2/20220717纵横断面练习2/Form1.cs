using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _20220717纵横断面练习2
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        List<Point> Points = new List<Point>();
        List<Point> Keys = new List<Point>();
        List<Point> ZDM = new List<Point>();
        List<Point> HDM1 = new List<Point>();
        List<Point> HDM2 = new List<Point>();
        double h0, ZDM_D = 0, ZDM_S = 0, dl = 10;
        double HDM1_S = 0, HDM2_S = 0;
        Rectangle rect;
        int flag = 0;
        public Form1()
        {
            InitializeComponent();
            dt.Columns.Add("点名");
            dt.Columns.Add("X");
            dt.Columns.Add("Y");
            dt.Columns.Add("H");
        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 一键计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Operation.ZDMJS(ZDM, Points, Keys, ref ZDM_D, dl);
            ZDM = ZDM.OrderBy(p => p.K).ToList();
            ZDM_S = Operation.G_S(ZDM, h0);
            Operation.HDMJS(HDM1, HDM2, Points, Keys);
            HDM1_S = Operation.G_S(HDM1, h0);
            HDM2_S = Operation.G_S(HDM2, h0);
            MessageBox.Show("计算成功", "提示");
            //绘图部分
            rect = pictureBox1.ClientRectangle;
            Drawing.CreateLM(rect, Points, HDM1, HDM2);
            //生成报告
            richTextBox1.AppendText(
    "---------------------纵横断面计算报告-------------------\n\n" +
    "纵断面信息\n" +
    "---------------------------------------------------------\n" +
    "纵断面面积：" + ZDM_S.ToString("0.000") + "\n" +
    "纵断面全长：" + ZDM_D.ToString("0.000") + "\n" +
    "线路主点:\n" +
    "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in ZDM)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            richTextBox1.AppendText("\n" +
               "横断面1信息\n" +
               "---------------------------------------------------------\n" +
               "横断面面积：" + HDM1_S.ToString("0.000") + "\n" +
               "横断面全长：" + 50 + "\n" +
               "线路主点:\n" +
               "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in HDM1)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            richTextBox1.AppendText("\n" +
               "横断面2信息\n" +
               "---------------------------------------------------------\n" +
               "横断面面积：" + HDM2_S.ToString("0.000") + "\n" +
               "横断面全长：" + 50 + "\n" +
               "线路主点:\n" +
               "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in HDM2)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            tabControl1.SelectedIndex = 2;
            MessageBox.Show("报告生成成功", "提示");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            flag = 1;pictureBox1.Refresh();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            flag = 2; pictureBox1.Refresh();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            flag = 3; pictureBox1.Refresh();
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Points.Clear();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "文本文件|*.txt|所有文件|*.*";
                ofd.Title = "导入数据";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                string path = ofd.FileName;
                using (StreamReader sr = new StreamReader(path))
                {
                    h0 = double.Parse(sr.ReadLine().Split(",".ToCharArray(), StringSplitOptions.None)[1]);
                    Point p1 = new Point(); Point p2 = new Point(); Point p3 = new Point();
                    string[] keynames = sr.ReadLine().Split(",".ToCharArray(), StringSplitOptions.None);
                    p1.name = keynames[0];
                    p2.name = keynames[1];
                    p3.name = keynames[2];
                    Keys.Add(p1); Keys.Add(p2); Keys.Add(p3);
                    string line;
                    sr.ReadLine();
                    while (!String.IsNullOrEmpty(line = sr.ReadLine()))
                    {
                        string[] datas = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        Point p = new Point();
                        p.name = datas[0];
                        p.x = double.Parse(datas[1]);
                        p.y = double.Parse(datas[2]);
                        p.h = double.Parse(datas[3]);
                        Points.Add(p);
                    }
                }
                for (int i = 0; i < Keys.Count; i++)
                {
                    Keys[i] = Points.Find(p => p.name.Equals(Keys[i].name));
                }
                foreach (Point p in Points)
                {
                    dt.Rows.Add(p.name, p.x, p.y, p.h);
                }
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt";
            sfd.Title = "保存报告";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            string path = sfd.FileName;
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(richTextBox1.Text);
            }
        }

        private void 纵断面计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 横断面计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG文件|*.txt";
            string path = "";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                path = sfd.FileName;
            }
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            bmp.Save(path);
        }

        private void 生产示意图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 生成报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText(
    "---------------------纵横断面计算报告-------------------\n\n" +
    "纵断面信息\n" +
    "---------------------------------------------------------\n" +
    "纵断面面积：" + ZDM_S.ToString("0.000") + "\n" +
    "纵断面全长：" + ZDM_D.ToString("0.000") + "\n" +
    "线路主点:\n" +
    "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in ZDM)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            richTextBox1.AppendText("\n" +
               "横断面1信息\n" +
               "---------------------------------------------------------\n" +
               "横断面面积：" + HDM1_S.ToString("0.000") + "\n" +
               "横断面全长：" + 50 + "\n" +
               "线路主点:\n" +
               "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in HDM1)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            richTextBox1.AppendText("\n" +
               "横断面2信息\n" +
               "---------------------------------------------------------\n" +
               "横断面面积：" + HDM2_S.ToString("0.000") + "\n" +
               "横断面全长：" + 50 + "\n" +
               "线路主点:\n" +
               "点名           里程K(m)          X(m)          Y(m)           H(m)\n");
            foreach (Point p in HDM2)
            {
                richTextBox1.AppendText(
                    p.name.PadRight(15) +
                    p.K.ToString("0.000").PadRight(15) +
                    p.x.ToString("0.000").PadRight(15) +
                    p.y.ToString("0.000").PadRight(15) +
                    p.h.ToString("0.000").PadRight(15) + "\n");
            }
            tabControl1.SelectedIndex = 2;
            MessageBox.Show("报告生成成功", "提示");
        }

        private void 保存示意图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_ClientSizeChanged(object sender, EventArgs e)
        {
            if(flag==1)
            {
                rect = pictureBox1.ClientRectangle;
                Drawing.CreateLM(rect, Points, HDM1, HDM2);
                pictureBox1.Refresh();
            }
            if (flag==2)
            {
                rect = pictureBox1.ClientRectangle;
                Drawing.CreateDM(rect, ZDM);
                pictureBox1.Refresh();
            }
            if (flag==3)
            {
                rect = pictureBox1.ClientRectangle;
                Drawing.CreateDM(rect, HDM1);
                pictureBox1.Refresh();
            }
            if (flag == 4)
            {
                rect = pictureBox1.ClientRectangle;
                Drawing.CreateDM(rect, HDM2);
                pictureBox1.Refresh();
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            flag = 4; pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (flag==0)
            {
                MessageBox.Show("请先选择示意图类型", "提示");
                return;
            }
            if (flag==1)
            {
                Drawing.DrawLM_AX(g, rect);
                Drawing.DrawLM(g, rect, Points, Keys, HDM1, HDM2);
            }
            if (flag==2)
            {
                Drawing.CreateDM(rect, ZDM);
                Drawing.DrawDM(g, rect, ZDM);
                Drawing.DrawDM_AX(g, rect, flag);
                
            }
            if (flag == 3)
            {
                Drawing.CreateDM(rect, HDM1);
                Drawing.DrawDM(g, rect, HDM1);
                Drawing.DrawDM_AX(g, rect, flag);
              
            }
            if (flag == 4)
            {
                Drawing.CreateDM(rect, HDM2);
                Drawing.DrawDM(g, rect, HDM2);
                Drawing.DrawDM_AX(g, rect, flag);
                
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Points.Count==0||ZDM.Count==0)
            {
                tabControl1.SelectedIndex = 0;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
                toolStripStatusLabel2.Text = "示意图界面";
            if (tabControl1.SelectedIndex == 2)
                toolStripStatusLabel2.Text = "报告界面";
        }
    }
}
