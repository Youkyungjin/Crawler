using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LQStructures
{
    // 채널 상태
    public enum ChannelState
    {
        CS_NOT_CONTRACT = 0,    // 비계약
        CS_CONTRACT,            // 계약
    }

    // 채널 정보
    [Serializable]
    public class LQChannelInfo
    {
        public LQChannelInfo()
        {
            nIdx_ = -1;
            Channel_Idx_ = -1;
            Channel_Name_ = "";
            strUpdateDate_ = "";
            strRegDate_ = "";


            connected_ip_ = "";
            crawler_status_ = "접속 끊김";
            checker_status_ = "접속 끊김";
        }

        public Int32 nIdx_;             // 인덱스
        public Int32 PartnerSeq_;       // 파트너 시퀀스
        public string PartnerName_;     // 파트너 이름
        public Int32 Channel_Idx_;      // 채널 인덱스
        public string Channel_Name_;    // 채널 이름
        public ChannelState State_;     // 채널 상태
        public string strUpdateDate_;   // 수정 날짜
        public string strRegDate_;      // 등록 날짜

        // 기타 정보
        public string connected_ip_;    // 연결된 ip
        public string crawler_status_;  // 동작 상태
        public string checker_status_;  // 체커 연결 상태
    }
}
