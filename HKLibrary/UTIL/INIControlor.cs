using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TMS.Common
{
    public class INIControlor : IDisposable
    {

        #region #전역변수
        private bool isDisposed = false;
        //public string Path = @"c:\\test.ini";
        public string Path = @"";
        #endregion

        #region #생성 & 소멸
        /// <summary>
        /// 생성자
        /// </summary>
		public INIControlor()
		{
		}

        /// <summary>
        /// 소멸자
        /// </summary>
        ~INIControlor()
        {
            Dispose(false);
        }

        /// <summary>
        /// 모든 리소스를 제거합니다.
        /// </summary>
        /// <param name="disposing">직접호출했는지 여부</param>
        protected void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            else
            {
                isDisposed = true;
            }
        }

        /// <summary>
        /// 모든 리소스를 제거합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region #기본 생성자
        /// <summary>
        /// 기본 생성자
        /// </summary>
        /// <param name="sPath"></param>
        public INIControlor(String sPath)
        {
            Path = sPath;
        }
        #endregion

        #region #파일 읽기 함수
        /// <summary>
        /// INI파일읽기함수(섹션설정)
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        public string[] GetIniValue1(string Section)
        {
            byte[] ba = new byte[255];
            uint Flag = GetPrivateProfileSection(Section, ba, 255, Path);
            return Encoding.Default.GetString(ba).Split(new char[1] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        /// <summary>
        /// INI파일읽기함수(섹션,키값설정)
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetIniValue2(string Section, string Key)
        {
            StringBuilder sb = new StringBuilder(500);
            int Flag = GetPrivateProfileString(Section, Key, "", sb, 500, Path);
            return sb.ToString();
        }
        #endregion

        #region #파일 쓰기 함수
        /// <summary>
        /// INI파일쓰기함수(섹션,키값설정)
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool SetIniValue(string Section, string Key, string Value)
        {
            return (WritePrivateProfileString(Section, Key, Value, Path));
        }
        #endregion

        #region #INI File Control
        //=====================================================================================

        //=====================================================================================

        /// <summary>
        /// INI파일에섹션과키로검색하여값을문자열형으로읽어옵니다.
        /// </summary>
        /// <param name="lpAppName">섹션명</param>
        /// <param name="lpKeyName">키값</param>
        /// <param name="lpDefault">기본값</param>
        /// <param name="lpReturnedString">가져온문자열</param>
        /// <param name="nSize">문자열버퍼크기</param>
        /// <param name="lpFileName">파일이름</param>
        /// <returns>가져온문자열의크기</returns>
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

 

        /// <summary>
        /// INI파일에섹션과키로검색하여값을저장합니다.
        /// </summary>
        /// <param name="lpAppName">섹션명</param>
        /// <param name="lpKeyName">키값</param>
        /// <param name="lpString">저장할문자열</param>
        /// <param name="lpFileName">파일이름</param>
        /// <returns>저장성공여부</returns>
        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

 

        /// <summary>
        /// INI파일에섹션과키로검색하여값을Inteager형으로불러옵니다.
        /// </summary>
        /// <param name="lpAppName">섹션명</param>
        /// <param name="lpKeyName">키값</param>
        /// <param name="nDefault">기본값</param>
        /// <param name="lpFileName">파일이름</param>
        /// <returns> 검색된값, 해당키로검색실패시기본값으로대체됨.</returns>
        [DllImport("kernel32")]
        public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

 

        /// <summary>
        /// INI파일에섹션으로검색하여키와값을Pair형태로가져옵니다.
        /// </summary>
        /// <param name="IpAppName">섹션명</param>
        /// <param name="IpPairValues">Pair한키와값을담을배열</param>
        /// <param name="nSize">배열의크기</param>
        /// <param name="IpFileName">파일이름</param>
        /// <returns>읽어온바이트수</returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileSection(string IpAppName, byte[] IpPairValues, uint nSize, string IpFileName);

 

        /// <summary>
        /// INI파일의섹션을가져옵니다.
        /// </summary>
        /// <param name="IpSections">섹션의리스트를직렬화하여담을배열</param>
        /// <param name="nSize">배열의크기</param>
        /// <param name="IpFileName">파일이름</param>
        /// <returns>읽어온바이트수</returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileSectionNames(byte[] IpSections, uint nSize, string IpFileName);
        #endregion
    }
}
