using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LQStructures
{
    public class LQCrawlerInfo
    {
        public LQCrawlerInfo()
        {
            nIdx_ = -1;
        }
        public Int32 nIdx_;             // 인덱스( -1 이면 아직 로드 되지 않았다 )
        public Int32 Channel_Idx_;      // 채널 인덱스
        public string ChannelName_;     // 채널 이름
        public Int32  PartnerSeq_;      // 파트너 인덱스
        public string PartnerName_;     // 파트너 이름
        public string MainUrl_;         // 메인 URL
        public string LoginIDTAG_;
        public string LoginPWTAG_;
        public string LoginUrl_;
        public string LoginParam_;      // 로그인 셋팅값
        public string LoginID_;         // 로그인 아이디
        public string LoginPW_;         // 로그인 암호
        public string LoginMethod_;     // 로그인 방식
        public string LoginEvent_;      // 로그인 버튼 이벤트
        public string LoginCheck_;
        public char LoginType_;
        public string ExcelDownUrl_;    // 엑셀 다운로드 URL
        public string ExcelDownParameter_;
        public string ExcelDownRule_;
        public string ExcelDownMethod_; // 엑셀 다운로드 방식

        public Int32 AuthoritySeq_;     // 권리사 인덱스
        public string AuthoriryName_;   // 권리사 이름
        
        public string UseGoodsUrl_;     // 사용고객 검색 URL
        public string UseGoodsParam_;   // 사용고객 검색파라미터
        public string UseGoodsUseTag_;  // 사용처리 버튼 태그
        public string UseGoodsCheck_;    // 사용처리후 성공 결과
        public string UseGoodsRule_;

        public string UseUserUrl_;      // 사용처리 URL
        public string UseUserParam_;    // 사용처리 파라미터
        public string UseUserCheck_;    // 사용처리 파라미터

        public string NUseGoodsUrl_;     // 미사용고객 검색 URL
        public string NUseGoodsParam_;   // 미사용고객 검색파라미터
        public string NUseGoodsUseTag_;  // 미사용처리 버튼 태그
        public string NUseGoodsCheck_;    // 취소 처리후 성공 결과
        public string NUseGoodsRule_;

        public string NUseUserUrl_;     // 미사용 처리 URL
        public string NUseUserParam_;   // 미사용 처리 파라미터
        public string NUseUserCheck_;   // 미사용 처리 성공 결과

        public string RUseUserUrl_;     // 환불 처리 URL
        public string RUseUserParam_;   // 환불 처리 파라미터
        public string RUseUserCheck_;    // 환불처리 성공 결과

        public Int32 ExData_Start_;
        public Int32 ExData_Coupncode_;    // 쿠폰 번호 컬럼 인덱스
        public Int32 ExData_Buydate_;    // 구매일 컬럼 인덱스
        public Int32 ExData_Option_;       // 옵션 컬럼 인덱스
        public Int32 ExData_Cancel_;       // 취소 여부 컬럼 인덱스
        public Int32 ExData_Use_;           // 사용 처리 여부 컬럼 인덱스
        public Int32 ExData_Buyer_;       // 구매자 이름 컬럼 인덱스
        public Int32 ExData_Buyphone_;       // 구매자 핸드폰 컬럼 인덱스
        public Int32 ExData_Price_;          // 판매가 컬럼 인덱스
        public Int32 ExData_Count_;          // 구매 갯수 
        public Int32 ExData_GoodName_;     // 엑셀의 상품명 컬럼
        public string ExData_UseCheck_;     // 사용 상태
        public string ExData_CancelCheck_;  // 취소 상태

        public string state_;           // 상태
        public string regdate_;         // 등록일
    }

    // 크롤링 결과 객체
    public class ResultData
    {
        public ResultData()
        {
            Reset();
        }

        public void Reset()
        {
            DBSelected_ = 0;
            Inserted_ = 0;
            Updated_ = 0;
            ErrorCount_ = 0;
            ProcessTime_DB_ = 0;
            ProcessTime_Web_ = 0;
            ProcessTime_Total_ = 0;
        }


        public Int32 DBSelected_ = 0;   // 현재 DB에 저장되어 있는 Row
        public Int32 Inserted_ = 0;     // 신규로 Insert 되야 하는 Row
        public Int32 Updated_ = 0;      // Update 되야 하는 Row
        public Int32 ErrorCount_ = 0;   // 에러난 횟수
        public Int32 ProcessTime_DB_ = 0;   // DB 처리 시간
        public Int32 ProcessTime_Web_ = 0;  // 웹 처리 시간
        public Int32 ProcessTime_Total_ = 0;

        // 모든 결과의 합.
        public Int32 TotalUseDeal_ = 0;
        public Int32 TotalCancelDeal_ = 0;
        public Int32 TotalRefundDeal_ = 0;
        public Int32 TotalErrorCount_ = 0;
    }
}
