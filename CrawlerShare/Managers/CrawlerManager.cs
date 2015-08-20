using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using LQStructures;

namespace CrawlerShare
{
    public enum CrawlerState
    {
        BEFORE_INIT = 0,    // 앱 뜨는중
        INITIATING,         // 초기화 중
        STOP,               // 크롤링 멈춤 상태( 초기화 이후 혹은 관리자가 중지시킴)
        STOPPING,           // 크롤링 중지중
        ERROR,              // 크롤링 멈춤 상태( 에러가 났기 때문에 )
        WORKING,            // 크롤링 중
    }

    public class StringData
    {
        public static string[] strCrawLerState = new string[] { "실행중", "초기화중", "중지됨", "중지중", "중지됨(에러)", "작동중" };
    }

    public class CrawlerManager : BaseSingleton<CrawlerManager>
    {
        Int32 CrawlingCount_ = 0;
        CrawlerState State_ = CrawlerState.BEFORE_INIT;
        LQChannelInfo ChannelInfo_;
        LQCrawlerInfo CrawlerInfo_;
        Dictionary<Int32, ChannelGoodInfo> GoodsInfoList_ = new Dictionary<Int32, ChannelGoodInfo>();

        LQCrawlerBase Crawler_ = null;                      // 크롤러 객체
        public ResultData ResultData_ = new ResultData();   // 크롤링 결과

        public void InitCrawler()
        {
            ChannelInfo_ = null;
            CrawlerInfo_ = null;
            Crawler_ = null;
            GoodsInfoList_.Clear();
        }

        public void AddCrawlingCount()
        {
            CrawlingCount_++;
        }

        public Int32 CrawlingCount()
        {
            return CrawlingCount_;
        }

        // 크롤링 클래스 만들기( 채널에 따라서 따로 만들어 집니다. )
        public void MakeCrawler(Int32 ChannelIndx)
        {
            //if (CrawlerInfo_ == null)
            //    return;

            if (Crawler_ != null)
                return;

            switch (ChannelIndx)
            {
                case 6: // 쿠팡
                    {
                        Crawler_ = new LQCrawlerCoupang();
                    }
                    break;
                case 7: // 티몬
                    {
                        Crawler_ = new LQCrawlerTicketMonster();
                    }
                    break;
                case 8: // 위메프
                    {
                        Crawler_ = new LQCrawlerWeMakePrice();
                    }
                    break;
                case 9: // ezwell
                    {
                        Crawler_ = new LQCrawlerEzwel();
                    }
                    break;
                case 11: // 옥션
                    {
                        string comparesite = "옥션";
                        string str_use_url_1_ = @"https://www.esmplus.com/Escrow/Order/OrderCheck";
                        string str_use_param_1_ = @"mID=140935&orderInfo={TicketCode},2,123275894";
                        string str_use_check_1_ = @":true,";

                        string str_use_url_2_ = @"https://www.esmplus.com/Escrow/Delivery/SetDoShippingGeneral";
                        string str_use_param_2_ = @"mID=140935&deliveryInfo={TicketCode},10033,직접전달, 15998370";
                        string str_use_check_2_ = @":true,";

                        Crawler_ = new LQCrawlereBay();
                        ((LQCrawlereBay)Crawler_).SetUseInfo(comparesite, str_use_url_1_, str_use_param_1_, str_use_check_1_
                            , str_use_url_2_, str_use_param_2_, str_use_check_2_);
                    }
                    break;
                case 12:    // 지마켓/지구
                    {
                        string comparesite = "지마켓";
                        string str_use_url_1_ = @"https://www.esmplus.com/Escrow/Order/OrderCheck";
                        string str_use_param_1_ = @"mID=140935&orderInfo={TicketCode},1,leisureq";
                        string str_use_check_1_ = @":true,";

                        string str_use_url_2_ = @"https://www.esmplus.com/Escrow/Delivery/SetDoShippingGeneral";
                        string str_use_param_2_ = @"mID=140935&deliveryInfo={TicketCode},10032,자체배송, 15998370";
                        string str_use_check_2_ = @":true,";

                        Crawler_ = new LQCrawlereBay();
                        ((LQCrawlereBay)Crawler_).SetUseInfo(comparesite, str_use_url_1_, str_use_param_1_, str_use_check_1_
                            , str_use_url_2_, str_use_param_2_, str_use_check_2_);
                    }
                    break;

                case 13:    // CJ 오클락
                    {
                        
                        Crawler_ = new LQCrawlerCJOclock();
                    }
                    break;
                case 14: //굿바이셀리
                    {
                        string comparesite = "굿바이셀리";
                        string str_use_url_1_ = @"http://www.goodbuyselly.com/shop/set_trans_ready_proc";
                        string str_use_param_1_ = @"order_srl={CouponCode}";
                        string str_use_check_1_ = @"success";

                        string str_use_url_2_ = @"http://www.goodbuyselly.com/shop/set_trans_proc";
                        string str_use_param_2_ = @"order_srl={CouponCode}&pay_srl={TicketCode}&invoice_no=0000&total_trans=N&trans_method=E&trans_comp=&trans_method_etc=직접 전달";
                        string str_use_check_2_ = @"0#@#";

                        string str_use_url_3_ = @"http://www.goodbuyselly.com/shop/set_trans_complete_proc";
                        string str_use_param_3_ = @"order_srl={CouponCode}";
                        string str_use_check_3_ = @"success";

                        Crawler_ = new LQCrawlerGoodByeSelly();
                        ((LQCrawlerGoodByeSelly)Crawler_).SetUseInfo(comparesite, str_use_url_1_, str_use_param_1_, str_use_check_1_
                            , str_use_url_2_, str_use_param_2_, str_use_check_2_, str_use_url_3_, str_use_param_3_, str_use_check_3_);
                    }
                    break;
                case 15: //이제너두
                    {
                        string str_down_url_1_ = @"http://malladmin.etbs.co.kr/tbs/comm/log/FileAccessConfirmPrc.jsp";
                        string str_down_param_1_ = @"sch_date_type=ORDER_DATE&sch_rpt_status=&sch_sel_svcd_name=%BC%AD%BA%F1%BD%BA+%C0%FC%C3%BC&afterLogURL=%2Fwl%2Fservlets%2Ftbs.pmt.servlets.PayMainBackServlet%3Faction%3Dlist&type=I&curPage=1&sch_value=&sch_sel_vendor=leisureq&sch_sel_cmpy=&sch_sel_method_name=%B0%E1%C1%A6%B9%E6%B9%FD%C0%FC%C3%BC&sch_sel_method=&sch_to_order_date={sDate}&sch_fr_order_date={eDate}&xls=Y&sch_field=USER_NAME&sch_ord_status=&sch_pmt_status=&sch_sel_svcd=&sch_item=&ACCESS_TYPE=XLS&ACCESS_REASON=%BE%F7%B9%AB%BF%EB&ACCESS_ADMIN=E&DATA_MASK_YN=Y&ACCESS_AGREE=";
                        string str_down_check_1_ = @"<title>자료다운로드관리</title>";

                        Crawler_ = new LQCrawlerETBS();
                        ((LQCrawlerETBS)Crawler_).SetUseInfo(str_down_url_1_, str_down_param_1_, str_down_check_1_);
                    }
                    break;
                case 16: //원데이맘
                    {
                        Crawler_ = new LQCrawlerOnedayMom();
                      
                    }
                    break;
                case 17: //11번가
                    {
                        Crawler_ = new LQCrawlerEleven();
                        
                    }
                    break;
                case 18: //gs샵
                    {
                        Crawler_ = new LQCrawlerGSShop();
                        break;
                    }
                case 20: //아가월드
                    {   
                        Crawler_ = new LQCrawlerGSShop();
                        break;
                    }
                case 21: //LG
                    {    //이지웰같은 복지몰
                        Crawler_ = new LQCrawlerLG();
                        break;
                    }
                case 22: //롯데닷컴
                    {    //오픈마켓
                        Crawler_ = new LQCrawlerLotteDotCom();
                        break;
                    }
                case 23: //롯데몰
                    {    //오픈마켓
                        Crawler_ = new LQCrawlerLotte();
                        break;
                    }
                case 24: //체험팩토리(맘스쿨)
                    {    //복지몰
                        Crawler_ = new LQCrawlerMomSchool();
                        break;
                    }
                case 25: //체험팩토리(맘스쿨)
                    {    //복지몰
                        Crawler_ = new LQCrawlerCJOShopping();
                        break;
                    }
                case 26: //세일투나잇
                    {    
                        Crawler_ = new LQCrawlerSaleToNight();
                        break;
                    }
                case 27: //위크온
                    {
                        Crawler_ = new LQCrawlerWeekOn();
                        break;
                    }
                case 28: //캔고투
                    {
                        Crawler_ = new LQCrawlerCanGoTo();
                        break;
                    }
                case 29: //티켓수다
                    {
                        Crawler_ = new LQCrawlerTicketSuDa();
                        break;
                    }
                case 30: //맘스투데이
                    {
                        Crawler_ = new LQCrawlerMomsToDay();
                        break;
                    }
                default:
                    {
                        Crawler_ = new LQCrawlerBase();
                    }
                    break;
            }

        }

        public LQCrawlerBase GetCrawler()
        {
            return Crawler_;
        }

        public void SetChannelInfo(LQStructures.LQChannelInfo pInfo)
        {
            ChannelInfo_ = pInfo;
        }

        public LQCrawlerInfo GetChannelInfo()
        {
            return CrawlerInfo_;
        }

        public void SetCrawlerInfo(LQStructures.LQCrawlerInfo pInfo)
        {
            CrawlerInfo_ = pInfo;
        }

        public LQCrawlerInfo GetCrawlerInfo()
        {
            return CrawlerInfo_;
        }

        public void SetState(CrawlerState state)
        {
            State_ = state;
        }

        public CrawlerState GetState()
        {
            return State_;
        }

        public Dictionary<Int32, ChannelGoodInfo> GetGoodsInfo()
        {
            return GoodsInfoList_;
        }


        public ChannelGoodInfo GetGoodInfoByOptionName(string optionname)
        {
            foreach (var pData in GoodsInfoList_)
            {
                if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0)
                {
                    return pData.Value;
                }
            }

            return null;
        }

        // 상품Seq 를 옵션명으로 체크해서 얻어옴
        public bool GetGoodSeqByOptionName(string optionname, ref Int32 goodSeq)
        {
            bool bResult = false;

            foreach (var pData in GoodsInfoList_)
            {
                if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0)
                {
                    goodSeq = pData.Value.Idx_;
                    bResult = true;
                    break;
                }
            }

            return bResult;
        }

        public ChannelGoodInfo GetGoodSeqByOptionNameAndGoodName(string goodsname, string optionname)
        {
            foreach (var pData in GoodsInfoList_)
            {

                // 상품명과 옵션명 둘다로 체크 한다.
                if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0
                    && string.Compare(goodsname, pData.Value.GoodsName_, true) == 0)
                {
                    return pData.Value;
                }
            }

            return null;
        }

        public ResultData GetResultData()
        {
            return ResultData_;
        }
    }
}
