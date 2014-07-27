using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 20; i++)
            {
                NodeList nodeList = new NodeList(20, 80, 15, 100, 100);
                AlgorithmFunction.AlgorithmPreparation(nodeList, 15);
                ////质心算法
                //AlgorithmFunction.CenterOfMass_algorithm(nodeList, 1);
                //DataExport.DataExportToExcel(nodeList, @"d:/COM.xls");
                ////DV-Hop算法
                List<Node> generalNodeList = nodeList.GetAllGeneralNode();
                foreach (GeneralNode gn in generalNodeList)
                {
                    gn.EstimatedX = gn.EstimatedY = 0d;
                    gn.IsLocatable = gn.IsAlreadyLocated = false;
                }
                AlgorithmFunction.DV_Hop_algorithm(nodeList);
                DataExport.DataExportToExcel(nodeList, @"d:/DV-Hop.xls");
                //Revised DV-Hop算法
                foreach (GeneralNode gn in generalNodeList)
                {
                    gn.EstimatedX = gn.EstimatedY = 0d;
                    gn.IsLocatable = gn.IsAlreadyLocated = false;
                }
                AlgorithmFunction.Revised_DV_Hop_algorithm(nodeList, 5);
                DataExport.DataExportToExcel(nodeList, @"d:/Revised-DV-Hop.xls"); 
            }
            Console.WriteLine("==========Done==========");
            Console.ReadKey();
        }
    }
}
