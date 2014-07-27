using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class NodeList : ArrayList
    {
        //所有节点保存在List中
        private List<Node> nodes;
        public List<Node> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        //全网平均跳距
        private double avgHopSize;
        public double AvgHopSize
        {
            get { return avgHopSize; }
            set { avgHopSize = value; }
        }
        //全网修正值
        private double revisedSumValue;
        public double RevisedSumValue
        {
            get { return revisedSumValue; }
            set { revisedSumValue = value; }
        }
        //最终全网平均每跳距离
        private double finalHopSize;
        public double FinalHopSize
        {
            get { return finalHopSize; }
            set { finalHopSize = value; }
        }
        //节点ID计数器
        static int n = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="beaconNodeCount">信标节点数量</param>
        /// <param name="unknownNodeCount">信未知节点数量</param>
        /// <param name="communicationRadius">节点通信半径</param>
        /// <param name="areaX">网络区域横向坐标</param>
        /// <param name="areaY">网络区域纵向坐标</param>
        public NodeList(int beaconNodeCount, int generalNodeCount, double communicationRadius, int areaX, int areaY)
        {
            Random rd = new Random();
            nodes = new List<Node>();
            //添加未知节点
            for (int i = 0; i < generalNodeCount; i++)
            {
                nodes.Add(new GeneralNode(rd.Next(0, areaX), rd.Next(0, areaY), communicationRadius));
            }
            //添加信标节点
            for (int i = 0; i < beaconNodeCount; i++)
            {
                nodes.Add(new BeaconNode(rd.Next(0, areaX), rd.Next(0, areaY), communicationRadius));
            }
            //添加节点ID
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Id = n++;
            }
        }

        /// <summary>
        /// 根据Id值查找节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns>若没找到则返回null</returns>
        public Node GetNodeById(int id)
        {
            foreach (Node n in nodes)
            {
                if (n.Id == id)
                {
                    return n;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取节点列表中所有节点的邻居节点
        /// </summary>
        /// <param name="nodeList"></param>
        public void GetNeighbourNode(NodeList nodeList)
        {
            //遍历所有节点，取得节点的邻居节点
            foreach (Node n in nodeList.Nodes)
            {
                n.GetAHopNode(nodeList.Nodes);
            }
        }

        /// <summary>
        /// 获取网络中所有节点的路由信息表
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="j"></param>
        public void GetNodeAllHop(NodeList nodeList, int j)
        {
            //遍历若干次，得到网络节点的跳数
            for (int i = 0; i < j; i++)
            {
                nodeList.GetAllHop();
            }
        }

        /// <summary>
        /// 取得所有可到达的节点跳数
        /// </summary>
        public void GetAllHop()
        {
            //遍历网络内所有节点
            for (int i = 0; i < nodes.Count; i++)
            {
                //遍历该节点的hopCountTable字段
                int t = nodes[i].HopCountTable.Count;
                for (int j = 0, a = 0; j < t; j++)
                {
                    //遍历j节点的HopCountTable
                    List<int> tempList1 = new List<int>();
                    foreach (int key in nodes[i].HopCountTable.Keys)
                    {
                        tempList1.Add(key);
                    }
                    List<int> tempList2 = new List<int>();
                    foreach (int key in GetNodeById(tempList1[a++]).HopCountTable.Keys)
                    {
                        tempList2.Add(key);
                    }
                    for (int k = 0; k < GetNodeById(tempList1[j]).HopCountTable.Count; k++)
                    {
                        if (tempList2[k] != nodes[i].Id && !nodes[i].HopCountTable.ContainsKey(tempList2[k]))
                        {
                            //加入到i节点的hopCountTable中,跳数加一
                            nodes[i].HopCountTable.Add(tempList2[k], GetNodeById(tempList1[j]).HopCountTable[tempList2[k]] + 1);
                        }
                        else if (tempList2[k] != nodes[i].Id && nodes[i].HopCountTable.ContainsKey(tempList2[k]))
                        {
                            //保留最小跳数
                            //是否缺少条件分支
                            if (nodes[i].HopCountTable[tempList2[k]] > GetNodeById(tempList1[j]).HopCountTable[tempList2[k]] + nodes[i].HopCountTable[GetNodeById(tempList1[j]).Id])
                            {
                                nodes[i].HopCountTable[tempList2[k]] = GetNodeById(tempList1[j]).HopCountTable[tempList2[k]] + nodes[i].HopCountTable[GetNodeById(tempList1[j]).Id];
                            }
                        }
                        else if (tempList2[k] == nodes[i].Id && (GetNodeById(tempList1[j]).HopCountTable[nodes[i].Id] > nodes[i].HopCountTable[tempList1[j]]))
                        {
                            GetNodeById(tempList1[j]).HopCountTable[nodes[i].Id] = nodes[i].HopCountTable[tempList1[j]];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查找返回所有未知节点
        /// </summary>
        /// <returns></returns>
        public List<Node> GetAllGeneralNode()
        {
            List<Node> generalNodeList = new List<Node>();
            foreach (Node n in this.Nodes)
            {
                if (!n.IsBeaconNode)
                {
                    generalNodeList.Add(n);
                }
            }
            return generalNodeList;
        }

        /// <summary>
        /// 查找返回所有信标节点
        /// </summary>
        /// <returns></returns>
        public List<Node> GetAllBeaconNode()
        {
            List<Node> beaconNodeList = new List<Node>();
            foreach (Node n in this.Nodes)
            {
                if (n.IsBeaconNode)
                {
                    beaconNodeList.Add(n);
                }
            }
            return beaconNodeList;
        }
    }
}
