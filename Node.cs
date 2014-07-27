using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public abstract class Node
    {
        //节点ID
        protected int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        //节点类型标示
        protected bool isBeaconNode;
        public bool IsBeaconNode
        {
            get { return isBeaconNode; }
            set { isBeaconNode = value; }
        }

        //节点通信半径
        protected double communicationRadius;
        public double CommunicationRadius
        {
            get { return communicationRadius; }
            set
            {
                if (value > 0)
                {
                    communicationRadius = value;
                }
                else
                {
                    communicationRadius = 0d;
                }
            }
        }
        //节点实际坐标X
        protected double realX;
        public double RealX
        {
            get { return realX; }
            set { realX = value; }
        }
        //节点实际坐标Y
        protected double realY;
        public double RealY
        {
            get { return realY; }
            set { realY = value; }
        }
        //存放该节点到其他节点的跳数信息
        protected Dictionary<int, int> hopCountTable;
        public Dictionary<int, int> HopCountTable
        {
            get { return hopCountTable; }
            set { hopCountTable = value; }
        }
        //平均每跳距离
        protected double avgHopSize;
        public double AvgHopSize
        {
            get { return avgHopSize; }
            set { avgHopSize = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Node() { }
        public Node(double x, double y, double radius)
        {
            this.communicationRadius = radius;
            this.realX = x;
            this.realY = y;
            this.hopCountTable = new Dictionary<int, int>();
        }

        /// <summary>
        /// 取得一跳范围内的所有节点信息
        /// </summary>
        /// <param name="nodes"></param>
        public void GetAHopNode(List<Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                double distance = Math.Sqrt((this.realX - ((Node)nodes[i]).realX) * (this.realX - ((Node)nodes[i]).realX) + (this.realY - ((Node)nodes[i]).realY) * (this.realY - ((Node)nodes[i]).realY));
                if (distance > 0 && distance < this.communicationRadius)
                {
                    this.hopCountTable.Add(((Node)nodes[i]).id, 1);
                }
            }
        }
    }
}
