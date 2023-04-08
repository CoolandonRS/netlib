using System.Net.Sockets;
using System.Text;
using keyring;

namespace netlib; 

/// <summary>
/// Communicates with a TcpClient using RSA and keyring.
/// </summary>
public class RsaCommunicator : TcpCommunicator {
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    private readonly Encoding encoding;
    private readonly (RSAUtil recieve, RSAUtil send) rsaKeys;
    private readonly int maxStrLen;
    private bool closed;

    /// <summary>
    /// Reads unencrypted bytes
    /// </summary>
    /// <param name="len">Quantity of bytes to read</param>
    /// <returns>Read bytes</returns>
    public byte[] ReadRawN(int len) {
        AssertNotClosed();
        var data = new byte[len];
        // ReSharper disable once MustUseReturnValue
        stream.Read(data);
        return data;
    }

    /// <summary>
    /// Reads an unencrypted string
    /// </summary>
    /// <returns>Read string</returns>
    public string ReadRawStr() {
        AssertNotClosed();
        return encoding.GetString(ReadRawN(maxStrLen)).Replace("\0", "");
    }

    /// <summary>
    /// Reads an encrypted string
    /// </summary>
    /// <returns>Read string</returns>
    public string ReadStr() {
        AssertNotClosed();
        // Hopefully *2 is enough.
        return rsaKeys.recieve.DecryptStr(ReadRawN(maxStrLen * 2));
    }

    /// <summary>
    /// Writes unencrypted bytes
    /// </summary>
    /// <param name="data">Bytes to write</param>
    public void WriteRaw(byte[] data) {
        AssertNotClosed();
        stream.Write(data);
    }

    /// <summary>
    /// Writes an unencrypted string
    /// </summary>
    /// <param name="data">String to write</param>
    public void WriteRawStr(string data) {
        AssertNotClosed();
        WriteRaw(encoding.GetBytes(data));
    }

    /// <summary>
    /// Writes encrypted bytes
    /// </summary>
    /// <param name="data">Bytes to write</param>
    public void Write(byte[] data) {
        AssertNotClosed();
        WriteRaw(rsaKeys.send.Encrypt(data));
    }

    /// <summary>
    /// Writes an encrypted string
    /// </summary>
    /// <param name="data">String to write</param>
    public void WriteStr(string data) {
        AssertNotClosed();
        WriteRaw(rsaKeys.send.EncryptStr(data));
    }
    
    private void AssertNotClosed() {
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

    public (RSAUtil recieve, RSAUtil send) GetRSAkeys() {
        return rsaKeys;
    }

    public RsaCommunicator(TcpClient client, string inPemData, string outPemData, Encoding? encoding = null, int maxStrLen = 1024) {
        this.closed = true;
        this.client = client;
        this.stream = client.GetStream();
        this.encoding = encoding ?? Encoding.UTF8;
        this.rsaKeys = (new RSAUtil(KeyType.Private, inPemData, encoding), new RSAUtil(KeyType.Public, outPemData, encoding));
        this.maxStrLen = maxStrLen;
    }
}