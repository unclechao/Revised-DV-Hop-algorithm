using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Revised_DV_Hop_algorithm
{
    public class DataExport
    {
        /// <summary>
        /// 将实验结果数据导出到Excel中
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void DataExportToExcel(NodeList nodeList, string filePath)
        {
            List<Node> generalNodeList = nodeList.GetAllGeneralNode();
            IWorkbook workBook = new HSSFWorkbook((new FileStream(filePath, FileMode.Open)));
            ISheet sheet = workBook.GetSheetAt(0);
            int rowCount = sheet.LastRowNum ;
            for (int i = rowCount; i < generalNodeList.Count + rowCount; i++)
            {
                int connectivity = 0;
                //计算节点连通度
                foreach (int value in generalNodeList[i - rowCount].HopCountTable.Values)
                {
                    if (value == 1)
                    {
                        connectivity++;
                    }
                }
                //创建第i行
                IRow row = sheet.CreateRow(i+1);
                //8列，分别为节点ID、通信半径、连通度、实际坐标X、实际坐标Y、估计坐标X、估计坐标Y
                ICell cell0 = row.CreateCell(0, CellType.NUMERIC);
                cell0.SetCellValue(generalNodeList[i - rowCount].Id);
                ICell cell1 = row.CreateCell(1, CellType.NUMERIC);
                cell1.SetCellValue(generalNodeList[i - rowCount].CommunicationRadius);
                ICell cell2 = row.CreateCell(2, CellType.NUMERIC);
                cell2.SetCellValue(connectivity);
                ICell cell3 = row.CreateCell(3, CellType.NUMERIC);
                cell3.SetCellValue(generalNodeList[i - rowCount].RealX);
                ICell cell4 = row.CreateCell(4, CellType.NUMERIC);
                cell4.SetCellValue(generalNodeList[i - rowCount].RealY);
                ICell cell5 = row.CreateCell(5, CellType.BOOLEAN);
                cell5.SetCellValue(((GeneralNode)generalNodeList[i - rowCount]).IsAlreadyLocated);
                ICell cell6 = row.CreateCell(6, CellType.NUMERIC);
                cell6.SetCellValue(((GeneralNode)generalNodeList[i - rowCount]).EstimatedX);
                ICell cell7 = row.CreateCell(7, CellType.NUMERIC);
                cell7.SetCellValue(((GeneralNode)generalNodeList[i - rowCount]).EstimatedY);
            }
            //文件保存
            using (Stream s = File.OpenWrite(filePath))
            {
                workBook.Write(s);
                Console.WriteLine("Data Export Success！");
            }
        }
    }
}
