using System.Net.Sockets;
using System.Text;

namespace CoolandonRS.netlib; 

/// <summary>
/// Communicates with a TcpClient
/// </summary>
public class TcpCommunicator {
    protected readonly TcpClient client;
    protected readonly NetworkStream stream;
    protected readonly Encoding encoding;
    protected readonly int maxStrLen;
    protected bool closed;

    /// <summary>
    /// Reads bytes
    /// </summary>
    /// <param name="len">Quantity of bytes to read</param>
    /// <returns>Read bytes</returns>
    public byte[] ReadN(int len) {
        AssertNotClosed();
        var data = new byte[len];
        // ReSharper disable once MustUseReturnValue
        stream.Read(data);
        return data;
    }

    /// <summary>
    /// Reads a string
    /// </summary>
    /// <returns>Read string</returns>
    public string ReadStr() {
        AssertNotClosed();
        return encoding.GetString(ReadN(maxStrLen)).Replace("\0", "");
    }

    /// <summary>
    /// Writes bytes
    /// </summary>
    /// <param name="data">Bytes to write</param>
    public void Write(byte[] data) {
        AssertNotClosed();
        stream.Write(data);
    }

    /// <summary>
    /// Writes a string
    /// </summary>
    /// <param name="data">String to write</param>
    public void WriteStr(string data) {
        AssertNotClosed();
        Write(encoding.GetBytes(data));
    }

    protected void AssertNotClosed() {
        if (closed) throw new InvalidOperationException("This TcpCommunicator has been closed");
    }

    /// <summary>
    /// Not required, but useful for code segmentation
    /// </summary>
    public void Close() {
        try {
            client.Close();
        } catch {
            // no-op
        }
        this.closed = true;
    }

    public bool IsClosed() {
        return closed;
    }

    public TcpCommunicator(TcpClient client, Encoding? encoding = null, int maxStrLen = 1024) {
        this.client = client;
        this.stream = client.GetStream();
        this.encoding = encoding ?? Encoding.UTF8;
        this.maxStrLen = maxStrLen;
    }
}