namespace CoolandonRS.netlib; 

public class ServerOperationException : ServerException {
    public ServerOperationException() {
    }
    public ServerOperationException(string msg) : base(msg) {
        
    }
}