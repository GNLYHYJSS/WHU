using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace _20220718Grid体积计算
{
    class Drawing
    {
        static double minX, minY, maxX, maxY;//存储画区最大最小点
        static double dsx, dsy;//存储画区比例尺，将实际距离转换为屏幕距离
        static double margin;//建立边界
        static Rectangle rect;//存储画布大小
        static System.Drawing.Point endPoint, startPoint;//存储鼠标移动位置
        static bool mousedown;

        static public void Create(Rectangle rect, Stack<Point> S)  //创建画布
        {
            margin = rect.Height / 6;
            List<Point> points = new List<Point>();
            foreach (Point s in S)
            {
                Point point = new Point();
                point.x = s.x;
                point.y = s.y;
                point.name = s.name;
                points.Add(point);
            }
            maxX = double.MinValue;
            maxY = double.MinValue;
            minX = double.MaxValue;
            minY = double.MaxValue;
            dsx = 1; dsy = 1;
            foreach (Point point in points)
            {
                if (point.x > maxX) maxX = point.x;
                if (point.x < minX) minX = point.x;
                if (point.y > maxY) maxY = point.y;
                if (point.y < minY) minY = point.y;
            }
            dsx = Math.Abs(maxX - minX) / (rect.Width - 2 * margin);
            dsy = Math.Abs(maxY - minY) / (rect.Height - 2 * margin);
        }

        static public void DrawTB(Graphics g, Rectangle rect, Stack<Point> S, List<Point> points) //画凸包点
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Brush b1 = new SolidBrush(Color.Yellow);
            Brush b2 = new SolidBrush(Color.Black);
            List<Point> Points = new List<Point>();
            foreach (Point s in S)
            {
                Point point = new Point();
                point.x = s.x;
                point.y = s.y;
                point.name = s.name;
                Points.Add(point);
            }
            PointF[] pointfs = new PointF[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                double x = (Points[i].x - minX) / dsx + margin;
                double y = (rect.Height - margin) - (Points[i].y - minY) / dsy;
                pointfs[i].X = (float)x;
                pointfs[i].Y = (float)y;
            }
            g.DrawPolygon(p1, pointfs);
            g.FillPolygon(b1, pointfs);


            foreach (Point point in points)
            {
                double x1 = margin + (point.x - minX) / dsx;
                double y1 = (rect.Height - margin) - (point.y - minY) / dsy;
                g.DrawEllipse(p1, (float)x1, (float)y1, (float)2, (float)2);
                g.FillEllipse(b2, (float)x1, (float)y1, (float)2, (float)2);
            }
        }

        static public void DrawGW(Graphics g, Rectangle rect, GW gw)   //画格网
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);

            for (int i = 0; i <= Math.Ceiling(gw.h / gw.L); i++)
            {
                double y0 = (rect.Height - margin) - (i * gw.L + (gw.P.y - minY)) / dsy;
                double x0 = margin + (gw.P.x - minX) / dsx;
                double x1 = margin + (gw.P.x+Math.Ceiling(gw.w/gw.L)*gw.L - minX) / dsx;
                g.DrawLine(p1, (float)x0, (float)y0, (float)x1, (float)y0);
            }

            for (int i = 0; i <= Math.Ceiling(gw.w/gw.L); i++)
            {
                double x0 = margin + (i * gw.L + (gw.P.x - minX)) / dsx;
                double y0 = (rect.Height - margin) - (gw.P.y - minY) / dsy;
                double y1 = (rect.Height - margin) - (gw.P.y + Math.Ceiling(gw.h / gw.L) * gw.L - minY) / dsy;
                g.DrawLine(p1, (float)x0, (float)y0, (float)x0, (float)y1);
            }
        }

        static public void DrawBT(Graphics g,Rectangle rect)
        {
            Pen p1 = new Pen(Color.Black, (float)3);
            Font f1 = new Font("宋体", 20, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            g.DrawString("规则格网示意图", f1, b1, (float)(rect.Width / 2 - 50), (float)10);
        }   //画标题

        static public void Mousedown(MouseEventArgs e)
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
                dx = (endPoint.X - startPoint.X) * dsx;
                dy = (endPoint.Y - startPoint.Y) * dsy;
                minX -= dx;
                minY += dy;
                startPoint = e.Location;
            }
        }

        static public void MouseDelta(MouseEventArgs e)
        {
            if (e.Delta>0)
            {
                dsx *= 0.8;
                dsy *= 0.8;
            }
            if (e.Delta<0)
            {
                dsx *= 1.2;
                dsy *= 1.2;
            }
        }

        static public void DrawAX(Graphics g,Rectangle rect)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Font f1 = new Font("宋体", 15, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            double ax = margin / 8;
            double AX = margin / 4;
            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)margin, (float)margin);
            g.DrawLine(p1, (float)margin, (float)margin, (float)(margin - ax), (float)(margin + AX));
            g.DrawLine(p1, (float)margin, (float)margin, (float)(margin + ax), (float)(margin + AX));
            g.DrawString("Y", f1, b1, (float)(margin - 20), (float)margin);

            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)(rect.Width - margin), (float)(rect.Height - margin));
            g.DrawLine(p1, (float)(rect.Width - margin), (float)(rect.Height - margin), (float)(rect.Width - margin - AX), (float)(rect.Height - margin + ax));
            g.DrawLine(p1, (float)(rect.Width - margin), (float)(rect.Height - margin), (float)(rect.Width - margin - AX), (float)(rect.Height - margin - ax));
            g.DrawString("X", f1, b1, (float)(rect.Width - margin), (float)(rect.Height - margin+15));
        }
    }
}
