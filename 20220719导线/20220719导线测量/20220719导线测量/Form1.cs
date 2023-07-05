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

namespace _20220719导线测量
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dt.Columns.Add("点名");
            dt.Columns.Add("观测角（°′″）");
            dt.Columns.Add("坐标方位角（°′″）");
            dt.Columns.Add("边长（m）");
            dt.Columns.Add("dx（m）");
            dt.Columns.Add("dy（m）");
            dt.Columns.Add("x（m）");
            dt.Columns.Add("y（m）");
        }

        DataTable dt = new DataTable();
        List<Point> Known_Ps = new List<Point>(4);//已知点坐标
        List<CZ> CZs = new List<CZ>();
        List<double> As = new List<double>();
        double fbaita = 0;
        double fdx = 0, fdy = 0, fs = 0, sum_s = 0;
        Rectangle rect;

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Known_Ps.Count==0||CZs.Count==0)
            {
                tabControl1.SelectedIndex = 0;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                toolStripStatusLabel2.Text = "示意图界面";
            }
            if (tabControl1.SelectedIndex == 2)
            {
                toolStripStatusLabel2.Text = "报告界面";
            }
        }

        private void 一键计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Known_Ps.Count == 0 || CZs.Count == 0)
                {
                    MessageBox.Show("请先输入数据！", "提示");
                    return;
                }
                //计算部分
                if (Operation.JD_Adjust(CZs, As, Known_Ps, ref fbaita) == 1)
                {
                    if (Operation.XY_Adjust(CZs, As, Known_Ps, ref fdx, ref fdy, ref fs, ref sum_s) == 1)
                    {
                        Operation.ZBJS(CZs, Known_Ps, sum_s, fdx, fdy);
                        dt.Clear();
                        JD A1 = Operation.HTD(Operation.G_A(Known_Ps[0], Known_Ps[1]));
                        dt.Rows.Add(Known_Ps[0].name, "", "", "", "", "", Known_Ps[0].X, Known_Ps[0].Y);
                        dt.Rows.Add("", "", A1.d.ToString() + "°" + A1.f.ToString() + "″" + A1.m.ToString("0.00") + "″");
                        CZ czf = CZs[0];
                        JD A2 = Operation.HTD(As[1]);
                        dt.Rows.Add(Known_Ps[1].name, czf.β.d.ToString() + "°" + czf.β.f.ToString() + "″" + czf.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[1].X, Known_Ps[1].Y);
                        dt.Rows.Add("", "", A2.d.ToString() + "°" + A2.f.ToString() + "″" + A2.m.ToString("0.00") + "″", czf.GCZs[2].value, czf.dx, czf.dy);
                        for (int i = 1; i < CZs.Count - 1; i++)//计算转角
                        {
                            CZ cz = CZs[i];
                            JD Ai = Operation.HTD(As[i + 1]);
                            dt.Rows.Add(cz.P.name, cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″", "", "", "", "", cz.P.X, cz.P.Y);
                            dt.Rows.Add("", "", Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″", cz.GCZs[2].value, cz.dx, cz.dy);
                        }
                        CZ czl = CZs[CZs.Count - 1];
                        JD A = Operation.HTD(Operation.G_A(Known_Ps[2], Known_Ps[3]));
                        dt.Rows.Add(czl.P.name, czl.β.d.ToString() + "°" + czl.β.f.ToString() + "″" + czl.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[2].X, Known_Ps[2].Y);
                        dt.Rows.Add("", "", A.d.ToString() + "°" + A.f.ToString() + "″" + A.m.ToString("0.00") + "″");
                        dt.Rows.Add(Known_Ps[3].name, "", "", "", "", "", Known_Ps[3].X, Known_Ps[3].Y);
                        MessageBox.Show("坐标计算成功！", "提示");
                    }
                    else
                    {
                        MessageBox.Show("超限！", "提示");
                    }
                }
                else
                {
                    MessageBox.Show("超限！", "提示");
                }
                //绘图部分
                rect = pictureBox1.ClientRectangle;
                Drawing.Create(rect, CZs, Known_Ps);
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                MessageBox.Show("画图成功！", "提示");
                richTextBox1.Clear();
                richTextBox1.AppendText(
                    "测试数据计算结果\n" +
                    "--------------限差要求-----------------\n" +
                    "角度闭合差限差：" + (40 * Math.Sqrt(CZs.Count)).ToString("0.000") + "\n" +
                    "导线全长相对闭合差限差：0.0002000\n\n");
                richTextBox1.AppendText(
                    "-------------导线基本信息----------------\n" +
                    "测站数：" + CZs.Count + "\n" +
                    "导线全长：" + sum_s.ToString("0.000000") + "\n" +
                    "角度闭合差：" + (fbaita * 3600).ToString("0.000") + "\n" +
                    "Y坐标闭合差：" + fdy.ToString("0.0000") + "\n" +
                    "X坐标闭合差：" + fdx.ToString("0.0000") + "\n" +
                    "导线全长相对闭合差：" + fs.ToString("0.0000") + "\n\n" +
                    "-------------测站点坐标-----------\n" +
                    "测站名            X坐标            Y坐标\n");
                foreach (CZ cz in CZs)
                {
                    richTextBox1.AppendText(
                        cz.P.name.PadRight(15) +
                        cz.P.X.ToString("0.0000").PadRight(15) +
                        cz.P.Y.ToString("0.0000").PadRight(15) + "\n");
                }
                richTextBox1.AppendText(
                    "\n-------------角度数据----------------\n" +
                    "测站名            观测角                 方位角  \n");
                for (int i = 0; i < CZs.Count; i++)
                {
                    CZ cz = CZs[i];
                    JD Ai = Operation.HTD(As[i]);
                    richTextBox1.AppendText((Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″").PadLeft(50) + "\n");
                    richTextBox1.AppendText(
                        cz.P.name.PadRight(15) +
                        (cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″").PadRight(20) + "\n");
                }
                JD Al = Operation.HTD(As[As.Count - 1]);
                richTextBox1.AppendText((Al.d.ToString() + "°" + Al.f.ToString() + "″" + Al.m.ToString("0.00") + "″").PadLeft(50) + "\n");
                tabControl1.SelectedTab = tabControl1.TabPages[2];
                MessageBox.Show("报告生成成功！", "提示");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Drawing.DrawAX(g, rect);
            Drawing.DrawKP(g, rect, Known_Ps);
            Drawing.DrawCZ(g, rect, CZs);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing.MouseUp(e);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Drawing.MouseDown(e);
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

        private void 计算近似方位角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Known_Ps.Count == 0 || CZs.Count == 0)
                {
                    MessageBox.Show("请先输入数据！", "提示");
                    return;
                }
                if (Operation.JD_Adjust(CZs, As, Known_Ps, ref fbaita) == 1)
                {
                    dt.Clear();
                    JD A1 = Operation.HTD(Operation.G_A(Known_Ps[0], Known_Ps[1]));
                    dt.Rows.Add(Known_Ps[0].name, "", "", "", "", "", Known_Ps[0].X, Known_Ps[0].Y);
                    dt.Rows.Add("", "", A1.d.ToString() + "°" + A1.f.ToString() + "″" + A1.m.ToString("0.00") + "″");
                    CZ czf = CZs[0];
                    JD A2 = Operation.HTD(As[1]);
                    dt.Rows.Add(Known_Ps[1].name, czf.β.d.ToString() + "°" + czf.β.f.ToString() + "″" + czf.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[1].X, Known_Ps[1].Y);
                    dt.Rows.Add("", "", A2.d.ToString() + "°" + A2.f.ToString() + "″" + A2.m.ToString("0.00") + "″", czf.GCZs[2].value);
                    for (int i = 1; i < CZs.Count - 1; i++)//计算转角
                    {
                        CZ cz = CZs[i];
                        JD Ai = Operation.HTD(As[i + 1]);
                        dt.Rows.Add(cz.P.name, cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″");
                        dt.Rows.Add("", "", Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″", cz.GCZs[2].value);
                    }
                    CZ czl = CZs[CZs.Count - 1];
                    JD A = Operation.HTD(Operation.G_A(Known_Ps[2], Known_Ps[3]));
                    dt.Rows.Add(czl.P.name, czl.β.d.ToString() + "°" + czl.β.f.ToString() + "″" + czl.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[2].X, Known_Ps[2].Y);
                    dt.Rows.Add("", "", A.d.ToString() + "°" + A.f.ToString() + "″" + A.m.ToString("0.00") + "″");
                    dt.Rows.Add(Known_Ps[3].name, "", "", "", "", "", Known_Ps[3].X, Known_Ps[3].Y);
                    MessageBox.Show("角度近似平差成功！", "提示");
                    dataGridView1.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("超限！", "提示");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           


        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dt.Clear(); CZs.Clear(); Known_Ps.Clear();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "导入数据";
                ofd.Filter = "文本文件|*.txt";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                string path = ofd.FileName;
                #region 读取数据
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    string fg = ",";
                    sr.ReadLine();//误差项
                    while (!String.IsNullOrEmpty(line = sr.ReadLine()))
                    {
                        string[] datas = line.Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        int n = datas.Length;
                        switch (n)
                        {
                            case 3:
                                {
                                    Point p = new Point();
                                    p.name = datas[0];
                                    p.X = double.Parse(datas[1]);
                                    p.Y = double.Parse(datas[2]);
                                    Known_Ps.Add(p);
                                    break;
                                }
                            case 1:
                                {
                                    CZ cz;
                                    if (CZs.FindIndex(c => c.P.name.Equals(datas[0])) > -1)
                                    {
                                        cz = CZs.Find(c => c.P.name.Equals(datas[0]));
                                        datas = sr.ReadLine().Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                        GCZ gcz1 = new GCZ();
                                        gcz1.GCD = datas[0];
                                        gcz1.type = datas[1];
                                        gcz1.value = double.Parse(datas[2]);
                                        cz.GCZs.Add(gcz1);
                                        if (gcz1.type == "L")
                                        {
                                            datas = sr.ReadLine().Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                            GCZ gcz2 = new GCZ();
                                            gcz2.GCD = datas[0];
                                            gcz2.type = datas[1];
                                            gcz2.value = double.Parse(datas[2]);
                                            cz.GCZs.Add(gcz2);
                                        }

                                    }
                                    else
                                    {
                                        cz = new CZ();
                                        cz.P.name = datas[0];
                                        datas = sr.ReadLine().Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                        GCZ gcz1 = new GCZ();
                                        gcz1.GCD = datas[0];
                                        gcz1.type = datas[1];
                                        gcz1.value = double.Parse(datas[2]);
                                        cz.GCZs.Add(gcz1);
                                        if (gcz1.type == "L")
                                        {
                                            datas = sr.ReadLine().Split(fg.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                            GCZ gcz2 = new GCZ();
                                            gcz2.GCD = datas[0];
                                            gcz2.type = datas[1];
                                            gcz2.value = double.Parse(datas[2]);
                                            cz.GCZs.Add(gcz2);
                                        }
                                        CZs.Add(cz);
                                    }
                                    break;
                                }
                        }
                    }
                }
                #endregion
                JD A1 = Operation.HTD(Operation.G_A(Known_Ps[0], Known_Ps[1]));
                dt.Rows.Add(Known_Ps[0].name, "", "", "", "", "", Known_Ps[0].X, Known_Ps[0].Y);
                dt.Rows.Add("", "", A1.d.ToString() + "°" + A1.f.ToString() + "″" + A1.m.ToString("0.00") + "″");
                CZ czf = CZs[0];
                double βf = Operation.DTH(Operation.dfmT(czf.GCZs[1].value - czf.GCZs[0].value));
                if (βf > Math.PI * 2) { βf -= Math.PI * 2; }
                if (βf < 0) { βf += Math.PI * 2; }
                czf.β = Operation.HTD(βf);
                dt.Rows.Add(Known_Ps[1].name, czf.β.d.ToString() + "°" + czf.β.f.ToString() + "″" + czf.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[1].X, Known_Ps[1].Y);
                dt.Rows.Add("", "", "", czf.GCZs[2].value);
                for (int i = 1; i < CZs.Count - 1; i++)//计算转角
                {
                    CZ cz = CZs[i];
                    double β = Operation.DTH(Operation.dfmT(cz.GCZs[1].value - cz.GCZs[0].value));
                    if (β > Math.PI * 2) { β -= Math.PI * 2; }
                    if (β < 0) { β += Math.PI * 2; }
                    cz.β = Operation.HTD(β);
                    dt.Rows.Add(cz.P.name, cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″");
                    dt.Rows.Add("", "", "", cz.GCZs[2].value);
                }
                //最后一站
                CZ czl = CZs[CZs.Count - 1];
                double βl = Operation.DTH(Operation.dfmT(czl.GCZs[1].value - czl.GCZs[0].value));
                if (βl > Math.PI * 2) { βl -= Math.PI * 2; }
                if (βl < 0) { βl += Math.PI * 2; }
                czl.β = Operation.HTD(βl);
                JD A = Operation.HTD(Operation.G_A(Known_Ps[2], Known_Ps[3]));
                dt.Rows.Add(czl.P.name, czl.β.d.ToString() + "°" + czl.β.f.ToString() + "″" + czl.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[2].X, Known_Ps[2].Y);
                dt.Rows.Add("", "", A.d.ToString() + "°" + A.f.ToString() + "″" + A.m.ToString("0.00") + "″");
                dt.Rows.Add(Known_Ps[3].name, "", "", "", "", "", Known_Ps[3].X, Known_Ps[3].Y);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }

        private void 计算坐标增量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Known_Ps.Count == 0 || CZs.Count == 0)
                {
                    MessageBox.Show("请先计算方位角！", "提示");
                    return;
                }
                if (Operation.XY_Adjust(CZs, As, Known_Ps, ref fdx, ref fdy, ref fs, ref sum_s) == 1)
                {
                    dt.Clear();
                    JD A1 = Operation.HTD(Operation.G_A(Known_Ps[0], Known_Ps[1]));
                    dt.Rows.Add(Known_Ps[0].name, "", "", "", "", "", Known_Ps[0].X, Known_Ps[0].Y);
                    dt.Rows.Add("", "", A1.d.ToString() + "°" + A1.f.ToString() + "″" + A1.m.ToString("0.00") + "″");
                    CZ czf = CZs[0];
                    JD A2 = Operation.HTD(As[1]);
                    dt.Rows.Add(Known_Ps[1].name, czf.β.d.ToString() + "°" + czf.β.f.ToString() + "″" + czf.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[1].X, Known_Ps[1].Y);
                    dt.Rows.Add("", "", A2.d.ToString() + "°" + A2.f.ToString() + "″" + A2.m.ToString("0.00") + "″", czf.GCZs[2].value, czf.dx, czf.dy);
                    for (int i = 1; i < CZs.Count - 1; i++)//计算转角
                    {
                        CZ cz = CZs[i];
                        JD Ai = Operation.HTD(As[i + 1]);
                        dt.Rows.Add(cz.P.name, cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″");
                        dt.Rows.Add("", "", Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″", cz.GCZs[2].value, cz.dx, cz.dy);
                    }
                    CZ czl = CZs[CZs.Count - 1];
                    JD A = Operation.HTD(Operation.G_A(Known_Ps[2], Known_Ps[3]));
                    dt.Rows.Add(czl.P.name, czl.β.d.ToString() + "°" + czl.β.f.ToString() + "″" + czl.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[2].X, Known_Ps[2].Y);
                    dt.Rows.Add("", "", A.d.ToString() + "°" + A.f.ToString() + "″" + A.m.ToString("0.00") + "″");
                    dt.Rows.Add(Known_Ps[3].name, "", "", "", "", "", Known_Ps[3].X, Known_Ps[3].Y);
                    MessageBox.Show("坐标增量计算成功！", "提示");
                }
                else
                {
                    MessageBox.Show("超限！", "提示");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }

          

        }

        private void 计算近似坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Operation.ZBJS(CZs, Known_Ps, sum_s, fdx, fdy);
                dt.Clear();
                JD A1 = Operation.HTD(Operation.G_A(Known_Ps[0], Known_Ps[1]));
                dt.Rows.Add(Known_Ps[0].name, "", "", "", "", "", Known_Ps[0].X, Known_Ps[0].Y);
                dt.Rows.Add("", "", A1.d.ToString() + "°" + A1.f.ToString() + "″" + A1.m.ToString("0.00") + "″");
                CZ czf = CZs[0];
                JD A2 = Operation.HTD(As[1]);
                dt.Rows.Add(Known_Ps[1].name, czf.β.d.ToString() + "°" + czf.β.f.ToString() + "″" + czf.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[1].X, Known_Ps[1].Y);
                dt.Rows.Add("", "", A2.d.ToString() + "°" + A2.f.ToString() + "″" + A2.m.ToString("0.00") + "″", czf.GCZs[2].value, czf.dx, czf.dy);
                for (int i = 1; i < CZs.Count - 1; i++)//计算转角
                {
                    CZ cz = CZs[i];
                    JD Ai = Operation.HTD(As[i + 1]);
                    dt.Rows.Add(cz.P.name, cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″", "", "", "", "", cz.P.X, cz.P.Y);
                    dt.Rows.Add("", "", Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″", cz.GCZs[2].value, cz.dx, cz.dy);
                }
                CZ czl = CZs[CZs.Count - 1];
                JD A = Operation.HTD(Operation.G_A(Known_Ps[2], Known_Ps[3]));
                dt.Rows.Add(czl.P.name, czl.β.d.ToString() + "°" + czl.β.f.ToString() + "″" + czl.β.m.ToString("0.00") + "″", "", "", "", "", Known_Ps[2].X, Known_Ps[2].Y);
                dt.Rows.Add("", "", A.d.ToString() + "°" + A.f.ToString() + "″" + A.m.ToString("0.00") + "″");
                dt.Rows.Add(Known_Ps[3].name, "", "", "", "", "", Known_Ps[3].X, Known_Ps[3].Y);
                MessageBox.Show("坐标计算成功！", "提示");
                rect = pictureBox1.ClientRectangle;
                Drawing.Create(rect, CZs, Known_Ps);
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                MessageBox.Show("画图成功！", "提示");
                richTextBox1.Clear();
                richTextBox1.AppendText(
                    "测试数据计算结果\n" +
                    "--------------限差要求-----------------\n" +
                    "角度闭合差限差：" + (40 * Math.Sqrt(CZs.Count)).ToString("0.000") + "\n" +
                    "导线全长相对闭合差限差：0.0002000\n\n");
                richTextBox1.AppendText(
                    "-------------导线基本信息----------------\n" +
                    "测站数：" + CZs.Count + "\n" +
                    "导线全长：" + sum_s.ToString("0.000000") + "\n" +
                    "角度闭合差：" + (fbaita * 3600).ToString("0.000") + "\n" +
                    "Y坐标闭合差：" + fdy.ToString("0.0000") + "\n" +
                    "X坐标闭合差：" + fdx.ToString("0.0000") + "\n" +
                    "导线全长相对闭合差：" + fs.ToString("0.0000") + "\n\n" +
                    "-------------测站点坐标-----------\n" +
                    "测站名            X坐标            Y坐标\n");
                foreach (CZ cz in CZs)
                {
                    richTextBox1.AppendText(
                        cz.P.name.PadRight(15) +
                        cz.P.X.ToString("0.0000").PadRight(15) +
                        cz.P.Y.ToString("0.0000").PadRight(15) + "\n");
                }
                richTextBox1.AppendText(
                    "\n-------------角度数据----------------\n" +
                    "测站名            观测角                 方位角  \n");
                for (int i = 0; i < CZs.Count; i++)
                {
                    CZ cz = CZs[i];
                    JD Ai = Operation.HTD(As[i]);
                    richTextBox1.AppendText((Ai.d.ToString() + "°" + Ai.f.ToString() + "″" + Ai.m.ToString("0.00") + "″").PadLeft(50) + "\n");
                    richTextBox1.AppendText(
                        cz.P.name.PadRight(15) +
                        (cz.β.d.ToString() + "°" + cz.β.f.ToString() + "″" + cz.β.m.ToString("0.00") + "″").PadRight(20) + "\n");
                }
                JD Al = Operation.HTD(As[As.Count - 1]);
                richTextBox1.AppendText((Al.d.ToString() + "°" + Al.f.ToString() + "″" + Al.m.ToString("0.00") + "″").PadLeft(50) + "\n");
                tabControl1.SelectedTab = tabControl1.TabPages[2];
                MessageBox.Show("报告生成成功！", "提示");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }

            
        }

        private void 保存报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(richTextBox1.Text))
                {
                    MessageBox.Show("请先计算", "提示");
                    return;
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "文本文件|*.txt";
                    sfd.Title = "保存报告";
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.Write(richTextBox1.Text);
                    }
                    MessageBox.Show("保存成功！", "提示");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }
            
        }

        private void 保存示意图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "JPG文件|*.jpg";
                sfd.Title = "示意图保存";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                string path = sfd.FileName;
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save(path);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }

           
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            try
            {
                Drawing.FD(e);
                pictureBox1.Refresh();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }
           
        }

        private void 操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_ClientSizeChanged(object sender, EventArgs e)
        {
            rect = pictureBox1.ClientRectangle;
            Drawing.Create(rect, CZs, Known_Ps);
            pictureBox1.Refresh();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                Drawing.SX(e);
                pictureBox1.Refresh();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "提示");
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
