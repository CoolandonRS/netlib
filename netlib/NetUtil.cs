using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace netlib; 

public static class NetUtil {
    private static SHA256 sha = SHA256.Create();
    public static string GetPlatformIdentifier() {
        var id = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) id += "win";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) id += "linux";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) id += "mac";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) id += "bsd";
        id += "-";
        id += RuntimeInformation.ProcessArchitecture switch {
            Architecture.Arm => "arm32",
            Architecture.Arm64 => "arm64",
            Architecture.S390x => "s390x",
            Architecture.Wasm => "wasm",
            Architecture.X86 => "32",
            Architecture.X64 => "64",
            _ => throw new InvalidEnumArgumentException()
        };
        return id;
    }

    public static bool IsPlatformIdentifier(string str) {
        return Regex.Match(str, "(win|linux|max|bsd)-(arm32|arm64|s39s0x|wasm|32|64)").Success;
    }

    public static string GetSha256Sum(byte[] data) {
        return Convert.ToBase64String(sha.ComputeHash(data));
    }

    public static string GetSha256Sum(string str, Encoding? encoding = null) {
        return GetSha256Sum((encoding ?? Encoding.UTF8).GetBytes(str));
    }
    
    public static bool IsAck(string msg) {
        if (msg.Contains("ACK")) return true;
        if (msg.Contains("NAK") || msg.Length == 0) return false;
        throw new InvalidOperationException("No ACK or NAK found");
    }
}