using SimpleSockets.DataModels;

namespace SimpleSockets.Interfaces;

public interface ISimpleSocketAsyncAuthenticator
{
    public Task<SimpleSocketAuthenticationResult> AuthenticateAsyncs();
}