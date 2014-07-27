using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class Three_edge_measurement
    {
        /// <summary>
        /// 三边测量法
        /// </summary>
        /// <param name="x1">第一个点坐标</param>
        /// <param name="y1">第一个点坐标</param>
        /// <param name="d1">未知点到第一个点距离</param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="d2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="d3"></param>
        /// <returns>待定位点坐标</returns>
        public static double[] trilateration(double x1, double y1, double d1, double x2, double y2, double d2, double x3, double y3, double d3)
        {
            double[] d = { 0.0, 0.0 };
            double a11 = 2 * (x1 - x3);
            double a12 = 2 * (y1 - y3);
            double b1 = Math.Pow(x1, 2) - Math.Pow(x3, 2) + Math.Pow(y1, 2) - Math.Pow(y3, 2) + Math.Pow(d3, 2) - Math.Pow(d1, 2);
            double a21 = 2 * (x2 - x3);
            double a22 = 2 * (y2 - y3);
            double b2 = Math.Pow(x2, 2) - Math.Pow(x3, 2) + Math.Pow(y2, 2) - Math.Pow(y3, 2) + Math.Pow(d3, 2) - Math.Pow(d2, 2);
            d[0] = (b1 * a22 - a12 * b2) / (a11 * a22 - a12 * a21);
            d[1] = (a11 * b2 - b1 * a21) / (a11 * a22 - a12 * a21);
            return d;
        }
    }
}
