using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20220718Grid体积计算
{
    class Point
    {
        public double x, y, h, d;//散点二维坐标，高度和距离
        public string name;//散点名字
    }
    class GW
    {
        public Point P = new Point();//格网最小点
        public double h, w, L;//格网长度宽度间隔距离
        public List<Point> Cs = new List<Point>();//格网中心点点集 
    }
}
