using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LQStructures
{
    public class ChannelGoodInfo
    {
        public void Init()
        {
            Idx_ = -1;
            Goods_Code_ = "";
            GoodsName_ = "";
            GoodsNickName_ = "";
            GoodsPassType_ = "";
            GoodsSendType_ = -1;
            sDate_ = "";
            eDateFormat_ = "";
            OptionName_ = "";
            OptionNickName_ = "";
            Expired_ = false;
        }

        public Int32 Idx_ = -1;
        public string Goods_Code_ = "";
        public string GoodsName_ = "";
        public string GoodsNickName_ = "";
        public string GoodsPassType_ = "";
        public Int32 GoodsSendType_ = -1;
        public string sDate_ = "";
        public string eDateFormat_ = "";
        public string OptionName_ = "";
        public string OptionNickName_ = "";
        public Int32 GoodsAttrType_ = 0;
        public bool Expired_ = false;
        public DateTime availableDateTime_ = new DateTime();
    }
}
