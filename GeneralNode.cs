using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class GeneralNode : Node
    {
        //是否已经定位
        private bool isAlreadyLocated;
        public bool IsAlreadyLocated
        {
            get { return isAlreadyLocated; }
            set { isAlreadyLocated = value; }
        }
        //是否可以进行定位
        private bool isLocatable;
        public bool IsLocatable
        {
            get { return isLocatable; }
            set { isLocatable = value; }
        }
        //DV-Hop算法得出的hopsize
        private double dV_Hop_Hopsize;
        public double DV_Hop_Hopsize
        {
            get { return dV_Hop_Hopsize; }
            set { dV_Hop_Hopsize = value; }
        }
        //节点估计坐标X
        private double estimatedX;
        public double EstimatedX
        {
            get { return estimatedX; }
            set { estimatedX = value; }
        }
        //节点估计坐标Y
        private double estimatedY;
        public double EstimatedY
        {
            get { return estimatedY; }
            set { estimatedY = value; }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">节点实际坐标X</param>
        /// <param name="y">节点实际坐标Y</param>
        /// <param name="radius">节点通信半径</param>
        public GeneralNode(double x, double y, double radius) : base(x, y, radius) { }
    }
}
