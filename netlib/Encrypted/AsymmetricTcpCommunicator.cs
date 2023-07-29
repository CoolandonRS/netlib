using System.Net.Sockets;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <summary>
/// Cosmetic child of EncryptedTcpCommunicator.
/// </summary>
/// <inheritdoc cref="EncryptedTcpCommunicator"/>
public class AsymmetricTcpCommunicator : EncryptedTcpCommunicator {
    public AsymmetricTcpCommunicator(TcpClient client, EncryptionUtil prv, EncryptionUtil pub, Encoding? encoding = null, int maxStrLen = 1024) : base(client, new UtilHolder(prv, pub), encoding, maxStrLen) {
    }
}