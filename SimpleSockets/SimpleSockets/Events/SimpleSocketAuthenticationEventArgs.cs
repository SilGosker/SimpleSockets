using Microsoft.AspNetCore.Http;

namespace SimpleSockets.Events;

public class SimpleSocketAuthenticationEventArgs
{
    public HttpContext HttpContext;
    public bool IsAuthenticated;
    
    public SimpleSocketAuthenticationEventArgs(HttpContext httpContext)
    {
        HttpContext = httpContext;
        IsAuthenticated = false;
    }
}