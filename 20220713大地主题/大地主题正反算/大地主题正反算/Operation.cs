using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 大地主题正反算
{
    class Operation
    {
        static public double DTH(double D)//ddmmss转弧度
        {      
            double d = Math.Floor(D);
            double f = Math.Floor((D - d) * 100);
            double m = ((D - d) * 100 - f) * 100;
            return (d + f / 60 + m / 3600) / 180 * Math.PI;
        }
        static public double HTD(double H)//弧度转度
        {
            return H / Math.PI * 180;
        }
        static public void ZS(GCZ gcz, TQ tq)
        {
            double B1 = DTH(gcz.p1.B);
            double A1 = DTH(gcz.p1.A);
            //计算起点的归化维度
            double W1 = Math.Sqrt(1 - tq.e2 * Math.Pow(Math.Sin(B1), 2));
            double sinu1 = (Math.Sin(B1) * Math.Sqrt(1 - tq.e2)) / W1;
            double cosu1 = Math.Cos(B1) / W1;
            //计算辅助函数值
            double sinA0 = cosu1 * Math.Sin(A1);
            double cotσ1 = cosu1 * Math.Cos(A1) / sinu1;
            double σ1 = Math.Atan(1 / cotσ1);
            //计算系数
            double A = 0, B = 0, C = 0;
            double e2 = tq.e2;
            double cos_2A0 = 1 - Math.Pow(sinA0, 2);
            double k2 = tq.e_2 * Math.Pow(cos_2A0, 2);
            A = (1 - k2 / 4 + 7 * k2 * k2 / 64 + 15 * k2 * k2 * k2 / 256) / tq.b;
            B = (k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512);
            C = k2 * k2 / 128 - k2 * k2 * k2 / 128;
            double α = (e2 / 2 + e2 * e2 / 8 + e2 * e2 * e2 / 16) - (e2 * e2 / 16 + e2 * e2 * e2 / 16) * cos_2A0 + 3 * e2 * e2 * e2 / 1280 * cos_2A0 * cos_2A0;
            double β = (e2 * e2 / 16 + e2 * e2 * e2 / 16) * cos_2A0 - (e2 * e2 * e2 / 32) * cos_2A0 * cos_2A0;
            double γ = (e2 * e2 * e2 / 256) * cos_2A0 * cos_2A0;
            double σ = A * gcz.S;
            double σ_ = A * gcz.S+10;
            while (Math.Abs(σ_- σ)>Math.Pow(1,-10))
            {
                σ_ = σ;
                σ = A * gcz.S + B * Math.Sin(σ) * Math.Cos(2 * σ1 + σ) + C * Math.Sin(2 * σ) * Math.Cos(4 * σ1 + 2 * σ);
            }
            double δ = (α * σ + β * Math.Cos(2 * σ1 + σ) * Math.Sin(σ) + γ * Math.Sin(2 * σ) * Math.Cos(4 * σ1 + 2 * σ)) * sinA0;
            double sinu2 = sinu1 * Math.Cos(σ)  + cosu1 * Math.Cos(A1) * Math.Sin(σ);
            double B2 = Math.Atan(sinu2 / (Math.Sqrt(1 - e2) * Math.Sqrt(1 - Math.Pow(sinu2, 2))));
            double λ = Math.Atan(Math.Sin(A1) * Math.Sin(σ) / (cosu1 * Math.Cos(σ) - sinu1 * Math.Sin(σ) * Math.Cos(A1)));
            if (Math.Sin(A1) > 0 && Math.Tan(λ) > 0) { λ = Math.Abs(λ); }
            if (Math.Sin(A1) > 0 && Math.Tan(λ) < 0) { λ = Math.PI - Math.Abs(λ); }
            if (Math.Sin(A1) < 0 && Math.Tan(λ) < 0) { λ = -Math.Abs(λ); }
            if (Math.Sin(A1) < 0 && Math.Tan(λ) > 0) { λ = Math.Abs(λ) - Math.PI; }
            double L2 = DTH(gcz.p1.L) + λ - δ;
            double A2 = Math.Atan(cosu1 * Math.Sin(A1) / (cosu1 * Math.Cos(σ) * Math.Cos(A1) - sinu1 * Math.Sin(σ)));
            if (Math.Sin(A1) > 0 && Math.Tan(A2) > 0) { A2 = Math.Abs(A2); }
            if (Math.Sin(A1) > 0 && Math.Tan(A2) < 0) { A2 = Math.PI - Math.Abs(A2); }
            if (Math.Sin(A1) < 0 && Math.Tan(A2) < 0) { A2 = Math.PI + Math.Abs(A2); }
            if (Math.Sin(A1) < 0 && Math.Tan(A2) > 0) { A2 = Math.PI * 2 - Math.Abs(A2); }
            if (A2 < 0) { A2 += Math.PI * 2; }
            if (A2 > Math.PI * 2) { A2 -= Math.PI * 2; }
            gcz.p1.B = HTD(DTH(gcz.p1.B));
            gcz.p1.L = HTD(DTH(gcz.p1.L));
            gcz.p1.A = HTD(DTH(gcz.p1.A));
            gcz.p2.B = HTD(B2);
            gcz.p2.L = HTD(L2);
            gcz.p2.A = HTD(A2);
        }
        static public void FS(GCZ gcz,TQ tq)
        {
            double B1 = DTH(gcz.p1.B);
            double B2 = DTH(gcz.p2.B);
            //辅助计算
            double u1 = Math.Atan(Math.Sqrt(1 - tq.e2) * Math.Tan(B1));
            double u2 = Math.Atan(Math.Sqrt(1 - tq.e2) * Math.Tan(B2));
            double l = DTH(gcz.p2.L - gcz.p1.L);//弧度
            double a1 = Math.Sin(u1) * Math.Sin(u2);
            double a2 = Math.Cos(u1) * Math.Cos(u2);
            double b1 = Math.Cos(u1) * Math.Sin(u2);
            double b2 = Math.Sin(u1) * Math.Cos(u2);
            #region 计算起点大地方位角
            double δ = 0;
            double δ1 = 10;
            double A1 = 0;
            double λ = 0;
            double σ1 = 0;
            double σ = 0;
            double A = 0, B = 0, C = 0;
            while (Math.Abs(δ1 - δ) > Math.Pow(1, -10)) 
            {
                δ1= δ;
                λ = l + δ;
                double p = Math.Cos(u2) * Math.Sin(λ);
                double q = b1 - b2 * Math.Cos(λ);
                A1 = Math.Atan(p / q);
                if (p > 0 && q > 0) { A1 = Math.Abs(A1); }
                if (p > 0 && q < 0) { A1 = Math.PI - Math.Abs(A1); }
                if (p < 0 && q < 0) { A1 = Math.PI + Math.Abs(A1); }
                if (p < 0 && q > 0) { A1 = 2 * Math.PI - Math.Abs(A1); }
                if (A1 < 0) { A1 += 2 * Math.PI; }
                if (A1 > Math.PI*2) { A1 -= 2 * Math.PI; }
                double sinσ = p * Math.Sin(A1) + q * Math.Cos(A1);
                double cosσ = a1 + a2 * Math.Cos(λ);
                σ = Math.Atan(sinσ/cosσ);
                if (cosσ > 0) { σ = Math.Abs(σ); }
                if (cosσ < 0) { σ =Math.PI- Math.Abs(σ);  }
                double A0 = Math.Cos(u1) * Math.Sin(A1);
                σ1 = Math.Tan(Math.Tan(u1) / Math.Cos(A1));
                //计算系数
                double e2 = tq.e2;
                double cos_2A0 = 1 - Math.Pow(Math.Sin(A0), 2);
                double k2 = tq.e_2 * Math.Pow(Math.Cos(A0), 2);
                A = (1 - k2 / 4 + 7 * k2 * k2 / 64 + 15 * k2 * k2 * k2 / 256) / tq.b;
                B = (k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512);
                C = k2 * k2 / 128 - k2 * k2 * k2 / 128;
                double α = (e2 / 2 + e2 * e2 / 8 + e2 * e2 * e2 / 16) - (e2 * e2 / 16 + e2 * e2 * e2 / 16) * cos_2A0 + 3 * e2 * e2 * e2 / 1280 * cos_2A0 * cos_2A0;
                double β = (e2 * e2 / 16 + e2 * e2 * e2 / 16) * cos_2A0 - (e2 * e2 * e2 / 32) * cos_2A0 * cos_2A0;
                double γ = (e2 * e2 * e2 / 256) * cos_2A0 * cos_2A0;
                δ=(α* σ+ β*Math.Cos(2* σ1+ σ)*Math.Sin(σ)+γ*Math.Sin(2* σ)*Math.Cos(4* σ1+2* σ))*Math.Sin(A0);
            }
            #endregion
            //计算大地线长度
            σ1 = Math.Tan(Math.Tan(u1) / Math.Cos(A1));
            double xs = C * Math.Sin(2 * σ) * Math.Cos(4 * σ1 + 2 * σ);
            double S = (σ - B * Math.Sin(σ) * Math.Cos(2 * σ1 + σ) - xs) / A;
            double A2 = Math.Atan(Math.Cos(u1) * Math.Sin(λ) / (b1 * Math.Cos(λ) - b2));
            if (A2 < 0) { A2 += 2 * Math.PI; }
            if (A2 > 2 * Math.PI) { A2 -= Math.PI * 2; }
            if (A1 < Math.PI && A2 < Math.PI) { A2 += Math.PI; }
            if (A1 > Math.PI && A2 > Math.PI) { A2 -= Math.PI; }
            gcz.p1.B = HTD(DTH(gcz.p1.B));
            gcz.p1.L = HTD(DTH(gcz.p1.L));
            gcz.p2.B = HTD(DTH(gcz.p2.B));
            gcz.p2.L = HTD(DTH(gcz.p2.L));
            gcz.S = S;
            gcz.p1.A = HTD(A1);
            gcz.p2.A = HTD(A2);
        }
    }
}
