namespace CoolandonRS.netlib; 

public class ServerConnectionException : ServerException {
    public ServerConnectionException() {
    }
    public ServerConnectionException(string msg) : base(msg) {
        
    }
}