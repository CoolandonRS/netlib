using System.Net.Sockets;
using System.Text;
using keyring;

namespace CoolandonRS.netlib; 

/// <summary>
/// Communicates with a TcpClient using RSA and keyring. <br/>
/// <b>NOTE:</b> Still retains all functionality of <see cref="TcpCommunicator"/>, but with the "Raw" keyword. <br/>
/// The normal methods are overriden to use RSA/keyring instead.
/// </summary>
public class TcpRsaCommunicator : TcpCommunicator {
    protected readonly (RSAUtil recieve, RSAUtil send) rsaKeys;

    /// <summary>
    /// Reads len bytes raw, then decrypts them
    /// </summary>
    /// <param name="len">Quantity of bytes to read</param>
    /// <returns>Read bytes</returns>
    public new byte[] ReadN(int len) {
        AssertNotClosed();
        return rsaKeys.recieve.Decrypt(ReadRawN(len));
    }

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
        return rsaKeys.recieve.DecryptStr(ReadRawN(maxStrLen * 2));
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
        WriteRaw(rsaKeys.send.Encrypt(data));
    }

    /// <summary>
    /// Writes an encrypted string
    /// </summary>
    /// <param name="data">String to write</param>
    public new void WriteStr(string data) {
        AssertNotClosed();
        WriteRaw(rsaKeys.send.EncryptStr(data));
    }

    public (RSAUtil recieve, RSAUtil send) GetRSAkeys() {
        return rsaKeys;
    }

    public TcpRsaCommunicator(TcpClient client, string inPemData, string outPemData, Encoding? encoding = null, int maxStrLen = 1024) : base(client, encoding, maxStrLen) {
        this.rsaKeys = (new RSAUtil(KeyType.Private, inPemData, encoding), new RSAUtil(KeyType.Public, outPemData, encoding));
    }
}