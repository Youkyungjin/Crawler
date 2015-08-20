using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HKLibrary.UTIL;
using System.ComponentModel;

namespace Channels
{
    public enum CRAWLER_STATE 
    {
        NONE,
        WAIT,               // 다음 크롤링을 기다리는 상태
        DB_CRAWLER_INFO,    // DB 에서 크롤링 정보 읽어 오는 중
        DB_GOODS_INFO,      // DB 에서 상품 정보 읽어 오는 중
        DB_SELECT_DEALS,    // DB 에서 Insert 된 상품 정보 읽어오는중
        CH_LOGIN,           // 채널에 로긴중
        CH_DOWN,            // 다운로드 중
        EX_PARSING,         // 엑셀 파싱중
        EX_CHECK_NEW,        // 신규 인지 체크
        EX_CHECK_USE,       // 사용처리 해야하는지 체크
        EX_CHECK_FIXUP,     // DB에는 완료로 나오는데, 채널에는 사용처리 되지 않은게 있는가?
        EX_COMBINE,         // 병합중
        CH_PROCESS,         // 웹에서 처리중
        CH_DOWN_CANCEL,     // 취소 엑셀 다운로드
        EX_PARSING_CANCEL,  // 취소 엑셀 파싱
        EX_CHECK_CANCEL,    // 취소 정보 체크중
        DB_PROCESS,         // 변경,추가 사항 DB에 저장중.

        FINISH_SUCCEED,     // 성공하고 끝났음.
        FINISH_WITH_ERROR,  // 에러났음.
    }

    public class ProcessStateManager : BaseSingleton<ProcessStateManager>
    {
        public string[] ActionMode_ = new string[] { "NONE", "취합", "처리", "반환" };

        public CRAWLER_STATE State_ = CRAWLER_STATE.NONE;
        public string LastErrorMessage_ = "";   // 마지막 에러 메시지
        public string ChannelName_ = "-";   // 채널명
        Int32 Start_Tick_ = 0;              // 크롤링을 시작한 Tick
        Int32 Step_Start_Tick_ = 0;         // 새로운 스텝으로 들어온 Tick
        public Int32 CrawlingCount_ = 0;    // 크롤링 횟수


        public Int32 NeedDownLoadCount_ = 0;       // 다운로드 받아야 하는 갯수
        public Int32 CurDownLoadCount_ = 0;        // 다운로드 받은 갯수
        public Int32 PassDownLoadCount_ = 0;        // 파일이 중복되어 받지 않고 넘어간 개수
        public Int32 NeedParsingCount_ = 0;        // 필요 파싱 갯수
        public Int32 CurParsingCount_ = 0;         // 파싱한 갯수 
        public Int32 PassParsingCount_ = 0;         // 패스된 파싱갯수
        public Int32 NeedWebProcessCount_ = 0;      // 웹에 처리해야할 갯수
        public Int32 CurWebProcessCount_ = 0;       // 웹에 처리한 갯수.
        public Int32 FailedWebProcessCount_ = 0;    // 웹에서 처리중 실패한 개수
        public Int32 NeedDBProcessCount_ = 0;      // DB 에 처리해야할 갯수
        public Int32 CurDBProcessCount_ = 0;       // DB에 처리한 갯수.
        public Int32 NextCrawlingTikc_ = 0;         // 다음 크롤링 시간

        public void Init()
        {
            Start_Tick_ = Environment.TickCount;
            Step_Start_Tick_ = Environment.TickCount;
            State_ = CRAWLER_STATE.NONE;

            NeedDownLoadCount_ = 0;
            CurDownLoadCount_ = 0;
            PassDownLoadCount_ = 0;
            NeedParsingCount_ = 0;
            CurParsingCount_ = 0;
            NeedWebProcessCount_ = 0;
            CurWebProcessCount_ = 0;
            FailedWebProcessCount_ = 0;
            NeedDBProcessCount_ = 0;
            CurDBProcessCount_ = 0;
            PassParsingCount_ = 0;         // 패스된 파싱갯수
        }

        public void ChangeStateAndReport(CRAWLER_STATE CurState, BackgroundWorker pWoker)
        {
            State_ = CurState;
            if (pWoker != null)
                pWoker.ReportProgress((Int32)State_);
        }

        public string GetCurStateString()
        {
            string statestring = "";

            switch(State_)
            {
                case CRAWLER_STATE.NONE:
                    {
                        statestring = "-";                        
                    }
                    break;
                case CRAWLER_STATE.WAIT:
                    {
                        Int32 LeftTime = NextCrawlingTikc_ - Environment.TickCount;
                        statestring = string.Format("다음 크롤링까지 남은 시간 : {0:F1}초", LeftTime * 0.001f);
                    }
                    break;
                case CRAWLER_STATE.DB_CRAWLER_INFO:
                    {
                        statestring = "DB에서 크롤링 정보 로드중";
                    }
                    break;
                case CRAWLER_STATE.DB_GOODS_INFO:
                    {
                        statestring = "DB에서 상품 정보 로드중";
                    }
                    break;
                case CRAWLER_STATE.DB_SELECT_DEALS:
                    {
                        statestring = "DB에서 유저들 구매 정보 로드중";
                    }
                    break;
                case CRAWLER_STATE.CH_LOGIN:
                    {
                        statestring = "채널에 로그인중";
                    }
                    break;
                case CRAWLER_STATE.CH_DOWN:
                    {
                        statestring = string.Format("채널에서 엑셀 파일 다운로드중 {0}/{1}", CurDownLoadCount_, NeedDownLoadCount_);
                    }
                    break;
                case CRAWLER_STATE.EX_PARSING:
                    {
                        statestring = "엑셀 파싱중";
                    }
                    break;
                case CRAWLER_STATE.EX_CHECK_NEW:
                    {
                        statestring = "엑셀 데이터 체크중";
                    }
                    break;
                case CRAWLER_STATE.EX_CHECK_USE:
                    {
                        statestring = "사용처리 해야할것 체크";
                    }
                    break;
                case CRAWLER_STATE.EX_CHECK_FIXUP:
                    {
                        statestring = "DB 상태 반환";
                    }
                    break;                    
                case CRAWLER_STATE.EX_COMBINE:
                    {
                        statestring = "데이터 비교및 처리중";
                    }
                    break;
                case CRAWLER_STATE.CH_PROCESS:
                    {
                        statestring = string.Format("채널에서 처리중 {0}/{1}", CurDBProcessCount_, NeedDBProcessCount_);
                    }
                    break;
                case CRAWLER_STATE.CH_DOWN_CANCEL:
                    {
                        statestring = string.Format("채널에서 취소 정보 다운로드중");
                    }
                    break;
                case CRAWLER_STATE.EX_PARSING_CANCEL:
                    {
                        statestring = string.Format("취소 정보 파싱중");
                    }
                    break;
                case CRAWLER_STATE.EX_CHECK_CANCEL:
                    {
                        statestring = string.Format("취소 정보 체크중");
                    }
                    break;
                case CRAWLER_STATE.DB_PROCESS:
                    {
                        statestring = string.Format("DB에 처리중 {0}/{1}", CurDBProcessCount_, NeedDBProcessCount_);
                    }
                    break;
                default:
                    {
                        statestring = "상태값 이상";
                    }
                    break;
            }

            return statestring;
        }

        public string GetBeforeStateString()
        {
            string beforestring = "";

            if (State_ == CRAWLER_STATE.FINISH_WITH_ERROR)
            {
                beforestring = string.Format("실패 : {0}", LastErrorMessage_);
            }
            else
            {
                float ElapsedTick = (Environment.TickCount - Start_Tick_) * 0.001f;

                beforestring = string.Format("성공 {0}초 - 다운로드 {1}/{2}/{3}, 파싱 {4}/{10}/{5}, 웹처리 {6}/{7}, DB {8}/{9}"
                    , (Int32)ElapsedTick, CurDownLoadCount_, PassDownLoadCount_, NeedDownLoadCount_
                    , CurParsingCount_, NeedParsingCount_, CurWebProcessCount_, NeedWebProcessCount_
                    , CurDBProcessCount_, NeedDBProcessCount_, PassParsingCount_);
            }

            return beforestring;
        }
    }
}
