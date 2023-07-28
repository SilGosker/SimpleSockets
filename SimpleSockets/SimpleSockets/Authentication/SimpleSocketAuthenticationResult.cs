using Microsoft.AspNetCore.Builder;
using SimpleSockets.Extensions;
using SimpleSockets.Options;

namespace SimpleSockets.Authentication;

public sealed class SimpleSocketAuthenticationResult
{
    /// <summary>
    /// Converts a boolean into a default <see cref="SimpleSocketAuthenticationResult"/> with the <see cref="IsAuthenticated"/> set to the value
    /// </summary>
    /// <param name="authenticationResult">Whether the result should indicate success or failure</param>
    public static implicit operator SimpleSocketAuthenticationResult(bool authenticationResult)
    {
        return new SimpleSocketAuthenticationResult(authenticationResult);
    }

    /// <summary>
    /// Converts a <see cref="SimpleSocketAuthenticationResult"/> into a boolean with the <see cref="IsAuthenticated"/> value
    /// </summary>
    /// <param name="authenticationResult">Whether the result indicated success or failure</param>
    public static implicit operator bool(SimpleSocketAuthenticationResult? authenticationResult)
    {
        return authenticationResult?.IsAuthenticated == true;
    }

    public SimpleSocketAuthenticationResult(bool isAuthenticated, string? roomId) : this(isAuthenticated, roomId, null)
    {
    }

    public SimpleSocketAuthenticationResult(bool isAuthenticated) : this(isAuthenticated, null, null)
    {
    }
    
    public SimpleSocketAuthenticationResult(bool isAuthenticated, string? roomId, string? userId)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        RoomId = roomId;
    }
    /// <summary>
    /// Whether the user is authenticated and allowed to connect to the server.
    ///
    /// Default is true. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets(IApplicationBuilder, Action{SimpleSocketMiddlewareOptions})"/>
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// A unique user id for a room
    ///
    /// The default is a random Guid. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets(IApplicationBuilder, Action{SimpleSocketMiddlewareOptions})"/>
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// The room id the user should join when authenticated
    ///
    /// The default is '__0'. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets(IApplicationBuilder, Action{SimpleSocketMiddlewareOptions})"/>
    /// </summary>
    public string? RoomId { get; set; }
}