using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 大地主题正反算
{
    class Point
    {
        public string name;
        public double B, L, A;
    }
    class GCZ
    {
        public Point p1=new Point(), p2 = new Point();
        public double S;//大地线
    }
    class TQ//椭球
    {
        public double a, b, f, e2, e_2;
        public TQ(double a,double f_ds)//构造函数
        {
            this.a = a;
            f = 1 / f_ds;
            b = a - a * f;
            e2 = (Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(a, 2);
            e_2 = e2/(1-e2);
        }
    }
}
