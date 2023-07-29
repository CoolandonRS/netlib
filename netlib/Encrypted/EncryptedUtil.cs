using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

public static class EncryptedUtil {
    private const string authSuccess = "AUTH_SUCCESS";
    
    /// <returns>true on ACK, false otherwise.</returns>
    public static bool AuthHandshakeClient(AsymmetricTcpCommunicator communicator, string identifier) {
        communicator.WriteStr(identifier);
        communicator.Write(communicator.Read());
        return communicator.ReadStr() == authSuccess;
    }


    /// <param name="client">The TcpClient</param>
    /// <param name="server">The servers private key</param>
    /// <param name="clientLocator">A function that can locate a clients public key given an identifier</param>
    /// <returns>Null on failure, the communicator otherwise</returns>
    public static AsymmetricTcpCommunicator? AuthHandshakeServer(TcpClient client, EncryptionUtil server, Func<string, EncryptionUtil?> clientLocator, Encoding? encoding = null, int maxStrLen = 1024) {
        try {
            var naive = new SymmetricTcpCommunicator(client, server, encoding, maxStrLen);
            var known = clientLocator(naive.ReadStr());
            if (known == null) return null;
            var asym = new AsymmetricTcpCommunicator(client, server, known, encoding, maxStrLen);
            var test = RandomNumberGenerator.GetBytes(32);
            asym.Write(test);
            if (asym.Read() != test) return null;
            asym.WriteStr(authSuccess);
            return asym;
        } catch {
            return null;
        }
    }

    public static AESTcpCommunicator NegotiateToAESServer(this AsymmetricTcpCommunicator asym, AESUtil? util = null, int maxStrLen = 1024) {
        util ??= new AESUtil(asym.GetEncoding());
        var secrets = util.GetSecrets();
        asym.Write(secrets.key);
        asym.Write(secrets.ivKey);
        return new AESTcpCommunicator(asym.client, secrets.key, secrets.ivKey, asym.GetEncoding(), maxStrLen);
    }

    public static AESTcpCommunicator NegotiateToAESClient(this AsymmetricTcpCommunicator asym, int maxStrLen = 1024) {
        var key = asym.Read();
        var ivKey = asym.Read();
        return new AESTcpCommunicator(asym.client, key, ivKey, asym.GetEncoding(), maxStrLen);
    }

    public static AESTcpCommunicator? AuthToAESServer(TcpClient client, EncryptionUtil server, Func<string, EncryptionUtil?> clientLocator, Encoding? encoding = null, int maxStrLen = 1024) {
        return AuthHandshakeServer(client, server, clientLocator, encoding, maxStrLen)?.NegotiateToAESServer(null, maxStrLen);
    }

    public static AESTcpCommunicator? AuthToAESClient(AsymmetricTcpCommunicator communicator, string identifier, int maxStrLen = 1024) {
        return !AuthHandshakeClient(communicator, identifier) ? null : communicator.NegotiateToAESClient(maxStrLen);
    }
}