using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace HKLibrary.Excel
{
    public class HKExcelHelper
    {
        //public static string HtmlToExcelTest(string filepath)
        //{
        //    Application oXL = new Application();
        //    Workbook oWB = oXL.Workbooks.Open(filepath);//@"d:\dailyCouponsList_20140324132927.xls");
        //    //Worksheet oSheet = oWB.Worksheets["dailyCouponsList_20140324132927"];
        //    Worksheet oSheet = oWB.Worksheets[1];
        //    // 이렇게 데이터를 가져올수 있고
        //    Range oRng = oSheet.Cells[5, 2];
        //    // 이렇게도 가져올수 있다.
        //    //Range oRng = oSheet.get_Range("A5", Type.Missing);

        //    string result = oRng.Value;
            
        //    oWB.Close(true);
        //    oXL.Quit();

        //    Marshal.FinalReleaseComObject(oSheet);
        //    Marshal.FinalReleaseComObject(oWB);
        //    Marshal.FinalReleaseComObject(oXL);

        //    return result;
        //}

        public static bool GetWorkSheet(string filepath, ref Application ap, ref Workbook wb, ref Worksheet ws)
        {
            try
            {
                ap = new Application();
                wb = ap.Workbooks.Open(filepath);
                ws = wb.Worksheets[1];
            }
            catch (System.Exception ex)
            {
                return false;
            }
            
            return true;
        }

        // CSV 형태의 파일을 로드할때 사용한다.
        public static bool GetWorkSheetFromText(string filepath, ref Application ap, ref Workbook wb, ref Worksheet ws)
        {
            try
            {
                ap = new Application();
                wb = ap.Workbooks.Open(filepath, Type.Missing, Type.Missing, 2);
                ws = wb.Worksheets[1];
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        //public string GetPos(Int32 nRow, Int32 nColumn)
        //{
        //    string sResult = "Check";

        //    return sResult;
        //}
    }
}
