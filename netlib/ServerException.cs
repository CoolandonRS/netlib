namespace CoolandonRS.netlib; 

public class ServerException : Exception {
    public ServerException() {
    }
    public ServerException(string msg) : base(msg) {
        
    }
}