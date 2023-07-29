using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <summary>
/// Communicates with a TcpClient encrypted messaging. <br/>
/// <b>NOTE:</b> Still retains all functionality of <see cref="TcpCommunicator"/>, but with the "Raw" keyword. <br/>
/// The normal methods are overriden to use encryption instead.
/// </summary>
public class EncryptedTcpCommunicator : TcpCommunicator {
    protected UtilHolder util;

    /// <summary>
    /// Reads the length of the encrypted data, then raw length bytes, then decrypts them.
    /// </summary>
    /// <returns>Read bytes</returns>
    public byte[] Read() {
        AssertNotClosed();
        var l = int.Parse(ReadStr());
        return util.Decrypt(ReadRawN(l));
    }

    /// <summary>
    /// Reads the desired length of bytes from the stream, then truncates it to the desired size.
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    [Obsolete("Use \"Read()\" instead to avoid truncation. Desired length is communicated automatically.")] 
    public new byte[] ReadN(int len) => Read()[..(len + 1)];

    /// <summary>
    /// Reads unencrypted bytes
    /// </summary>
    /// <param name="len">Quantity of bytes to read</param>
    /// <returns>Read bytes</returns>
    public byte[] ReadRawN(int len) {
        return base.ReadN(len);
    }
    

    /// <summary>
    /// Reads an unencrypted string
    /// </summary>
    /// <returns>Read string</returns>
    public string ReadRawStr() {
        return base.ReadStr();
    }

    /// <summary>
    /// Reads an encrypted string
    /// </summary>
    /// <returns>Read string</returns>
    public new string ReadStr() {
        AssertNotClosed();
        // Hopefully *2 is enough.
        return util.DecryptStr(ReadRawN(maxStrLen * 2));
    }

    /// <summary>
    /// Writes unencrypted bytes
    /// </summary>
    /// <param name="data">Bytes to write</param>
    public void WriteRaw(byte[] data) {
        base.Write(data);
    }

    /// <summary>
    /// Writes an unencrypted string
    /// </summary>
    /// <param name="data">String to write</param>
    public void WriteRawStr(string data) {
        base.WriteStr(data);
    }

    /// <summary>
    /// Writes encrypted bytes
    /// </summary>
    /// <param name="data">Bytes to write</param>
    public new void Write(byte[] data) {
        AssertNotClosed();
        var b = util.Encrypt(data);
        WriteStr(b.Length.ToString());
        WriteRaw(b);
    }

    /// <summary>
    /// Writes an encrypted string
    /// </summary>
    /// <param name="data">String to write</param>
    public new void WriteStr(string data) {
        AssertNotClosed();
        WriteRaw(util.EncryptStr(data));
    }

    public UtilHolder GetKeyUtil() {
        return util;
    }

    public EncryptedTcpCommunicator(TcpClient client, UtilHolder util, Encoding? encoding = null, int maxStrLen = 1024) : base(client, encoding, maxStrLen) {
        this.util = util;
    }
}