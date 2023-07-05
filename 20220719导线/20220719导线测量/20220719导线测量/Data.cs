using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20220719导线测量
{
    class Point
    {
        public string name;//点名
        public double X, Y;//坐标
    }
    class GCZ//观测值
    {
        public string GCD, type;//类型
        public double value;//值
    }
    struct JD//角度结构体
    {
        public double d, f, m;//度分秒
    }
    class CZ//测站
    {
        public Point P = new Point();//测站点
        public List<GCZ> GCZs = new List<GCZ>();
        public JD β;//转角
        public double dx, dy;
    }
}
