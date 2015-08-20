using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKLibrary.UTIL
{
    public class HKFileHelper
    {
        // 폴더 생성
        public static void MakeFolder(string foldername)
        {
            System.IO.Directory.CreateDirectory(foldername);
        }

        // 현재 폴더 가져오기
        public static string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        // 파일 이름 변경 기능
        public static bool SaveToFile(string filepath, string addstring)
        {
            bool bResult = true;
            
            try
            {
                System.IO.File.WriteAllText(filepath, addstring);
            }
            catch (System.Exception ex)
            {
                bResult = false;
            }
            
            return bResult;
        }
        // 파일 저장 기능
        public static bool AddToFile(string filepath, string addstring)
        {
            bool bResult = true;
            DateTime nowtime = DateTime.Now;
            
            string savestring = string.Format(@"[{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}] : {6}", nowtime.Year, nowtime.Month, nowtime.Day
                , nowtime.Hour, nowtime.Minute, nowtime.Second, addstring);

            try
            {
                string[] pS = new string[] { savestring };
                //System.IO.File.AppendAllText(filepath, savestring);
                System.IO.File.AppendAllLines(filepath, pS);
            }
            catch (System.Exception ex)
            {
                bResult = false;
            }

            return bResult;
        }
    }
}
