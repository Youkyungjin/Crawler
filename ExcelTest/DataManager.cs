using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelTest
{
    public class SellData
    {
        public bool bNeedInsert_ = false;
        public string CouponNumber_ = "";
        public string UserName_ = "";
        public float Cost_ = 0;

        public void Clear()
        {
            bNeedInsert_ = false;
            CouponNumber_ = "";
            UserName_ = "";
            Cost_ = 0;
        }
    }

    class DataManager : BaseSingleton<DataManager>
    {
        Int32 MaxCount_ = 100000;
        Int32 nCursor_ = 0;

        //Dictionary<string, SellData> ListSellData_ = new Dictionary<string, SellData>();
        List<SellData> ListSellData_ = new List<SellData>();
        //Array p = new Array<SellData>();
        //Array<SellData> ListSellData_ = new Array<SellData>();
        public void MakeList()
        {
            for (Int32 i = 0; i < MaxCount_; i++)
            {
                SellData pSellData = new SellData();
                ListSellData_.Add(pSellData);
            }

            nCursor_ = 0;
        }

        public void InitAllList()
        {
            nCursor_ = 0;
            foreach (var pData in ListSellData_)
            {
                pData.Clear();
            }
        }

        public SellData GetData()
        {
            if (nCursor_ >= MaxCount_)
                nCursor_ = 0;

            return ListSellData_[nCursor_++];
        }
    }
}
