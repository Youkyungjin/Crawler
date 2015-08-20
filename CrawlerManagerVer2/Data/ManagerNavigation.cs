using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ManagerNavigation : BaseSingleton<ManagerNavigation>
{
    public Int32 Selected_Monitor_ = 0;     // 크롤러 리스트에서 클릭된 모니터 시퀀스
    //public Int32 Selected_Crawler_ = 0;     // 크롤러 리스트에서 클릭된 크롤러 시퀀스, 이것을 이용해서 상세 정보를 표시한다.
    public Int32 Selected_Goods_Seq_ = 0;   // 상품 변경시 선택된 값.
    //public Int32 Selected_
}
