using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace LQStructures
{
    public class PacketProcess
    {
        public static void Serialize(PacketBase packet, byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            System.Text.Encoding encode = System.Text.Encoding.Unicode;
            BinaryWriter writer = new BinaryWriter(ms, encode);
            packet.Serialize(writer);

            // copy to byte[]
            writer.BaseStream.Position = 0;
            ms.Read(buffer, 0, (int)packet.len);
        }

        // 버퍼를 패킷으로 Deserialize 해줌.
        public static void Deserialize(PacketBase packet, byte[] buffer)
        {
            MemoryStream rl_ms = new MemoryStream();
            rl_ms.Write(buffer, 0, Marshal.SizeOf(packet));
            rl_ms.Position = 0;
            BinaryReader rl_binary = new BinaryReader(rl_ms, System.Text.Encoding.Unicode);
            packet.Deserialize(rl_binary);
        }
    }

    // 크롤러 -> 매니저
    public enum PACKET_IDX : byte
    {
        CM_CHANNEL_IDX = 1, // 채널 번호 보냄
        KM_CHANNEL_IDX,     // 관리하고 있는 채널 번호 보냄
        MK_RESTART,         // 크롤러 재시작
        CK_HEARTBEAT,       // 크롤러가 정상 동작 하고 있음을 체커에게 보냄.


        // 신규 추가 패킷
        CK_RUNNING_INFO,    // 크롤러->체커로 보내는 실행 상태 정보
        PACKET_END,         // 패킷의 끝
    }

    // 패킷 베이스
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class PacketBase
    {
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write((Int16)len);
            writer.Write((Byte)num);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            len = reader.ReadInt16();
            num = reader.ReadByte();
        }

        // 패킷의 길이
        [MarshalAs(UnmanagedType.I2)]
        public Int16 len;
        
        // 패킷 종류
        [MarshalAs(UnmanagedType.I1)]
        public Byte num;
    }

    #region #크롤러 -> 매니저 패킷
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class C_TO_M_CHANNEL_IDX : PacketBase
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(nIdx);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            nIdx = reader.ReadInt32();
        }

        [MarshalAs(UnmanagedType.I4)]
        public Int32 nIdx;
    }
    #endregion

    #region #크롤러 -> 체커 패킷
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class C_TO_K_HEARTBEAT : PacketBase
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CrawlingCount);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            CrawlingCount = reader.ReadInt32();
        }

        [MarshalAs(UnmanagedType.I4)]
        public Int32 CrawlingCount;
    }


    #endregion

    #region #체커 -> 매니저 패킷
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class K_TO_M_CHANNEL_IDX : PacketBase
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(nIdx);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            nIdx = reader.ReadInt32();
        }

        [MarshalAs(UnmanagedType.I4)]
        public Int32 nIdx;
    }
    #endregion

    #region #매니저 -> 채커 패킷
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class M_TO_K_RESTART : PacketBase
    {
    }
    #endregion

    
}
