using System.Text;
using CoolandonRS.keyring;

namespace CoolandonRS.netlib.Encrypted; 

/// <summary>
/// Used to transform a pair of asymmetric keys into an (effectively) symmetric key.
/// </summary>
public class UtilHolder : EncryptionUtil {
    private readonly EncryptionUtil enc;
    private readonly Func<byte[], byte[]> dec;

    public override byte[] Encrypt(byte[] b) {
        return enc.Encrypt(b);
    }

    public override byte[] Decrypt(byte[] b) {
        return dec(b);
    }

    /// <summary>
    /// Meant to be used in symmetric situations, which is why you cannot retrieve the decoder.
    /// </summary>
    public EncryptionUtil GetEncoder() => enc;

    // encoding does not effect this implementation. It is marked as symmetric as the same util is used for both decryption and encryption. And I don't want to add a new enum case. That too.
    public UtilHolder(EncryptionUtil one, EncryptionUtil? two = null) : base(KeyType.Symmetric, null) {
        if (two == null) {
            if (one.GetKeyType() != KeyType.Symmetric) throw new KeyTypeException("Symmetric key must be symmetric");
            this.dec = one.Decrypt;
        } else {
            if (one.GetKeyType() != KeyType.Private) throw new KeyTypeException("First key must be private");
            if (two.GetKeyType() != KeyType.Public) throw new KeyTypeException("Second key must be public");
            this.dec = two.Decrypt;
        }
        this.enc = one;
    }
}