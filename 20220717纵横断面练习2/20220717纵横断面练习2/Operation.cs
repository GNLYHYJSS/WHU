using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _20220717纵横断面练习2
{
    class Operation
    {
        static public double G_A(Point p1, Point p2)
        {
            double A = -1;
            double dy = p2.y - p1.y;
            double dx = p2.x - p1.x;
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
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }
        static public void IDW(List<Point> Points, Point p0, int n)
        {
            List<Point> Q = new List<Point>(n);
            foreach (Point pi in Points)
            {
                pi.d = Dist(p0, pi);
                if (Q.Count < n)
                {
                    Q.Add(pi);
                }
                else
                {
                    int index = 0; double max = double.MinValue;
                    for (int i = 0; i < Q.Count; i++)
                    {
                        if (Q[i].d > max) { max = Q[i].d; index = i; }
                    }
                    if (pi.d < max) { Q.Remove(Q[index]); Q.Add(pi); }
                }
            }
            double fz = 0, fm = 0;
            foreach (Point p in Q)
            {
                fz += p.h / p.d;
                fm += 1 / p.d;
            }
            p0.h = fz / fm;
        }
        static public double G_S(List<Point> DM, double h0)
        {
            double S = 0;
            for (int i = 0; i < DM.Count - 1; i++)
            {
                Point pi = DM[i];
                Point pj = DM[i + 1];
                S += (pi.h + pj.h - 2 * h0) / 2 * (pj.K - pi.K);
            }
            return S;
        }
        static public void ZDMJS(List<Point> ZDM, List<Point> Points, List<Point> Keys, ref double ZDM_D, double dl)
        {
            Keys[0].K = 0;
            ZDM.Add(Keys[0]);
            for (int i = 0; i < Keys.Count - 1; i++)
            {
                Keys[i + 1].K = Keys[i].K + Dist(Keys[i], Keys[i + 1]);
                ZDM.Add(Keys[i + 1]);
            }
            ZDM_D = Keys[2].K;
            int count = 1;
            for (int i = 1; i * dl < ZDM_D; i++)
            {
                Point pi = new Point();
                pi.name = "V" + count;
                pi.K = i * dl;
                if (pi.K < Keys[1].K)
                {
                    pi.x = Keys[0].x + pi.K * Math.Cos(G_A(Keys[0], Keys[1]));
                    pi.y = Keys[0].y + pi.K * Math.Sin(G_A(Keys[0], Keys[1]));
                    IDW(Points, pi, 20);
                }
                else
                {
                    pi.x = Keys[1].x + (pi.K - Keys[1].K) * Math.Cos(G_A(Keys[1], Keys[2]));
                    pi.y = Keys[1].y + (pi.K - Keys[1].K) * Math.Sin(G_A(Keys[1], Keys[2]));
                    IDW(Points, pi, 20);
                }
                ZDM.Add(pi);
                count++;
            }
        }
        static public void HDMJS(List<Point> HDM1, List<Point> HDM2, List<Point> Points, List<Point> Keys)
        {
            Point M = new Point(); Point N = new Point();
            M.name = "M";
            M.x = (Keys[0].x + Keys[1].x) / 2;
            M.y = (Keys[0].y + Keys[1].y) / 2;
            IDW(Points, M, 20);
            N.name = "N";
            N.x = (Keys[1].x + Keys[2].x) / 2;
            N.y = (Keys[1].y + Keys[2].y) / 2;
            IDW(Points, N, 20);
            for (int i = -5; i <= 5; i++)
            {
                if (i == 0)
                {
                    M.K = 25;
                    N.K = 25;
                    HDM1.Add(M);
                    HDM2.Add(N);
                }
                else
                {
                    Point pmi = new Point();
                    pmi.name = "C-(" + i + ")";
                    pmi.K = i * 5 + 25;
                    pmi.x = M.x + i * 5 * Math.Cos(G_A(Keys[0], Keys[1]) + Math.PI / 2);
                    pmi.y = M.y + i * 5 * Math.Sin(G_A(Keys[0], Keys[1]) + Math.PI / 2);
                    IDW(Points, pmi, 20);
                    Point pni = new Point();
                    pni.name = "C-(" + i + ")";
                    pni.K = i * 5 + 25;
                    pni.x = N.x + i * 5 * Math.Cos(G_A(Keys[1], Keys[2]) + Math.PI / 2);
                    pni.y = N.y + i * 5 * Math.Sin(G_A(Keys[1], Keys[2]) + Math.PI / 2);
                    IDW(Points, pni, 20);
                    HDM1.Add(pmi);
                    HDM2.Add(pni);
                }
            }
        }
    }
}
