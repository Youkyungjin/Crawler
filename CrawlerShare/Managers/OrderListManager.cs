using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LQStructures;

namespace CrawlerShare
{
    public class OrderManager : BaseSingleton<OrderManager>
    {
        Dictionary<string, tblOrderData> OrderDBList_ = new Dictionary<string, tblOrderData>();         // DB 에서 얻어온 구매 내역      
        Dictionary<string, tblOrderData> WrongOrderList_ = new Dictionary<string, tblOrderData>();      // 잘못된 구매 내역
        Dictionary<string, tblOrderData> OrderExcelList_ = new Dictionary<string, tblOrderData>();      // 엑셀에서 읽어온 구매 내역
        Dictionary<string, string> GoodsDownInfo_ = new Dictionary<string, string>();                   // 다운로드 받은 엑셀 파일 위치 정보

        public void Init()
        {
            GoodsDownInfo_.Clear();
            OrderDBList_.Clear();
            WrongOrderList_.Clear();
            OrderExcelList_.Clear();
        }
        
        public bool AddGoodsData(string goodscode, string downfilepath)
        {
            if (GoodsDownInfo_.ContainsKey(goodscode) == true)
                return false;

            GoodsDownInfo_.Add(goodscode, downfilepath);
            return true;
        }

        public Dictionary<string, string> GetGoodsList()
        {
            return GoodsDownInfo_;
        }

        public bool AddOrderData(tblOrderData pData)
        {
            if (OrderDBList_.ContainsKey(pData.channelOrderCode_) == true)
                return false;

            OrderDBList_.Add(pData.channelOrderCode_, pData);

            return true;
        }

        public Dictionary<string, tblOrderData> GetOrderList()
        {
            return OrderDBList_;
        }

        public bool AddWrongData(tblOrderData pData)
        {
            if (WrongOrderList_.ContainsKey(pData.channelOrderCode_) == true)
                return false;

            WrongOrderList_.Add(pData.channelOrderCode_, pData);

            return true;
        }

        public Dictionary<string, tblOrderData> GetWrongOrderList()
        {
            return WrongOrderList_;
        }

        public bool AddExcelData(tblOrderData pData)
        {
            if (OrderExcelList_.ContainsKey(pData.channelOrderCode_) == true)
                return false;

            OrderExcelList_.Add(pData.channelOrderCode_, pData);

            return true;
        }

        public Dictionary<string, tblOrderData> GetExcelOrderList()
        {
            return OrderExcelList_;
        }
    }
}
