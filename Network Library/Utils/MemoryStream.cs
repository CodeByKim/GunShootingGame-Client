using System;
using System.Text;

public class MemoryStream
{
    private int offset;
    private byte[] buffer;

    public int Offset
    {
        get { return this.offset; }
    }

    public byte[] Buffer
    {
        get { return this.buffer; }
    }

    public MemoryStream(byte[] packetBuffer, int length)
    {
        this.buffer = new byte[length];
        Array.Copy(packetBuffer, 0, this.buffer, 0, length);
        this.offset = 0;
    }

    public void WriteInt(int data)
    {
        Array.Copy(BitConverter.GetBytes(data), 0, this.buffer, this.offset, 4);
        this.offset += 4;
    }

    public void WriteFloat(float data)
    {
        Array.Copy(BitConverter.GetBytes(data), 0, this.buffer, this.offset, 4);
        this.offset += 4;
    }

    public void WriteString(string data)
    {
        byte[] serializedData = Encoding.UTF8.GetBytes(data);
        WriteInt(serializedData.Length);
        Array.Copy(serializedData, 0, this.buffer, this.offset, serializedData.Length);
        this.offset += serializedData.Length;
    }

    public void WriteBool(bool data)
    {
        Array.Copy(BitConverter.GetBytes(data), 0, this.buffer, this.offset, 1);
        this.offset += 1;
    }

    public int ReadInt()
    {
        int data = BitConverter.ToInt32(this.buffer, this.offset);
        this.offset += 4;
        return data;
    }

    public float ReadFloat()
    {
        float data = BitConverter.ToSingle(this.buffer, this.offset);
        this.offset += 4;
        return data;
    }

    public bool ReadBool()
    {
        bool data = BitConverter.ToBoolean(this.buffer, this.offset);
        this.offset += 1;
        return data;
    }

    public string ReadString()
    {
        int stringLength = ReadInt();
        string data = Encoding.UTF8.GetString(this.buffer, this.offset, stringLength);
        this.offset += stringLength;        
        return data;
    }
}