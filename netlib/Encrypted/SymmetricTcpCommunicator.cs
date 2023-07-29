using System.Net.Sockets;
using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <summary>
/// (Mostly) Cosmetic child of EncryptedTcpCommunicator.
/// </summary>
/// <inheritdoc cref="EncryptedTcpCommunicator"/>
public class SymmetricTcpCommunicator : EncryptedTcpCommunicator {
    public SymmetricTcpCommunicator(TcpClient client, EncryptionUtil key, Encoding? encoding = null, int maxStrLen = 1024) : base(client, new UtilHolder(key), encoding, maxStrLen) {
    }
    
    public EncryptionUtil GetEncoder() => util.GetEncoder();
}