using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class AlgorithmFunction
    {
        /// <summary>
        /// 算法准备阶段
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="count"></param>
        public static void AlgorithmPreparation(NodeList nodeList, int count)
        {
            //取得节点的邻居节点
            nodeList.GetNeighbourNode(nodeList);
            //得到网络节点的跳数
            nodeList.GetNodeAllHop(nodeList, count);
        }

        /// <summary>
        /// Revised DV-Hop algorithm
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="k">阈值k</param>
        public static void Revised_DV_Hop_algorithm(NodeList nodeList, int k)
        {
            //取得所有信标节点平均每跳距离
            GetBeaconNodeAvgHopSize(nodeList);
            //计算加权后节点平均每跳距离
            CalculateNodeHopSize(nodeList);
            //计算全网平均跳距
            CalculateAvgHopSize(nodeList);
            //计算信标节点跳距修正值
            CalculateBeaconNodeRevisedHopSize(nodeList);
            // 计算修正后的全网平均每跳距离
            CalculateFinalHopSize(nodeList);
            //定位未知节点
            LocateGeneralNode(nodeList, k);
        }

        /// <summary>
        /// 传统DV-Hop算法计算hopSize
        /// </summary>
        /// <param name="nodeList"></param>
        public static void DV_Hop_algorithm(NodeList nodeList)
        {
            List<Node> generalNodeList = nodeList.GetAllGeneralNode();
            foreach (GeneralNode gn in generalNodeList)
            {
                //遍历每个未知节点，求跳距
                Dictionary<int, int> beaconNodeDic = new Dictionary<int, int>();
                //取得所有与未知节点可以通信的信标节点
                foreach (int key in gn.HopCountTable.Keys)
                {
                    if (nodeList.GetNodeById(key).IsBeaconNode)
                    {
                        beaconNodeDic.Add(key, gn.HopCountTable[key]);
                    }
                }
                //找到beaconNodeDic中与未知节点跳数最小的信标节点
                int shortestHop = int.MaxValue;
                int shortestHopNodeId = -1;
                foreach (int key in beaconNodeDic.Keys)
                {
                    if (beaconNodeDic[key] < shortestHop)
                    {
                        shortestHop = beaconNodeDic[key];
                        shortestHopNodeId = key;
                    }
                }
                Node nearestBeaconNode = nodeList.GetNodeById(shortestHopNodeId);
                //找出能与nearestBeaconNode信标节点通信的其他信标节点
                Dictionary<int, int> assistLocateBeaconNodeDic = new Dictionary<int, int>();
                if (nearestBeaconNode == null)
                {
                    //如果没有能与其通信的信标节点
                    gn.IsLocatable = false;
                    gn.IsAlreadyLocated = false;
                }
                else
                {
                    foreach (int id in nearestBeaconNode.HopCountTable.Keys)
                    {
                        if (nodeList.GetNodeById(id).IsBeaconNode)
                        {
                            assistLocateBeaconNodeDic.Add(id, nearestBeaconNode.HopCountTable[id]);
                        }
                    }
                    //计算hopsize
                    double sumDistance = 0d;
                    int sumHop = 0;
                    foreach (int nodeId in assistLocateBeaconNodeDic.Keys)
                    {
                        sumDistance += CalculateNodeDistance(nearestBeaconNode, nodeList.GetNodeById(nodeId));
                        sumHop += CalculateNodeHop(nearestBeaconNode, nodeList.GetNodeById(nodeId));
                    }
                    gn.DV_Hop_Hopsize = sumDistance / sumHop;
                    //利用三边测量法定位
                    List<Node> assistLocateBeaconNode = new List<Node>();
                    foreach (int key in gn.HopCountTable.Keys)
                    {
                        if (nodeList.GetNodeById(key).IsBeaconNode && assistLocateBeaconNode.Count < 3)
                        {
                            assistLocateBeaconNode.Add(nodeList.GetNodeById(key));
                        }
                    }
                    //判断信标节点数量，若小于3，则无法进行定位
                    if (assistLocateBeaconNode.Count < 3)
                    {
                        gn.IsLocatable = false;
                    }
                    else
                    {
                        //三边测量法定位
                        double[] pos = Three_edge_measurement.trilateration(assistLocateBeaconNode[0].RealX, assistLocateBeaconNode[0].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[0]), assistLocateBeaconNode[1].RealX, assistLocateBeaconNode[1].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[1]), assistLocateBeaconNode[2].RealX, assistLocateBeaconNode[2].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[2]));
                        gn.EstimatedX = pos[0];
                        gn.EstimatedY = pos[1];
                        if (gn.EstimatedX < 0)
                        {
                            gn.EstimatedX = 0;
                        }
                        if (gn.EstimatedY < 0)
                        {
                            gn.EstimatedY = 0;
                        }
                        if (gn.EstimatedX > 100)
                        {
                            gn.EstimatedX = 100;
                        }
                        if (gn.EstimatedY > 100)
                        {
                            gn.EstimatedY = 100;
                        }
                        gn.IsLocatable = true;
                        gn.IsAlreadyLocated = true;
                    }
                }

            }
        }

        /// <summary>
        /// 质心算法
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="j">质心算法中取j跳范围内的信标节点帮助计算</param>
        public static void CenterOfMass_algorithm(NodeList nodeList, int j)
        {
            List<Node> generalNodeList = nodeList.GetAllGeneralNode();
            foreach (GeneralNode gn in generalNodeList)
            {
                //循环，进行定位
                List<Node> AssistLocateNodeList = new List<Node>();
                //将信标节点加入到协助定位的节点列表中
                foreach (int nodeId in gn.HopCountTable.Keys)
                {
                    if (nodeList.GetNodeById(nodeId).IsBeaconNode && gn.HopCountTable[nodeId] <= j)
                    {
                        AssistLocateNodeList.Add(nodeList.GetNodeById(nodeId));
                    }
                }
                if (AssistLocateNodeList.Count >= 1)
                {
                    double sumEstimatedX = 0d;
                    double sumEstimatedY = 0d;
                    foreach (BeaconNode bn in AssistLocateNodeList)
                    {
                        sumEstimatedX += bn.RealX;
                        sumEstimatedY += bn.RealY;
                    }
                    gn.IsLocatable = true;
                    gn.IsAlreadyLocated = true;
                    gn.EstimatedX = sumEstimatedX / AssistLocateNodeList.Count;
                    gn.EstimatedY = sumEstimatedY / AssistLocateNodeList.Count;
                }
            }
        }

        /// <summary>
        /// 计算两点之间实际距离
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static double CalculateNodeDistance(Node n1, Node n2)
        {
            return Math.Sqrt((n2.RealY - n1.RealY) * (n2.RealY - n1.RealY) + (n2.RealX - n1.RealX) * (n2.RealX - n1.RealX));
        }

        /// <summary>
        /// 计算两点之间跳数
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns>若两点之间无法通信，返回-1</returns>
        private static int CalculateNodeHop(Node n1, Node n2)
        {
            foreach (int key in n1.HopCountTable.Keys)
            {
                if (key == n2.Id)
                {
                    return n1.HopCountTable[key];
                }
            }
            return -1;
        }

        /// <summary>
        /// 取得信标节点平均每跳距离
        /// </summary>
        /// <param name="nodeList"></param>
        private static void GetBeaconNodeAvgHopSize(NodeList nodeList)
        {
            List<Node> beaconNodeList = nodeList.GetAllBeaconNode();
            foreach (BeaconNode bn in beaconNodeList)
            {
                int sumHop = 0;
                double sumDistance = 0;
                foreach (Node m in beaconNodeList)
                {
                    //保证信标节点之间可联通
                    if (bn.Id != m.Id && (CalculateNodeHop(bn, m) > 0))
                    {
                        sumDistance += CalculateNodeDistance(bn, m);
                        sumHop += CalculateNodeHop(bn, m);
                    }
                }
                if (sumHop != 0)
                {
                    bn.AvgHopSize = sumDistance / sumHop;
                }
            }
        }

        /// <summary>
        /// 计算未知节点加权平均每跳距离
        /// </summary>
        /// <param name="nodeList"></param>
        private static void CalculateNodeHopSize(NodeList nodeList)
        {
            List<Node> beaconNodeList = nodeList.GetAllBeaconNode();
            List<Node> generalNodeList = nodeList.GetAllGeneralNode();
            //对各个未知节点求加权平均每跳距离
            foreach (GeneralNode gn in generalNodeList)
            {
                List<Node> tempBeaconNodeList = new List<Node>();
                int sumHop = 0;
                foreach (int beaconNodeId in gn.HopCountTable.Keys)
                {
                    //将gn节点HopCountTable中所有beaconNode取出放入tempBeaconNodeList中,并保存gn到所有beaconNode的跳数和
                    if (nodeList.GetNodeById(beaconNodeId).IsBeaconNode == true)
                    {
                        tempBeaconNodeList.Add(nodeList.GetNodeById(beaconNodeId));
                        sumHop += gn.HopCountTable[beaconNodeId];
                    }
                }
                double temp = 0d;
                int tempHop = 0;
                //将tempBeaconNodeList中AvgHopDistance属性为0的节点去除
                for (int i = 0; i < tempBeaconNodeList.Count; i++)
                {
                    if (((BeaconNode)tempBeaconNodeList[i]).AvgHopSize <= 0)
                    {
                        tempBeaconNodeList.Remove(tempBeaconNodeList[i]);
                    }
                }
                foreach (BeaconNode bn in tempBeaconNodeList)
                {
                    tempHop += sumHop - CalculateNodeHop(bn, gn);
                    temp += bn.AvgHopSize * (sumHop - CalculateNodeHop(bn, gn));
                }
                if (tempHop != 0)
                {
                    gn.AvgHopSize = temp / tempHop;
                }
            }
        }

        /// <summary>
        /// 计算全网平均每跳距离
        /// </summary>
        private static void CalculateAvgHopSize(NodeList nodeList)
        {
            double sumHop = 0d;
            int generalNodeCount = 0;
            foreach (Node n in nodeList.Nodes)
            {
                //选择未知节点
                if (n.IsBeaconNode == false)
                {
                    //去除无法计算出AvgHopSize结果的未知节点
                    if (((GeneralNode)n).AvgHopSize > 0)
                    {
                        generalNodeCount++;
                        sumHop += ((GeneralNode)n).AvgHopSize;
                    }
                }
            }
            nodeList.AvgHopSize = sumHop / generalNodeCount;
        }

        /// <summary>
        /// 计算信标节点每跳距离修正值
        /// </summary>
        /// <param name="nodeList"></param>
        private static void CalculateBeaconNodeRevisedHopSize(NodeList nodeList)
        {
            List<Node> beaconNodeList = nodeList.GetAllBeaconNode();
            //分别计算每个信标节点的RevisedValue
            foreach (BeaconNode bn in beaconNodeList)
            {
                foreach (BeaconNode tempNode in beaconNodeList)
                {
                    //需要保证信标节点之间可以连通
                    if (bn.Id != tempNode.Id && CalculateNodeHop(bn, tempNode) > 0)
                    {
                        bn.RevisedValue = (CalculateNodeDistance(bn, tempNode) - CalculateNodeHop(bn, tempNode) * nodeList.AvgHopSize) / beaconNodeList.Count;
                    }
                }
            }
            foreach (BeaconNode bn in beaconNodeList)
            {
                nodeList.RevisedSumValue += bn.RevisedValue;
            }
        }

        /// <summary>
        /// 计算修正后的全网平均每跳距离
        /// </summary>
        /// <param name="nodeList"></param>
        private static void CalculateFinalHopSize(NodeList nodeList)
        {
            nodeList.FinalHopSize = nodeList.AvgHopSize + nodeList.RevisedSumValue;
        }

        /// <summary>
        /// 定位未知节点
        /// </summary>
        /// <param name="nodeList"></param>
        private static void LocateGeneralNode(NodeList nodeList, int k)
        {
            //取得所有未知节点，遍历并定位
            List<Node> generalNodeList = nodeList.GetAllGeneralNode();
            foreach (GeneralNode gn in generalNodeList)
            {
                //辅助DV-Hop算法定位节点集合
                List<Node> assistLocateBeaconNode = new List<Node>();
                //辅助质心算法定位节点集合
                List<Node> assistCOMLocateNodeList = new List<Node>();
                foreach (int key in gn.HopCountTable.Keys)
                {
                    if (nodeList.GetNodeById(key).IsBeaconNode && assistLocateBeaconNode.Count < 3)
                    {
                        assistLocateBeaconNode.Add(nodeList.GetNodeById(key));
                    }
                    if (nodeList.GetNodeById(key).IsBeaconNode && gn.HopCountTable[key] == 1)
                    {
                        assistCOMLocateNodeList.Add(nodeList.GetNodeById(key));
                    }
                }
                if (assistCOMLocateNodeList.Count >= k)
                {
                    //信标节点数量大于事先设定阈值，使用质心算法定位
                    double sumEstimatedX = 0d;
                    double sumEstimatedY = 0d;
                    foreach (BeaconNode bn in assistCOMLocateNodeList)
                    {
                        sumEstimatedX += bn.RealX;
                        sumEstimatedY += bn.RealY;
                    }
                    gn.IsLocatable = true;
                    gn.IsAlreadyLocated = true;
                    gn.EstimatedX = sumEstimatedX / assistCOMLocateNodeList.Count;
                    gn.EstimatedY = sumEstimatedY / assistCOMLocateNodeList.Count;
                }
                //判断信标节点数量，若小于3，则无法进行定位
                else if (assistLocateBeaconNode.Count < 3)
                {
                    gn.IsLocatable = false;
                }
                else
                {
                    //三边测量法定位
                    double[] pos = Three_edge_measurement.trilateration(assistLocateBeaconNode[0].RealX, assistLocateBeaconNode[0].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[0]), assistLocateBeaconNode[1].RealX, assistLocateBeaconNode[1].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[1]), assistLocateBeaconNode[2].RealX, assistLocateBeaconNode[2].RealY, nodeList.FinalHopSize * CalculateNodeHop(gn, assistLocateBeaconNode[2]));
                    gn.EstimatedX = pos[0];
                    gn.EstimatedY = pos[1];
                    if (gn.EstimatedX < 0)
                    {
                        gn.EstimatedX = 0;
                    }
                    if (gn.EstimatedY < 0)
                    {
                        gn.EstimatedY = 0;
                    }
                    if (gn.EstimatedX > 100)
                    {
                        gn.EstimatedX = 100;
                    }
                    if (gn.EstimatedY > 100)
                    {
                        gn.EstimatedY = 100;
                    }
                    gn.IsLocatable = true;
                    gn.IsAlreadyLocated = true;
                }
            }
        }
    }
}
