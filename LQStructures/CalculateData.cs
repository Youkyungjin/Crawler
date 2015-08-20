using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LQStructures
{
    public class tblOrderData
    {
        public enum NeedDBProc
        {
            None = 0,
            Insert,
            Update,
        }

        public void CopyFrom(tblOrderData srcData)
        {
            NeedDBProc_ = srcData.NeedDBProc_;
            bFindInExcel_ = srcData.bFindInExcel_;
            bProcessed_ = srcData.bProcessed_;
            seq_ = srcData.seq_;
            goodsSeq_ = srcData.goodsSeq_;
            memberSeq_ = srcData.memberSeq_;
            channelSeq_ = srcData.channelSeq_;
            goodsCode_ = srcData.goodsCode_;
            channelOrderCode_ = srcData.channelOrderCode_;
            orderReserveCode_ = srcData.orderReserveCode_;
            orderID_ = srcData.orderID_;
            orderSettlePrice_ = srcData.orderSettlePrice_;
            orderName_ = srcData.orderName_;
            orderPhone_ = srcData.orderPhone_;
            orderMethod_ = srcData.orderMethod_;
            orderEtc1_ = srcData.orderEtc1_;
            orderEtc2_ = srcData.orderEtc2_;
            orderTotalPrice_ = srcData.orderTotalPrice_;
            orderCouponPrice_ = srcData.orderCouponPrice_;
            orderPointPrice_ = srcData.orderPointPrice_;
            addPoint_ = srcData.addPoint_;
            State_ = srcData.State_;
            BuyDate_ = srcData.BuyDate_;
            ExData_GoodsName_ = srcData.ExData_GoodsName_;
            ExData_OptionOriginal_ = srcData.ExData_OptionOriginal_;
            BuyCount_ = srcData.BuyCount_;
        }

        public NeedDBProc NeedDBProc_ = NeedDBProc.None;
        public bool bFindInExcel_ = false;  // 엑셀에서 데이터를 찾았는가?
        public bool bProcessed_ = false;    // 이번 크롤링에서 이미 처리 했는가?

        public Int64 seq_ = -1;
        public Int32 goodsSeq_ = -1;                // 상품 시퀀스( Goods Table 에 있는 index )
        public Int32 memberSeq_ = -1;               // 맴버 시퀀스
        public Int32 channelSeq_ = -1;              // 채널 시퀀스
        public Int32 authoritySeq_ = -1;            // 권리사 시퀀스
        public string goodsCode_ = "";              // 상품코드
        public string channelOrderCode_ = "";       // 채널 주문코드(쿠폰코드)
        public string orderReserveCode_ = "";       // 예약 코드
        public string orderID_ = "";                // 주문자 아이디
        public float orderSettlePrice_ = 0; // 주문 금액
        public string orderName_ = "";      // 주문자 이름
        public string orderPhone_ = "";     // 주문 전화 
        public string orderMethod_ = "";    // 주문 방법
        public string orderEtc1_ = "";
        public string orderEtc2_ = "";
        public float orderTotalPrice_ = 0;
        public float orderCouponPrice_ = 0;
        public float orderPointPrice_ = 0;
        public float addPoint_ = 0;
        public string State_ = "";          // 상태
        public string BuyDate_ = "";        // 구매일
        public Int32 BuyCount_ = 0;       //구매 갯수(옵션명에 갯수가 없고 따로 빠져있는경우)   

        // 필요에 의해 넣은값.
        public string ExData_GoodsName_ = "";       // 상품명
        public string ExData_Option_ = "";          // 옵션명
        public string ExData_OptionOriginal_ = "";  // 정규식 통하지 않은 원래 옵션명
        public string ExData_Cancel_ = "";          // 취소했는가 체크
        public string ExData_Use_ = "";             // 사용했는가 체크
    }
}
