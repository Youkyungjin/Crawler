using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CrawlerShare;

namespace CrawlerShare
{
    public enum DealStateEnum
    {
        NOT = 0,                // 아무것도 지정되지 않음.
        FINISH_BUY = 1,         // 채널에서 구매가 완료된 상태
        FINISH_RESERVED = 2,    // 레저큐를 통해서 예약 완료
        USED = 3,               // 채널에서 사용처리 완료 했음.
        NEED_CANCEL_USE = 4,    // 레저큐에서 취소했음, 채널에서 사용처리 취소 해야함
        USER_WANT_REFUND = 5,   // 유저가 채널에서 환불요청을 하였음.
        FINISH_REFUND = 6,      // 환불 완료
        CANCEL = 7,             // 주문이 취소 되었음.
        NEED_DEPOSIT = 8,       // 유저가 레저큐에서 주문하고 아직 입금전인 상황
        BLACK = 9,              // 채널에서 구매이후 레저큐에서 쿠폰번호를 받은후 채널에서는 취소 해두고 레저큐에서 쿠폰번호로 예약까지 한 상황.
        AR = 10,                // 파트너사에서 채널사용처리를 요청함
        A = 11,                 // 사용처리완료

        COUNT = 12,
    }

    // 주문별 상태 클래스
    public class DealState
    {
        public Int32 StateType_ = 0;
        public string StateName_ = "";
        public string Explain_ = "";
    }

    public class DealStateManager : BaseSingleton<DealStateManager>
    {
        public string[] StateString_ = new string[(Int32)DealStateEnum.COUNT];

        Dictionary<Int32, DealState> StateList_ = new Dictionary<Int32, DealState>();

        public void Init()
        {
            StateList_.Clear();
        }

        public void Add(Int32 type, string Name, string Explain)
        {
            if(StateList_.ContainsKey(type) == true)
            {
                string message = string.Format("상태정보 중복{0}_{1}_{2}", type, Name, Explain);
                LogManager.Instance.Log(message);
                return;
            }

            DealState pDealState = new DealState();
            pDealState.StateType_ = type;
            pDealState.StateName_ = Name;
            pDealState.Explain_ = Explain;

            StateList_.Add(type, pDealState);

            if (type > 0 && type < (Int32)DealStateEnum.COUNT)
            {
                StateString_[type] = Name;
            }
        }

        public Dictionary<Int32, DealState> GetStateList()
        {
            return StateList_;
        }
    }
}
