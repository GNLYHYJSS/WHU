using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace _20220719导线测量
{
    class Drawing
    {
        static double minX, minY, maxX, maxY;//画区最大最小值
        static double dsx, dsy;//画区比例尺，实际距离转换为屏幕距离
        static double margin;//画区边界
        static System.Drawing.Point endPoint, startPoint;//鼠标抬起，按下位置
        static bool mousedown;//鼠标是否抬起

        static public void Create(Rectangle rect, List<CZ> CZs, List<Point> Known_Ps) //创建画布
        {
            margin = rect.Height / 6;
            maxX = double.MinValue;
            maxY = double.MinValue;
            minX = double.MaxValue;
            minY = double.MaxValue;
            dsx = 1;
            dsy = 1;
            foreach (Point point in Known_Ps)
            {
                if (point.X > maxX) maxX = point.X;
                if (point.X < minX) minX = point.X;
                if (point.Y > maxY) maxY = point.Y;
                if (point.Y < minY) minY = point.Y;
            }
            foreach (CZ cz in CZs)
            {
                if (cz.P.X > maxX) maxX = cz.P.X;
                if (cz.P.X < minX) minX = cz.P.X;
                if (cz.P.Y > maxY) maxY = cz.P.Y;
                if (cz.P.Y < minY) minY = cz.P.Y;
            }
            dsy = Math.Abs(maxX - minX) / (rect.Height - 2 * margin);//dsy就是纵轴上的比例尺
            dsx = Math.Abs(maxY - minY) / (rect.Width - 2 * margin);//dsx就是横轴上的比例尺
        }

        static public void DrawKP(Graphics g, Rectangle rect, List<Point> Known_Ps)  //已知点以及连线，三角形
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Red, (float)2);
            Pen p2 = new Pen(Color.Black, (float)2);
            Font f1 = new Font("宋体", 10, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Red);
            Brush b2 = new SolidBrush(Color.Black);

            double x0 = margin + (Known_Ps[0].Y - minY) / dsx;
            double y0 = rect.Height - margin - (Known_Ps[0].X - minX) / dsy;

            double x1 = margin + (Known_Ps[1].Y - minY) / dsx;
            double y1 = rect.Height - margin - (Known_Ps[1].X - minX) / dsy;

            double x2 = margin + (Known_Ps[2].Y - minY) / dsx;
            double y2 = rect.Height - margin - (Known_Ps[2].X - minX) / dsy;

            double x3 = margin + (Known_Ps[3].Y - minY) / dsx;
            double y3 = rect.Height - margin - (Known_Ps[3].X - minX) / dsy;

            g.DrawLine(p1, (float)x0, (float)y0, (float)x1, (float)y1);
            g.DrawLine(p1, (float)x2, (float)y2, (float)x3, (float)y3);

            foreach (Point point in Known_Ps)
            {
                PointF[] ps = new PointF[4];
                double x = margin + (point.Y - minY) / dsx;
                double y = rect.Height - margin - (point.X - minX) / dsy;

                ps[0].X = (float)x; ps[0].Y = (float)(y - 2);
                ps[1].X = (float)(x - 2); ps[1].Y = (float)(y + 2);
                ps[2].X = (float)(x + 2); ps[2].Y = (float)(y + 2);
                ps[3].X = (float)x; ps[3].Y = (float)(y - 2);

                g.DrawPolygon(p1, ps);
                g.FillPolygon(b1, ps);

                g.DrawString(point.name, f1, b2, (float)x, (float)y);
            }
        }

        static public void DrawCZ(Graphics g, Rectangle rect, List<CZ> CZs)        //测站点以及连线，圆形
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)2);
            Pen p2 = new Pen(Color.Blue, (float)2);
            Brush b1 = new SolidBrush(Color.Black);

            for (int i = 1; i < CZs.Count; i++)
            {
                double x0 = margin + (CZs[i].P.Y - minY) / dsx;
                double y0 = rect.Height - margin - (CZs[i].P.X - minX) / dsy;
                double x1 = margin + (CZs[i - 1].P.Y - minY) / dsx;
                double y1 = rect.Height - margin - (CZs[i - 1].P.X - minX) / dsy;

                g.DrawLine(p1, (float)x0, (float)y0, (float)x1, (float)y1);
            }

            foreach (CZ cz in CZs)
            {
                double x = margin + (cz.P.Y - minY) / dsx;
                double y = rect.Height - margin - (cz.P.X - minX) / dsy;

                g.DrawEllipse(p1, (float)x, (float)y, (float)2, (float)2);
                g.FillEllipse(b1, (float)x, (float)y, (float)2, (float)2);
            }
        }

        static public void DrawAX(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            double AX = margin / 4;
            double ax = margin / 8;
            double tmargin = margin / 2;
            Pen p1 = new Pen(Color.Black, (float)3);
            Font f1 = new Font("宋体", 20, FontStyle.Bold);
            Font f2 = new Font("宋体", 8, FontStyle.Bold);
            Font f3 = new Font("宋体", 16, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);

            //X轴
            g.DrawLine(p1, (float)tmargin, (float)(rect.Height - tmargin), (float)(rect.Width - tmargin), (float)(rect.Height - tmargin));
            g.DrawLine(p1, (float)(rect.Width - tmargin), (float)(rect.Height - tmargin), (float)(rect.Width - tmargin - AX), (float)(rect.Height - tmargin + ax));
            g.DrawLine(p1, (float)(rect.Width - tmargin), (float)(rect.Height - tmargin), (float)(rect.Width - tmargin - AX), (float)(rect.Height - tmargin - ax));
            g.DrawString("Y", f3, b1, (float)(rect.Width - tmargin), (float)(rect.Height - tmargin - 15));
            double dx = (rect.Width - 2 * tmargin) / 8 * dsx;
            for (int i = 0; i < 8; i++)
            {
                double x = tmargin + i * dx / dsx;
                string st = (minY + i * dx-(margin-tmargin)*dsx).ToString("0.0");
                g.DrawLine(p1, (float)x, (float)(rect.Height - tmargin), (float)x, (float)(rect.Height - tmargin + 5));
                g.DrawString(st, f2, b1, (float)(x - 5), (float)(rect.Height - tmargin + 10));
            }

            //Y轴
            g.DrawLine(p1, (float)tmargin, (float)(rect.Height - tmargin), (float)tmargin, (float)tmargin);
            g.DrawLine(p1, (float)tmargin, (float)tmargin, (float)(tmargin - ax), (float)(tmargin + AX));
            g.DrawLine(p1, (float)tmargin, (float)tmargin, (float)(tmargin + ax), (float)(tmargin + AX));
            g.DrawString("X", f3, b1, (float)(tmargin + 10), (float)tmargin);
            double dy = (rect.Height - 2 * tmargin) / 6 * dsy;
            for (int i = 0; i < 6; i++)
            {
                double y = (rect.Height - tmargin) - i * dy / dsy;
                string st = (minX + i * dy-(margin-tmargin)*dsy).ToString("0.0");
                g.DrawLine(p1, (float)tmargin, (float)y, (float)(tmargin - 5), (float)y);
                g.DrawString(st, f2, b1, (float)(tmargin - 60), (float)y);
            }

            g.DrawString("附和导线近似平差示意图", f1, b1, (float)(rect.Width / 2 - 80), (float)(10));
        }

        static public void MouseDown(MouseEventArgs e)
        {
            mousedown = true;
            endPoint = e.Location;
            startPoint = e.Location;
        }

        static public void MouseUp(MouseEventArgs e)
        {
            mousedown = false;
        }

        static public void MouseMove(MouseEventArgs e)
        {
            double dx, dy;
            if (mousedown)
            {
                endPoint = e.Location;
                dy = (endPoint.X - startPoint.X) * dsx;
                dx = (endPoint.Y - startPoint.Y) * dsy;
                minX += dx;
                minY -= dy;
                startPoint = e.Location;
            }
        }

        static public void MouseDelta(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                dsx *= 0.8;
                dsy *= 0.8;
            }
            if (e.Delta < 0)
            {
                dsx *= 1.2;
                dsy *= 1.2;
            }
        }

        static public void FD(EventArgs e)
        {
            dsx *= 1.2;
            dsy *= 1.2;
        }

        static public void SX(EventArgs e)
        {
            dsx *= 0.8;
            dsy *= 0.8;
        }
    }
}
