using System.Net.Sockets;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <inheritdoc cref="AsymmetricTcpCommunicator"/>
public class RSATcpCommunicator : AsymmetricTcpCommunicator {
    public RSATcpCommunicator(TcpClient client, string prvPem, string pubPem, Encoding? encoding = null, int maxStrLen = 50) : base(client, new RSAUtil(KeyType.Private, prvPem, encoding), new RSAUtil(KeyType.Public, pubPem, encoding), encoding, maxStrLen) {
    }
}