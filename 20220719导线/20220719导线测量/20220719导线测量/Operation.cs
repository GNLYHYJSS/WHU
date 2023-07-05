using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _20220719导线测量
{
    class Operation
    {
        static public JD dfmT(double dfm)
        {
            JD jd = new JD();
            jd.d = Math.Floor(dfm);
            jd.f = Math.Floor(100 * (dfm - jd.d));
            jd.m = 100 * ((dfm - jd.d) * 100 - jd.f);
            return jd;
        }
        static public double DTH(JD jd)
        {
            return (jd.d + jd.f / 60 + jd.m / 3600) / 180 * Math.PI;
        }
        static public JD HTD(double H)
        {
            JD jd = new JD();
            double temp = H * 180 / Math.PI;
            jd.d = Math.Floor(temp);
            jd.f = Math.Floor((temp - jd.d) * 60);
            jd.m = (temp - jd.d - jd.f / 60) * 3600;
            return jd;
        }
        static public double G_A(Point p1, Point p2)//计算坐标方位角
        {
            double A = -1;
            double dy = p2.Y - p1.Y;
            double dx = p2.X - p1.X;
            if (dy == 0)
            {
                if (dx > 0) { A = 0; }
                if (dx < 0) { A = Math.PI; }
                if (dx == 0) { A = -1; }
            }
            else
            {
                A = Math.PI - Math.PI / 2 * Math.Sign(dy) - Math.Atan(dx / dy);
            }
            return A;
        }
        static public double Dist(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        static public double JD_Adjust(List<CZ> CZs, List<double> As, List<Point> Known_Ps, ref double fbeita)//角度近似平差
        {
            int n = 1;
            As.Add(G_A(Known_Ps[0], Known_Ps[1]));//第一个方位角
            for (int i = 0; i < CZs.Count; i++)
            {
                double A = As[n - 1] + Operation.DTH(CZs[i].β) - Math.PI;//弧度
                while (A > Math.PI * 2) { A -= Math.PI * 2; }
                while (A < 0) { A += Math.PI * 2; }
                As.Add(A);
                n++;
            }
            fbeita = As[n - 1] - G_A(Known_Ps[2], Known_Ps[3]);
            if (Math.Abs(fbeita * 3600) < 40 * Math.Sqrt(CZs.Count)) //是否符合限差
            {
                int index = 1;
                double v = -fbeita / CZs.Count;
                foreach (CZ cz in CZs)
                {
                    cz.β = Operation.HTD(Operation.DTH(cz.β) + v);
                }
                foreach (CZ cz in CZs)
                {
                    As[index] = As[index - 1] + Operation.DTH(cz.β) - Math.PI;
                    while (As[index] > Math.PI * 2) { As[index] -= Math.PI * 2; }
                    while (As[index] < 0) { As[index] += Math.PI * 2; }
                    index++;
                }
                return 1;
            }
            else
            {
                return -1;
            }
        }
        static public double XY_Adjust(List<CZ> CZs, List<double> As, List<Point> Known_Ps, ref double fx, ref double fy, ref double fs, ref double sum_s)//角度近似平差
        {
            for (int i = 0; i < CZs.Count - 1; i++)
            {
                CZ cz = CZs[i];
                cz.dx = cz.GCZs[2].value * Math.Cos(As[i + 1]);
                cz.dy = cz.GCZs[2].value * Math.Sin(As[i + 1]);
                fx += cz.dx;
                fy += cz.dy;
                sum_s += cz.GCZs[2].value;
            }
            fx = fx + Known_Ps[1].X - Known_Ps[2].X;
            fy = fy + Known_Ps[1].Y - Known_Ps[2].Y;
            fs = Math.Sqrt(fx * fx + fy * fy);
            if (fs / sum_s <= (double)1 / (double)5000)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        static public void ZBJS(List<CZ> CZs, List<Point> Known_Ps, double sum_s,double fx,double fy)
        {
            CZs[0].P = Known_Ps[1];
            CZs[CZs.Count - 1].P = Known_Ps[2];
            for (int i = 1; i < CZs.Count - 1; i++)
            {
                CZ czi = CZs[i-1];
                CZ czj = CZs[i];
                double vxi = -fx * czi.GCZs[2].value / sum_s;
                double vyi = -fy * czi.GCZs[2].value / sum_s;
                czj.P.X = czi.P.X + vxi + czi.dx;
                czj.P.Y = czi.P.Y + vyi + czi.dy;
            }
        }
    }
}
