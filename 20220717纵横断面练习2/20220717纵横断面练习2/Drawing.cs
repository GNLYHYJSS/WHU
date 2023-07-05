using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace _20220717纵横断面练习2
{
    class Drawing
    {
        static double minX, minY, maxX, maxY;
        static double minK, minH, maxK, maxH;
        static double dsx, dsy;
        static double dsh, dsk;
        static double margin;
        static Rectangle rect;

        //路面画图
        public static void CreateLM(Rectangle rect, List<Point> points, List<Point> hdms1, List<Point> hdms2)
        {
            margin = rect.Height / 6;
            minX = double.MaxValue;
            minY = double.MaxValue;
            maxX = double.MinValue;
            maxY = double.MinValue;
            foreach (Point point in points)
            {
                if (point.x > maxX) maxX = point.x;
                if (point.y > maxY) maxY = point.y;
                if (point.x < minX) minX = point.x;
                if (point.y < minY) minY = point.y;
            }
            foreach (Point hdm1 in hdms1)
            {
                if (hdm1.x > maxX) maxX = hdm1.x;
                if (hdm1.y > maxY) maxY = hdm1.y;
                if (hdm1.x < minX) minX = hdm1.x;
                if (hdm1.y < minY) minY = hdm1.y;
            }
            foreach (Point hdm2 in hdms2)
            {
                if (hdm2.x > maxX) maxX = hdm2.x;
                if (hdm2.y > maxY) maxY = hdm2.y;
                if (hdm2.x < minX) minX = hdm2.x;
                if (hdm2.y < minY) minY = hdm2.y;
            }
            dsx = Math.Abs(maxX - minX) / (rect.Width - 2 * margin);
            dsy = Math.Abs(maxY - minY) / (rect.Height - 2 * margin);
        }

        public static void DrawLM(Graphics g, Rectangle rect, List<Point> points, List<Point> zds, List<Point> hdms1, List<Point> hdms2)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Pen p2 = new Pen(Color.Red, (float)3);
            Brush b1 = new SolidBrush(Color.Black);
            Brush b2 = new SolidBrush(Color.Red);
            Font f1 = new Font("宋体", 16, FontStyle.Bold);

            //画线，纵断面
            double xz0 = margin + (zds[0].x - minX) / dsx;
            double yz0 = rect.Height - margin - (zds[0].y - minY) / dsy;
            double xz1 = margin + (zds[2].x - minX) / dsx;
            double yz1 = rect.Height - margin - (zds[2].y - minY) / dsy;
            g.DrawLine(p1, (float)xz0, (float)yz0, (float)xz1, (float)yz1);

            //画线，横断面
            for (int i = 1; i < hdms1.Count; i++)
            {
                double xh10 = margin + (hdms1[i - 1].x - minX) / dsx;
                double yh10 = rect.Height - margin - (hdms1[i - 1].y - minY) / dsy;
                double xh11 = margin + (hdms1[i].x - minX) / dsx;
                double yh11 = rect.Height - margin - (hdms1[i].y - minY) / dsy;
                g.DrawLine(p1, (float)xh10, (float)yh10, (float)xh11, (float)yh11);
            }

            for (int i = 1; i < hdms2.Count; i++)
            {
                double xh20 = margin + (hdms2[i - 1].x - minX) / dsx;
                double yh20 = rect.Height - margin - (hdms2[i - 1].y - minY) / dsy;
                double xh21 = margin + (hdms2[i].x - minX) / dsx;
                double yh21 = rect.Height - margin - (hdms2[i].y - minY) / dsy;
                g.DrawLine(p1, (float)xh20, (float)yh20, (float)xh21, (float)yh21);
            }

            //画点，散点
            foreach (Point point in points)
            {
                double x = margin + (point.x - minX) / dsx;
                double y = rect.Height - margin - (point.y - minY) / dsy;
                g.DrawEllipse(p1, (float)x, (float)y, (float)2, (float)2);
                g.FillEllipse(b1, (float)x, (float)y, (float)2, (float)2);
            }

            //画点，主点
            foreach (Point zd in zds)
            {
                double x = margin + (zd.x - minX) / dsx;
                double y = rect.Height - margin - (zd.y - minY) / dsy;
                g.DrawEllipse(p2, (float)x, (float)y, (float)3, (float)3);
                g.FillEllipse(b2, (float)x, (float)y, (float)3, (float)3);
                g.DrawString(zd.name, f1,b1,(float)x, (float)y);
            }

        }

        public static void DrawLM_AX(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Font f1 = new Font("宋体", 10, FontStyle.Bold);
            Font f2 = new Font("宋体", 20, FontStyle.Bold);
            Font f3 = new Font("宋体", 12, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            double AX = margin / 4;
            double ax = margin / 8;

            //X轴
            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)(rect.Width - margin), (float)(rect.Height - margin));
            g.DrawLine(p1, (float)(rect.Width - margin), (float)(rect.Height - margin), (float)(rect.Width - margin - AX), (float)(rect.Height - margin + ax));
            g.DrawLine(p1, (float)(rect.Width - margin), (float)(rect.Height - margin), (float)(rect.Width - margin - AX), (float)(rect.Height - margin - ax));
            g.DrawString("X", f3, b1, (float)(rect.Width - margin), (float)(rect.Height - margin + 10));
            double dx = (rect.Width - 2 * margin) / 6 * dsx;
            for (int i = 0; i < 6; i++)
            {
                double x = margin + i * dx / dsx;
                g.DrawLine(p1, (float)x, (float)(rect.Height - margin), (float)x, (float)(rect.Height - margin + 5));
                string st = (minX + i * dx).ToString("0");
                g.DrawString(st, f1, b1, (float)(x-5), (float)(rect.Height - margin + 10));
            }

            //Y轴
            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)margin, (float)margin);
            g.DrawLine(p1, (float)margin, (float)margin, (float)(margin - ax), (float)(margin + AX));
            g.DrawLine(p1, (float)margin, (float)margin, (float)(margin + ax), (float)(margin + AX));
            g.DrawString("Y", f3, b1, (float)(margin+5 ), (float)margin);

            double dy = (rect.Height - 2 * margin) / 5 * dsy;
            for (int i = 0; i < 5; i++)
            {
                double y = rect.Height - margin - i * dy / dsy;
                g.DrawLine(p1, (float)margin, (float)y, (float)(margin - 5), (float)y);
                string st = (minY + i * dy).ToString("0");
                g.DrawString(st, f1, b1, (float)(margin - 45), (float)y);
            }

            g.DrawString("路面示意图", f2, b1, (float)(rect.Width / 2 - 50), (float)10);
        }

        public static void CreateDM(Rectangle rect, List<Point> hdms)
        {
            margin = rect.Height / 6;
            minK = double.MaxValue;
            minH = 0;
            maxK = double.MinValue;
            maxH = double.MinValue;
            foreach (Point hdm in hdms)
            {
                if (hdm.K > maxK) maxK = hdm.K;
                if (hdm.h > maxH) maxH = hdm.h;
                if (hdm.K < minK) minK = hdm.K;

            }
            dsh = Math.Abs(maxH - minH) / (rect.Height - 2 * margin);
            dsk = Math.Abs(maxK - minK) / (rect.Width - 2 * margin);
        }

        public static void DrawDM(Graphics g, Rectangle rect, List<Point> hdms)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Brush b1 = new SolidBrush(Color.Gray);

            PointF[] pointfs = new PointF[hdms.Count + 3];

            pointfs[0].X = (float)margin;
            pointfs[0].Y = (float)(rect.Height - margin);

            for (int i = 1; i < hdms.Count + 1; i++)
            {
                double x = margin + (hdms[i - 1].K - minK) / dsk;
                double y = rect.Height - margin - (hdms[i - 1].h - minH) / dsh;
                pointfs[i].X = (float)x;
                pointfs[i].Y = (float)y;
            }
            pointfs[hdms.Count + 1].X = (float)(margin + (hdms[hdms.Count - 1].K - minK) / dsk);
            pointfs[hdms.Count + 1].Y = (float)(rect.Height - margin);

            pointfs[hdms.Count + 2].X = (float)margin;
            pointfs[hdms.Count + 2].Y = (float)(rect.Height - margin);


            g.DrawPolygon(p1, pointfs);
            g.FillPolygon(b1, pointfs);

            for (int i = 1; i < hdms.Count; i++)
            {
                double x0 = margin + (hdms[i - 1].K - minK) / dsk;
                double y0 = rect.Height - margin - (hdms[i - 1].h - minH) / dsh;
                double x1 = margin + (hdms[i].K - minK) / dsk;
                double y1 = rect.Height - margin - (hdms[i].h - minH) / dsh;
                g.DrawLine(p1, (float)x0, (float)y0, (float)x1, (float)y1);
            }
        }

        public static void DrawDM_AX(Graphics g, Rectangle rect, int flag)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen p1 = new Pen(Color.Black, (float)3);
            Font f1 = new Font("宋体", 10, FontStyle.Bold);
            Font f2 = new Font("宋体", 20, FontStyle.Bold);
            Font f3 = new Font("宋体", 16, FontStyle.Bold);
            Brush b1 = new SolidBrush(Color.Black);
            double AX = margin / 4;
            double ax = margin / 8;

            //X轴
            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)(rect.Width), (float)(rect.Height - margin));
            g.DrawLine(p1, (float)(rect.Width), (float)(rect.Height - margin), (float)(rect.Width - AX), (float)(rect.Height - margin + ax));
            g.DrawLine(p1, (float)(rect.Width), (float)(rect.Height - margin), (float)(rect.Width - AX), (float)(rect.Height - margin - ax));
            g.DrawString("K", f3, b1, (float)(rect.Width - ax-5), (float)(rect.Height - margin + 10));
            double dk = (rect.Width - 2 * margin) / 6 * dsk;
            for (int i = 0; i < 7; i++)
            {
                double k = margin + i * dk / dsk;
                g.DrawLine(p1, (float)k, (float)(rect.Height - margin), (float)k, (float)(rect.Height - margin + 5));
                string st = (minK + i * dk).ToString("0");
                g.DrawString(st, f1, b1, (float)k, (float)(rect.Height - margin + 10));
            }

            //Y轴
            g.DrawLine(p1, (float)margin, (float)(rect.Height - margin), (float)margin, (float)0);
            g.DrawLine(p1, (float)margin, (float)0, (float)(margin - ax), (float)(0 + AX));
            g.DrawLine(p1, (float)margin, (float)0, (float)(margin + ax), (float)(0 + AX));
            g.DrawString("H", f3, b1, (float)(margin - 25), (float)0 + 5);

            double dh = (rect.Height - 2 * margin) / 5 * dsh;
            for (int i = 0; i < 6; i++)
            {
                double h = rect.Height - margin - i * dh / dsh;
                g.DrawLine(p1, (float)margin, (float)h, (float)(margin - 5), (float)h);
                string st = (minH + i * dh).ToString("0");
                g.DrawString(st, f1, b1, (float)(margin - 25), (float)h);
            }

            if (flag == 2) g.DrawString("纵断面示意图", f2, b1, (float)(rect.Width / 2 - 50), (float)10);
            else g.DrawString("横断面示意图", f2, b1, (float)(rect.Width / 2 - 50), (float)10);
        }
    }
}
