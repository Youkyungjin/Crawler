using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CGoodsData
{
    public Int32 Seq_ = -1;             // 상품 시퀀스
    public string Goods_Code_ = "";     // 상품 코드
    public string GoodsName_ = "";      // 상품 이름
    public string GoodsNickName_ = "";  // 상품 닉네임    
    public string OptionName_ = "";     // 옵션명
    public string OptionNickName_ = ""; // 옵션 닉네임    
    public Int32 ChannelSeq_ = 0;       // 채널 시퀀스
    public Int32 AuthoritySeq_ = 0;     // 권리사 시퀀스
    public Int32 AuthorityLoginSeq_ = 0;     // 권리사 로그인 시퀀스
    public Int32 CrawlerSeq_ = 0;       // 크롤러 시퀀스
    public string State_ = "";          // 상태


    public string ChannelName_ = "";    // 채널 시퀀스로 검색한 채널 이름
    public string AuthrityName_ = "";   // 권리사 시퀀스로 검색한 권리사 이름
}

// 크롤러 정보 매니저
public class GoodsManager : BaseSingleton<GoodsManager>
{
    Dictionary<Int32, CGoodsData> List_ = new Dictionary<Int32, CGoodsData>();


    public void InitList()
    {
        List_.Clear();
    }

    public CGoodsData GetCGoodsData(Int32 goodseq)
    {
        if (List_.ContainsKey(goodseq) == true)
            return List_[goodseq];

        return null;
    }

    public Dictionary<Int32, CGoodsData> GetList()
    {
        return List_;
    }

    public Int32 GetListByCrawerSeq(Int32 CrawlerSeq, ref Dictionary<Int32, CGoodsData> OutList)
    {
        Int32 nCount = 0;

        foreach (var pData in List_)
        {
            if (pData.Value.CrawlerSeq_ == CrawlerSeq)
            {
                OutList.Add(pData.Key, pData.Value);
                nCount++;
            }
        }

        return nCount;
    }
}

