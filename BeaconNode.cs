using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class BeaconNode : Node
    {
        //每跳距离修正值
        private double revisedValue;
        public double RevisedValue
        {
            get { return revisedValue; }
            set { revisedValue = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">节点实际坐标X</param>
        /// <param name="y">节点实际坐标X</param>
        /// <param name="radius">节点通信半径</param>
        public BeaconNode(double x, double y, double radius)
        {
            this.communicationRadius = radius;
            this.realX = x;
            this.realY = y;
            this.IsBeaconNode = true;
            this.HopCountTable = new Dictionary<int, int>();
        }
    }
}
