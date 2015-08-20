using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;

// 패킷을 컨버팅 해주는 함수.
// IOS 에서는 사용을 할수가 없다. 쓰지 않는다.
public class Packet_Convert
{
    // 바이너리 포맷터를 사용하면, 실제 클래스 사이즈보다 크게 Serialize 된다 이유는 패킹 정보가 함께 담기기 때문이다.
    private static BinaryFormatter formatter = new BinaryFormatter();

    public static void Serialize_Stream_Test<T>(ref T s, ref MemoryStream stream)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, s);
    }

    public static void Serialize_Stream<T>(ref T s, ref MemoryStream stream)
    {
        Packet_Convert.formatter.Serialize(stream, s);
    }

    public static Object Deserialize_Stream<T>(ref MemoryStream stream)
    {
        System.Object obj = null;

        try
        {
            obj = Packet_Convert.formatter.Deserialize(stream);
        }
        catch{}

        return obj;
    }

    public static byte[] Serialize<T>(T s)
    {
        Int32 iSizeOMyDataStruct = Marshal.SizeOf(typeof(T));     
        byte[] byteArrayMyDataStruct = new byte[iSizeOMyDataStruct];
        GCHandle gch = GCHandle.Alloc(byteArrayMyDataStruct, GCHandleType.Pinned);
        IntPtr pbyteArrayMyDataStruct = gch.AddrOfPinnedObject();
        Marshal.StructureToPtr(s, pbyteArrayMyDataStruct, false);
        gch.Free();
        return byteArrayMyDataStruct;
    }

    public static byte[] Serialize<T>(ref T s, ref byte[] pos)
    {
        Int32 iSizeOMyDataStruct = Marshal.SizeOf(typeof(T));
        byte[] byteArrayMyDataStruct = new byte[iSizeOMyDataStruct];
        GCHandle gch = GCHandle.Alloc(pos, GCHandleType.Pinned);
        IntPtr pbyteArrayMyDataStruct = gch.AddrOfPinnedObject();
        Marshal.StructureToPtr(s, pbyteArrayMyDataStruct, false);
        gch.Free();
        return byteArrayMyDataStruct;
    }

    public static void Deserialize<T>(ref T s, ref byte[] byteSerializedData)
    {
        GCHandle gch = GCHandle.Alloc(byteSerializedData, GCHandleType.Pinned);
        IntPtr pbyteSerializedData = gch.AddrOfPinnedObject();
        s = (T)Marshal.PtrToStructure(pbyteSerializedData, typeof(T));
        gch.Free();
    }

    public static T ReadStruct<T>(byte[] buffer) where T : class
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
            throw new Exception();
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);        
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    } 

}

