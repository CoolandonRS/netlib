namespace netlib; 

public class ServerAuthException : ServerException {
    public ServerAuthException() {
    }
    public ServerAuthException(string msg) : base(msg) {
        
    }
}