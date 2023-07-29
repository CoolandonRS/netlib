using System.Net.Sockets;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <inheritdoc cref="SymmetricTcpCommunicator"/>
public class AESTcpCommunicator : SymmetricTcpCommunicator {
    public AESTcpCommunicator(TcpClient client, byte[] key, byte[] ivKey, Encoding? encoding = null, int maxStrLen = 1024) : base(client, new AESUtil(KeyType.Symmetric, key, ivKey, encoding), encoding, maxStrLen) {
    }

    public AESTcpCommunicator(TcpClient client, Encoding? encoding = null, int maxStrLen = 1024) : base(client, new AESUtil(), encoding, maxStrLen) {
    }
}