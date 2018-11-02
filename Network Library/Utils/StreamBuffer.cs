using System;
using System.Collections;
using System.Collections.Generic;

public class StreamBuffer
{
    private int remainLength;
    private byte[] buffer;

    public int RemainLength
    {
        get { return this.remainLength; }
        set { this.remainLength = value; }
    }

    public byte[] Buffer
    {
        get { return this.buffer; }
    }

    public StreamBuffer()
    {
        this.remainLength = 0;
        this.buffer = new byte[Defines.MAX_BUFFER_LENGTH];
    }

    public void PutStream(byte[] socketBuffer, int length)
    {
        Array.Copy(socketBuffer, 0, this.buffer, this.remainLength, length);
        this.remainLength += length;
    }
}
