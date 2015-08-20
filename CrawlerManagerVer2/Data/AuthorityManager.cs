using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Form 에서 쓰이는 임시 클래스
public class TempChannel
{
    public Int32 seq_ = 0;              // 채널 시퀀스
    public string ChannelName_ = "";    // 채널 이름
}

public class TempAutoLogin
{
    public Int32 seq_ = 0;          // AuthLogin 시퀀스
    public string Name_ = "";       // AuthLogin 이름
}
#endregion

// 권리사 로그인 정보
public class AuthorityLoginInfoData
{
    public Int32 seq_ = 0;
    public Int32 ChannelSeq_ = 0;
    public Int32 PartnerSeq_ = 0;
    public string Name_ = "";
    public string ChannelName_ = "";
    public string AuthorityName_ = "";

    public Int32 ComboIndex_ = -1;
}

public class AuthorityLoginManager : BaseSingleton<AuthorityLoginManager>
{
    Dictionary<Int32, AuthorityLoginInfoData> List_ = new Dictionary<Int32, AuthorityLoginInfoData>();

    public Dictionary<Int32, AuthorityLoginInfoData> GetList()
    {
        return List_;
    }

    public void InitList()
    {
        List_.Clear();
    }

    public AuthorityLoginInfoData GetAuthrityLoginInfo(Int32 seq)
    {
        if (List_.ContainsKey(seq) == true)
        {
            return List_[seq];
        }

        return null;
    }

    // 권리사 시퀀스를 이용해서 채널을 구해온다. 동일 채널은 건너 뛴다.
    public Int32 GetChannelListByAuthoritySeq(Int32 AuthoritySeq, ref List<TempChannel> channel_list)
    {
        Int32 Result_Count = 0;
        Dictionary<Int32, Int32 > inpulist = new Dictionary<Int32 , Int32>();   // 오직 검사 용도로만 쓰임
        foreach (var pData in List_)
        {
            if (pData.Value.PartnerSeq_ == AuthoritySeq)
            {
                if (inpulist.ContainsKey(pData.Value.ChannelSeq_) == false)
                {
                    Result_Count++;
                    TempChannel pTempChannel = new TempChannel();
                    pTempChannel.seq_ = pData.Value.ChannelSeq_;
                    pTempChannel.ChannelName_ = pData.Value.ChannelName_;
                    channel_list.Add(pTempChannel);
                    inpulist.Add(pData.Value.ChannelSeq_, pData.Value.ChannelSeq_);
                }
            }
        }

        return Result_Count;
    }

    public Int32 GetAuthorityLoginListByAuthoSeqAndChannelSeq(Int32 authseq, Int32 ChannelSeq, ref List<TempAutoLogin> list)
    {
        Int32 Result_Count = 0;
        foreach (var pData in List_)
        {
            if (pData.Value.PartnerSeq_ == authseq && pData.Value.ChannelSeq_ == ChannelSeq)
            {
                TempAutoLogin pTempAutoLogin = new TempAutoLogin();
                pTempAutoLogin.seq_ = pData.Value.seq_;
                pTempAutoLogin.Name_ = pData.Value.Name_;
                list.Add(pTempAutoLogin);
                Result_Count++;
            }
        }

        return Result_Count;
    }
    // 콤보 박스 인덱스로 
    public AuthorityLoginInfoData GetAuthrityLoginByComboBoxIndex(Int32 ComboBoxIndex)
    {
        foreach (var pData in List_)
        {
            if (pData.Value.ComboIndex_ == ComboBoxIndex)
                return pData.Value;
        }

        return null;
    }
}


// 권리사 정보
public class AuthorityInfoData
{
    public Int32 seq_ = 0;
    public string partnerName_ = "";

    //public Int32 ChannelSeq_ = 0;
    //public string ChannelName_ = "";
    //public Int32 PartnerSeq_ = 0;
    //public string AuthorityName_ = "";
    //public string AuthorityName_Identity_ = "";
    //public string ID_ = "";         // 권리사 아이디

    public Int32 ComboIndex_ = -1;  // 콤보박스에 들어간 인덱스
}

public class AuthorityManager : BaseSingleton<AuthorityManager>
{
    Dictionary<Int32, AuthorityInfoData> List_ = new Dictionary<Int32, AuthorityInfoData>();

    public Dictionary<Int32, AuthorityInfoData> GetList()
    {
        return List_;
    }

    public void InitList()
    {
        List_.Clear();
    }

    public AuthorityInfoData GetAuthrity(Int32 authseq)
    {
        if (List_.ContainsKey(authseq) == true)
        {
            return List_[authseq];
        }

        return null;
    }

    public AuthorityInfoData GetAuthrityByComboBoxIndex(Int32 ComboBoxIndex)
    {
        foreach (var pData in List_)
        {
            if (pData.Value.ComboIndex_ == ComboBoxIndex)
                return pData.Value;
        }

        return null;
    }
}


// 채널 정보
public class ChannelInfoData
{
    public Int32 seq_ = 0;
    public string ChannelCode_ = "";
    public string ChannelName_ = "";

    public Int32 ComboIndex_ = -1;
}

public class ChannelManager : BaseSingleton<ChannelManager>
{
    Dictionary<Int32, ChannelInfoData> List_ = new Dictionary<Int32, ChannelInfoData>();

    public Dictionary<Int32, ChannelInfoData> GetList()
    {
        return List_;
    }

    public void InitList()
    {
        List_.Clear();
    }

    public ChannelInfoData GetChannel(Int32 channelseq)
    {
        if (List_.ContainsKey(channelseq) == true)
        {
            return List_[channelseq];
        }

        return null;

    }
    public ChannelInfoData GetChannelByComboBoxIndex(Int32 ComboBoxIndex)
    {
        foreach (var pData in List_)
        {
            if (pData.Value.ComboIndex_ == ComboBoxIndex)
                return pData.Value;
        }

        return null;
    }
}


// 채널 정보
public class ChannelInfoTempData
{
    public Int32 seq_ = 0;
    public string ChannelName_ = "";

    public Int32 ComboIndex_ = -1;
}

public class ChannelTempManager : BaseSingleton<ChannelTempManager>
{
    Dictionary<Int32, ChannelInfoTempData> List_ = new Dictionary<Int32, ChannelInfoTempData>();

    public Dictionary<Int32, ChannelInfoTempData> GetList()
    {
        return List_;
    }

    public void InitList()
    {
        List_.Clear();
    }

    public ChannelInfoTempData GetChannel(Int32 channelseq)
    {
        if (List_.ContainsKey(channelseq) == true)
        {
            return List_[channelseq];
        }

        return null;

    }

    public ChannelInfoTempData GetChannelByComboBoxIndex(Int32 ComboBoxIndex)
    {
        foreach (var pData in List_)
        {
            if (pData.Value.ComboIndex_ == ComboBoxIndex)
                return pData.Value;
        }

        return null;
    }
}

