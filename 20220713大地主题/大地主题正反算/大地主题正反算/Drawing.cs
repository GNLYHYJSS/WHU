using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace 大地主题正反算
{
    class Drawing
    {
        public static double minB;                   //存储画布横轴最小值
        public static double maxB;                   //存储画布横轴最大值
        public static double minL;                   //存储画布纵轴最小值
        public static double maxL;                   //存储画布纵轴最大值
        public static double margin;                 //画布边界规定
        public static double dsb;                    //绘图比例尺，将横轴上的实际距离转换为屏幕距离
        public static double dsl;                    //绘图比例尺，将纵轴上的实际距离转换为屏幕距离
        public static bool mousedown = false;          //图像移动，记录鼠标结束
        public static System.Drawing.Point endPoint; //图像移动，记录鼠标结束位置
        public static System.Drawing.Point startPoint;//图像移动，记录鼠标开始位置

        public static void Create(Rectangle rect, List<GCZ> gczs)//创建画布
        {
            minB = double.MaxValue;
            minL = double.MaxValue;
            maxB = double.MinValue;
            maxL = double.MinValue;
            margin = rect.Height / 6;
            foreach (GCZ gcz in gczs)
            {
                if (gcz.p1.B > maxB) maxB = gcz.p1.B;
                if (gcz.p1.L > maxL) maxL = gcz.p1.L;
                if (gcz.p1.B < minB) minB = gcz.p1.B;
                if (gcz.p1.L < minL) minL = gcz.p1.L;

                if (gcz.p2.B > maxB) maxB = gcz.p2.B;
                if (gcz.p2.L > maxL) maxL = gcz.p2.L;
                if (gcz.p2.B < minB) minB = gcz.p2.B;
                if (gcz.p2.L < minL) minL = gcz.p2.L;
            }
            //计算图像上绘图时最大点和最小点
            dsb = Math.Abs(maxB - minB) / (rect.Height - 2 * margin);
            dsl = Math.Abs(maxL - minL) / (rect.Width - 2 * margin);
            //得到将实际距离转换成为屏幕距离的比例尺
        }

        public static void Draw(Graphics g, Rectangle rect, List<GCZ> gczs)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Pen p2 = new Pen(Color.Red, (float)3);
            Font f1 = new Font("宋体", 10, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            Brush b2 = new SolidBrush(Color.Red);

            foreach (GCZ gcz in gczs)
            {
                double x0 = margin + (gcz.p1.L - minL) / dsl;
                double y0 = (rect.Height - margin) - (gcz.p1.B - minB) / dsb;
                double x1 = margin + (gcz.p2.L - minL) / dsl;
                double y1 = (rect.Height - margin) - (gcz.p2.B - minB) / dsb;
                g.DrawLine(p1, (float)x0, (float)y0, (float)x1, (float)y1);
                //绘制大地线
                double x = margin + (gcz.p1.L - minL) / dsl;
                double y = (rect.Height - margin) - (gcz.p1.B - minB) / dsb;
                string name0 = gcz.p1.name;
                double xx = margin + (gcz.p2.L-minL) / dsl;
                double yy = (rect.Height - margin) - (gcz.p2.B - minB) / dsb;
                string name1 = gcz.p2.name;
                g.DrawEllipse(p2, (float)x, (float)y, (float)2, (float)2);
                g.FillEllipse(b2, (float)x, (float)y, (float)2, (float)2);
                g.DrawEllipse(p2, (float)xx, (float)yy, (float)2, (float)2);
                g.FillEllipse(b2, (float)xx, (float)yy, (float)2, (float)2);
                g.DrawString(name0, f1, b1, (float)x, (float)y+5);
                g.DrawString(name1, f1, b1, (float)xx, (float)yy+5);
                //绘制数据点和结果点
            }



        }

        public static void AX(Graphics g, Rectangle rect)
        {
            double tmargin = margin / 1.5;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Pen p2 = new Pen(Color.Black, (float)2);
            Font f1 = new Font("宋体", 8, FontStyle.Bold);
            Font f3 = new Font("宋体", 16, FontStyle.Bold);
            Font f4 = new Font("宋体", 10, FontStyle.Bold);
            Font f2 = new Font("宋体", 20, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            Brush b2 = new SolidBrush(Color.Gray);
            // Brush b2 = new SolidBrush(Color.Red);
            double AX = margin / 2;
            double ax = margin / 4;

            g.DrawLine(p1, (float)tmargin, (float)(rect.Height - tmargin), (float)tmargin, (float)tmargin);
            g.DrawLine(p1, (float)tmargin, (float)tmargin, (float)(tmargin - ax/2), (float)(tmargin + AX/2));
            g.DrawLine(p1, (float)tmargin, (float)tmargin, (float)(tmargin + ax/2), (float)(tmargin + AX/2));
            g.DrawString("B", f3, b1, (float)(tmargin - 25), (float)tmargin);

            g.DrawLine(p1, (float)tmargin, (float)(rect.Height - tmargin), (float)(rect.Width - tmargin), (float)(rect.Height - tmargin));
            g.DrawLine(p1, (float)(rect.Width - tmargin), (float)(rect.Height - tmargin), (float)(rect.Width - tmargin - AX/2), (float)(rect.Height - tmargin - ax/2));
            g.DrawLine(p1, (float)(rect.Width - tmargin), (float)(rect.Height - tmargin), (float)(rect.Width - tmargin - AX/2), (float)(rect.Height - tmargin + ax/2));
            g.DrawString("L", f3, b1, (float)(rect.Width - tmargin + 5), (float)(rect.Height - tmargin + 5));

            double dx = (rect.Width - 2 * tmargin) / 8 * dsl;
            double dy = (rect.Height - 2 * tmargin) / 5 * dsb;

            for (int i = 0; i < 8; i++)
            {
                double x = i * dx / dsl + tmargin;
                g.DrawLine(p2, (float)x, (float)(rect.Height - tmargin), (float)x, (float)(rect.Height - tmargin + 10));
                string st = (minL-(margin-tmargin)*dsl + i * dx).ToString("0.0");
                g.DrawString(st, f4, b1, (float)x, (float)(rect.Height - tmargin + 10));
            }
            for (int i = 0; i < 5; i++)
            {
                double y = (rect.Height - tmargin)  - i * dy / dsb;
                g.DrawLine(p2, (float)tmargin, (float)y, (float)(tmargin - 10), (float)y);
                string st = (minB-(margin-tmargin)*dsb + i * dy).ToString("0.0");
                g.DrawString(st, f4, b1, (float)tmargin - 45, (float)(y-10));
            }

            g.DrawString("大地主题解算示意图", f2, b1, (float)(rect.Width / 2-100), (float)(margin - AX));

            PointF[] pointf = new PointF[5];

            pointf[0].X = (float)(rect.Width - margin);
            pointf[0].Y = (float)(margin);

            pointf[1].X = (float)(rect.Width - margin - ax);
            pointf[1].Y = (float)(margin + AX);

            pointf[2].X = (float)(rect.Width - margin);
            pointf[2].Y = (float)(margin + ax);

            pointf[3].X = (float)(rect.Width - margin + ax);
            pointf[3].Y = (float)(margin + AX);

            pointf[4].X = (float)(rect.Width - margin);
            pointf[4].Y = (float)(margin);

            g.FillPolygon(b2, pointf);

            g.DrawString("N", f3, b1, (float)(rect.Width - margin + ax), (float)(margin));
        }

        public static void MouseUP(MouseEventArgs e)
        {
            mousedown = false;
        }

        public static void MouseDown(MouseEventArgs e)
        {
            mousedown = true;
            endPoint = e.Location;
            startPoint = e.Location;
        }

        public static void MouseMove(MouseEventArgs e)
        {
            double dx = 0, dy = 0;
            if (mousedown)//判断鼠标是否按下
            {
                endPoint = e.Location;
                dx = (endPoint.X - startPoint.X) * dsl;
                dy = (endPoint.Y - startPoint.Y) * dsb;
                //鼠标移动距离
                minL -= dx;
                minB += dy;
                //图像移动距离
                startPoint = e.Location;
            }
        }

        public static void MouseDelta(MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                dsb *= 1.2;
                dsl *= 1.2;
            }
            if (e.Delta > 0)
            {
                dsb *= 0.8;
                dsl *= 0.8;
            }
        }


    }
}

