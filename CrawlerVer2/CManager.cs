using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Channels;
using System.Windows.Forms;
using HKLibrary;
using HKLibrary.UTIL;

namespace CrawlerVer2
{
    public enum CRAWLER_ACTION
    {
        NONE = 0,
        INSERT,     // 취합( 신규 데이터를 Insert )
        PROCESS,    // 처리( 데이터의 변경 사항을 Update )
        CHECK,      // 반환(체크한다)
    }

    public enum CHANNEL
    {
        COUPANG = 6,        // 쿠팡
        TIKET_MONSTER = 7,  // 티몬
        WEMAPE = 8,         // 위메프
        EZWELL = 9,         // 이지웰
        AUCTION = 11,       // 옥션
        G9 = 12,            // 지구
        CJOCLOCK = 13,      // CJ O클락
        GOODBYESELLY = 14,  // 굿바이셀리
        ETBS = 15,          // 이제너두
        ONEDAYMOM = 16,     // 원데이맘
        STREET_11 = 17,     // 11번가
        GSSHOP = 18,        // GSShop
        LG = 21,            // LG
        MOMSCHOOL = 24,     // 맘스쿨
        SALESTONIGHT = 26,   //세일즈투나잇
        TICKETSUDA = 29,   //티켓수다
        MOMSTODAY = 30,   //맘스투데이
        WEEKON = 27   //위크온


    }

    class CManager : BaseSingleton<CManager>
    {
        BaseChannel BaseChannel_ = null;
        BackgroundWorker Worker_ = null;
        public WebBrowser WB_ = null;

        public void StartCrawling(BackgroundWorker pWorker, CRAWLER_ACTION action, Int32 ChannelIndex)
        {
            bool bResult = true;

            Worker_ = pWorker;
            bResult = MakeCrawler((CHANNEL)ChannelIndex);
            
            if (bResult)
            {
                BaseChannel_.Init();
                switch (action)
                {
                    case CRAWLER_ACTION.INSERT:
                        {
                            Insert();
                        }
                        break;
                    case CRAWLER_ACTION.PROCESS:
                        {
                            ProcessX();
                        }
                        break;
                    case CRAWLER_ACTION.CHECK:
                        {
                            Check();
                        }
                        break;
                    default:
                        {
                            ProcessStateManager.Instance.State_ = CRAWLER_STATE.FINISH_WITH_ERROR;
                            ProcessStateManager.Instance.LastErrorMessage_ = string.Format("Error StartCrawling 액션값이 잘못됨 {0}", action);
                            NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
                            bResult = false;
                        }
                        break;
                }
            }
            else
            {
                ProcessStateManager.Instance.State_ = CRAWLER_STATE.FINISH_WITH_ERROR;
                ProcessStateManager.Instance.LastErrorMessage_ = string.Format("Error StartCrawling 크롤러 생성에 실패함 {0}", ChannelIndex);
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 사용한 엑셀 파일 삭제
            if (CINIManager.Instance.deletedownfile_ == true)
            {
                if (BaseChannel_ != null)
                    BaseChannel_.DeleteDownloadedFile();
            }
        }

        bool Insert()
        {
            bool bResult = true;
            ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_CRAWLER_INFO, Worker_);
            bResult = BaseChannel_.DB_GetInfos();   // DB에서 크롤러 정보 로드

            BaseChannel_.actionType = "insert";
            // 채널에 로그인
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_LOGIN, Worker_);
                bResult = BaseChannel_.Web_Login();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error BaseChannel_.DB_GetInfos 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 상품 판매 정보 다운로드 해서 엑셀에 저장
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_DOWN, Worker_);
                bResult = BaseChannel_.Web_DownLoadExcel();
                
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error BaseChannel_.Web_Login 로그인 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 엑셀 데이터 로드해서 리스트에 넣기
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_PARSING, Worker_);
                bResult = BaseChannel_.ExcelParsing();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error BaseChannel_.Web_DownLoadExcel 엑셀다운 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 새로 추가된 데이터 인지 체크
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_CHECK_NEW, Worker_);
                bResult = BaseChannel_.CheckNewOrder();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error BaseChannel_.ExcelParsing 엑셀파싱 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }
            // 두개를 비교하여 새로 생긴것은 Insert 하자.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_PROCESS, Worker_);
                bResult = BaseChannel_.DB_InsertData();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error BaseChannel_.CheckNewOrder 체크 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }
            return bResult;
        }

        bool ProcessX()
        {
            BaseChannel_.actionType = "ProcessX";
            bool bResult = true;
            ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_CRAWLER_INFO, Worker_);
            bResult = BaseChannel_.DB_GetInfos();   // DB에서 크롤러 정보 로드

            // 채널에 로그인
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_LOGIN, Worker_);
                bResult = BaseChannel_.Web_Login();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX BaseChannel_.DB_GetInfos 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 상품 판매 정보 다운로드 해서 엑셀에 저장
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_DOWN, Worker_);
                bResult = BaseChannel_.Web_DownLoadExcel();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX Web_Login 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 엑셀 데이터 로드해서 리스트에 넣기
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_PARSING, Worker_);
                bResult = BaseChannel_.ExcelParsing();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX Web_DownLoadExcel 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 사용처리를 해야 할것이 있는지 체크하자.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_CHECK_USE, Worker_);
                bResult = BaseChannel_.CheckNeedUseWeb();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX ExcelParsing 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 웹에서 사용처리하자.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_PROCESS, Worker_);
                bResult = BaseChannel_.Web_Use();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX CheckNeedUseWeb 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 오픈 마켓은 바로 UR,AR 은 그냥 A 혹은 U 로 변경한다.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_PROCESS, Worker_);
                bResult = BaseChannel_.OpenMarketChangeState();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX Web_Use 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 사용 처리를 완료하여 상태가 변경된것은 Update 한다.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_PROCESS, Worker_);
                bResult = BaseChannel_.DB_UpdateData();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error ProcessX OpenMarketChangeState 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            return bResult;
        }

        bool Check()
        {
            BaseChannel_.actionType = "Check";
            bool bResult = true;
            ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_CRAWLER_INFO, Worker_);
            bResult = BaseChannel_.DB_GetInfos();   // DB에서 크롤러 정보 로드

            // 채널에 로그인
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_LOGIN, Worker_);
                bResult = BaseChannel_.Web_Login();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check DB_GetInfos 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 상품 판매 정보 다운로드 해서 엑셀에 저장
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_DOWN, Worker_);
                bResult = BaseChannel_.Web_DownLoadExcel();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check Web_Login 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 엑셀 데이터 로드해서 리스트에 넣기
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_PARSING, Worker_);
                bResult = BaseChannel_.ExcelParsing();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check Web_DownLoadExcel 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }
            // DB 값 수정해야 하는것 체크
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_CHECK_FIXUP, Worker_);
                bResult = BaseChannel_.CheckNeedFixUp();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check ExcelParsing 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }
            // 취소 리스트 받기
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_DOWN_CANCEL, Worker_);
                bResult = BaseChannel_.Web_DownLoad_CancelList();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check CheckNeedFixUp 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 취소 엑셀 파싱
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_PARSING_CANCEL, Worker_);
                bResult = BaseChannel_.ExcelParsing_Cancel();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check Web_DownLoad_CancelList 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // 취소 정보 취함
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.EX_CHECK_CANCEL, Worker_);
                bResult = BaseChannel_.CheckIsCancel();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check ExcelParsing_Cancel 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            // DB 값이 잘못되어 있는 것을 원래대로 돌린다.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.DB_PROCESS, Worker_);
                bResult = BaseChannel_.DB_UpdateData_FixUp();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check CheckIsCancel 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }
            // 엑셀에서 취소 된것은 DB의 값을 바꿔주자.
            if (bResult == true)
            {
                ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.CH_PROCESS, Worker_);
                bResult = BaseChannel_.DB_UpdateData_Cancel();
            }
            else
            {
                ProcessStateManager.Instance.LastErrorMessage_ = "Error bool Check DB_UpdateData_FixUp 실패";
                NewLogManager2.Instance.Log(ProcessStateManager.Instance.LastErrorMessage_);
            }

            return bResult;
        }

        // 크롤러 생성
        bool MakeCrawler(CHANNEL channel)
        {
            bool bResult = true;
            if (BaseChannel_ != null)
                return true;

            switch (channel)
            {
                case CHANNEL.COUPANG: // 쿠팡
                    {
                        BaseChannel_ = new Coupang();
                    }
                    break;
                case CHANNEL.TIKET_MONSTER: // 티몬
                    {
                        BaseChannel_ = new TicketMonster();
                    }
                    break;
                case CHANNEL.WEMAPE: // 위메프
                    {
                        BaseChannel_ = new WeMakePrice();
                    }
                    break;
                case CHANNEL.EZWELL: // ezwell
                    {
                        BaseChannel_ = new EzWell();
                    }
                    break;
                case CHANNEL.AUCTION: // 옥션
                    {
                        BaseChannel_ = new eBay_Auction();
                    }
                    break;
                case CHANNEL.G9:    // 지마켓/지구
                   {
                        BaseChannel_ = new eBay_G9();
                    }
                    break;
                case CHANNEL.CJOCLOCK:    // CJ 오클락
                    {
                        BaseChannel_ = new CJOClock();
                    }
                    break;
                case CHANNEL.GOODBYESELLY: //굿바이셀리
                    {
                        BaseChannel_ = new GoodByeSelly();
                    }
                    break;
                case CHANNEL.ETBS: //이제너두
                    {
                        bResult = false;
                    }
                    break;
                case CHANNEL.ONEDAYMOM: //원데이맘
                    {
                        BaseChannel_ = new OneDayMom();
                    }
                    break;
                case CHANNEL.STREET_11: //11번가
                    {
                        BaseChannel_ = new Street11();
                    }
                    break;
                case CHANNEL.GSSHOP: //gs샵
                    {
                        //BaseChannel_ = new GS_Shop();
                        BaseChannel_ = new GSShop();
                    }
                    break;
                case CHANNEL.LG: //:LG
                    {
                        BaseChannel_ = new LG();
                    }
                    break;
                case CHANNEL.MOMSCHOOL: //:MOMSCHOOL
                    {
                        BaseChannel_ = new MomSchool();
                    }
                    break;
                case CHANNEL.SALESTONIGHT: //:세일즈투나잇
                    {
                        BaseChannel_ = new SaleToNight();
                    }
                    break;
                case CHANNEL.TICKETSUDA: //:티켓수다
                    {
                        BaseChannel_ = new TicketSuDa();
                    }
                    break;
                case CHANNEL.MOMSTODAY: //:맘스투데이
                    {
                        BaseChannel_ = new moms2Day();
                    }
                    break;
                case CHANNEL.WEEKON : //위크온
                    {
                        BaseChannel_ = new WeekOn();
                    }
                    break;
                default:
                    bResult = false;
                    break;
            }

            return bResult;
        }
    }
}
