using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKLibrary.UTIL
{
    public class ProcessChecker
    {
        // 현재 프로그램 메모리 사용량 가져오기
        public static Int64 GetUsageMemory()
        {
            return System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;
        }
    }
}
