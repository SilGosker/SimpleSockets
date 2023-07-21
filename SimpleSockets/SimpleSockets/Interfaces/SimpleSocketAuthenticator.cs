using SimpleSockets.DataModels;

namespace SimpleSockets.Interfaces;

public interface ISimpleSocketAuthenticator
{
    public SimpleSocketAuthenticationResult Authenticate();
}