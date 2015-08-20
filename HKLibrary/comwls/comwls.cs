using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using HKLibrary.UTIL;
using Tamir.SharpSsh;


namespace HKLibrary.comwls
{
    public class comwls
    {
        public static bool image_01_FileMake(string filePath)
        {
            Boolean bResult;
            string _sftpHost = "ledev.leisureq.co.kr";
            string _sftpUserId = "lion";
            string _sftpUserPw = "gkffl1!";
            Int32 _sftpPort = 10004;

            string[] path = filePath.Split(new char[] { '/' });

            try
            {
                string ppath = "";
                Sftp sftp = new Tamir.SharpSsh.Sftp(_sftpHost, _sftpUserId, _sftpUserPw);
                sftp.Connect(_sftpPort);

                for (int i = 1; i < path.Length; i++)
                {
                    ppath = ppath + "/" + path[i];
                    ArrayList res = sftp.GetFileList(ppath);
                    string checkFile = path[i+1];

                    if (res.IndexOf(checkFile) == -1)
                    {
                        sftp.Mkdir(ppath + "/" + checkFile);
                    }
             
                }
            
                bResult = true;
            }
            catch (SystemException ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error image_01_FileMake {0}", ex.Message));
                bResult = false;
            }

            return bResult;
        }


    }
}
