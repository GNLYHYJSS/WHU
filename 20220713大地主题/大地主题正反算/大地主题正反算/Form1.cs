using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace 大地主题正反算
{
    public partial class Form1 : Form
    {
        bool flag = true;
        DataTable dt = new DataTable();
        List<GCZ> GCZs = new List<GCZ>();
        TQ tq;
         Rectangle rect;
        public Form1()
        {
            InitializeComponent();
            dt.Columns.Add("起点");
            dt.Columns.Add("B1");
            dt.Columns.Add("L1");
            dt.Columns.Add("A1");
            dt.Columns.Add("S");
            dt.Columns.Add("终点");
            dt.Columns.Add("B");
            dt.Columns.Add("L");
            dt.Columns.Add("A");
        }

        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GCZs.Clear(); dt.Clear();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "导入数据";
                ofd.Filter = "文本文件|*.txt|所有文件|*.*";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                tabControl1.SelectedTab = tabPage1;
                if (MessageBox.Show("是正算嘛？(点击是为正算数据，否为反算数据)", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                string path = ofd.FileName;
                #region 读取数据
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    string fg = ",";
                    while (!String.IsNullOrEmpty(line = sr.ReadLine()))
                    {
                        string[] datas = line.Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        int n = datas.Length;
                        switch (n)
                        {
                            case 2:
                                {
                                    TQ temp = new TQ(double.Parse(datas[0]), double.Parse(datas[1]));
                                    tq = temp;
                                    break;
                                }
                            case 6:
                                {
                                    //if (flag && Double.IsNaN(double.Parse(datas[3])))
                                    //{
                                    //    MessageBox.Show("输入数据不是正算数据", "提示");
                                    //    return;
                                    //}
                                    //if (!flag && !Double.IsNaN(double.Parse(datas[3])))
                                    //{
                                    //    MessageBox.Show("输入数据不是反算数据", "提示");
                                    //    return;
                                    //}
                                    if (flag)//正算
                                    {
                                        GCZ gcz = new GCZ();
                                        Point p1 = new Point();
                                        p1.name = datas[0];
                                        p1.B = double.Parse(datas[1]);
                                        p1.L = double.Parse(datas[2]);
                                        p1.A = double.Parse(datas[3]);
                                        gcz.S = double.Parse(datas[4]);
                                        Point p2 = new Point();
                                        p2.name = datas[5];
                                        gcz.p1 = p1;
                                        gcz.p2 = p2;
                                        GCZs.Add(gcz);
                                    }
                                    else
                                    {
                                        GCZ gcz = new GCZ();
                                        Point p1 = new Point();
                                        p1.name = datas[0];
                                        p1.B = double.Parse(datas[1]);
                                        p1.L = double.Parse(datas[2]);
                                        Point p2 = new Point();
                                        p2.name = datas[3];
                                        p2.B = double.Parse(datas[4]);
                                        p2.L = double.Parse(datas[5]);
                                        gcz.p1 = p1;
                                        gcz.p2 = p2;
                                        GCZs.Add(gcz);
                                    }
                                    break;
                                }
                        }
                    }

                }
                #endregion
                foreach (GCZ gcz in GCZs)
                {
                    if (flag)//正算
                    {
                        dt.Rows.Add(
                            gcz.p1.name,
                            Operation.HTD(Operation.DTH(gcz.p1.B)),
                            Operation.HTD(Operation.DTH(gcz.p1.L)),
                            Operation.HTD(Operation.DTH(gcz.p1.A)),
                            gcz.S,
                            gcz.p2.name);
                    }
                    else
                    {
                        dt.Rows.Add(
                            gcz.p1.name,
                            Operation.HTD(Operation.DTH(gcz.p1.B)),
                            Operation.HTD(Operation.DTH(gcz.p1.L)), "", "",
                            gcz.p2.name,
                            Operation.HTD(Operation.DTH(gcz.p2.B)),
                            Operation.HTD(Operation.DTH(gcz.p2.L)));
                    }
                }
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 正算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!flag)
            {
                MessageBox.Show("请进行反算", "提示");
                return;
            }
            dt.Clear();
            foreach (GCZ gcz in GCZs)
            {
                Operation.ZS(gcz, tq);
                dt.Rows.Add(gcz.p1.name, gcz.p1.B, gcz.p1.L, gcz.p1.A, gcz.S, gcz.p2.name, gcz.p2.B, gcz.p2.L, gcz.p2.A);
            }
            dataGridView1.DataSource = dt;
            MessageBox.Show("计算成功","提示");
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, GCZs);

        }

        private void 反算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                MessageBox.Show("请进行正算", "提示");
                return;
            }
            dt.Clear();
            foreach (GCZ gcz in GCZs)
            {
                Operation.FS(gcz, tq);
                dt.Rows.Add(gcz.p1.name, gcz.p1.B, gcz.p1.L, gcz.p1.A, gcz.S, gcz.p2.name, gcz.p2.B, gcz.p2.L, gcz.p2.A);
            }
            dataGridView1.DataSource = dt;
            MessageBox.Show("计算成功", "提示");
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, GCZs);
        }

        private void 生成报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Clear();
                tabControl1.SelectedTab = tabPage3;
                richTextBox1.AppendText(
                    "---------------------------计算结果-----------------------------\n\n" +
                    "点名      维度B（°）    经度L（°） 大地方位角A（°） 大地线S（m）\n");
                foreach (GCZ gcz in GCZs)
                {
                    richTextBox1.AppendText(
                        gcz.p1.name.PadRight(10) +
                        gcz.p1.B.ToString("0.0000").PadRight(15) +
                        gcz.p1.L.ToString("0.0000").PadRight(15) +
                        gcz.p1.A.ToString("0.0000").PadRight(15) +
                        gcz.S.ToString("0.0000").PadRight(15) + "\n" +
                        gcz.p2.name.PadRight(10) +
                        gcz.p2.B.ToString("0.0000").PadRight(15) +
                        gcz.p2.L.ToString("0.0000").PadRight(15) +
                        gcz.p2.A.ToString("0.0000").PadRight(15) +
                        gcz.S.ToString("0.0000").PadRight(15) + "\n\n");
                }
                MessageBox.Show("报告生成成功", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 生成示意图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(GCZs.Count==0)
                {
                    MessageBox.Show("请先导入数据或计算", "提示");
                    return;
                }
                //pictureBox1.Refresh();
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                MessageBox.Show("画图成功", "提示");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void pictureBox1_ClientSizeChanged(object sender, EventArgs e)
        {
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, GCZs);
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            Graphics g = e.Graphics;
            Drawing.AX(g, rect);
            Drawing.Draw(g, rect, GCZs);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Drawing.MouseDown(e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing.MouseUP(e);
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

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 保存图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG文件|*.jpg";
            string path = "";
            if (sfd.ShowDialog()==DialogResult.OK)
            {
                path = sfd.FileName;
            }
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            bitmap.Save(path);
            MessageBox.Show("图像保存成功");
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                toolStripStatusLabel2.Text = "数据页面";
            }
            if (tabControl1.SelectedIndex == 1)
            {
                toolStripStatusLabel2.Text = "画图页面";
            }
            if (tabControl1.SelectedIndex == 2)
            {
                toolStripStatusLabel2.Text = "报告页面";
            }
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("请先生成报告", "提示");
                return;
            }
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
            MessageBox.Show("保存成功");
        }

        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (GCZs.Count == 0)
            {
                tabControl1.SelectedIndex = 0;
            }
            else
            {
                if (flag)
                {
                    if (GCZs[0].p2.A == 0)
                    {
                        tabControl1.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (GCZs[0].S == 0)
                    {
                        tabControl1.SelectedIndex = 0;
                    }
                }
            }
            
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
