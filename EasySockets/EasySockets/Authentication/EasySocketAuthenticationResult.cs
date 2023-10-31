using Microsoft.AspNetCore.Builder;
using EasySockets.Builder;

namespace EasySockets.Authentication;

/// <summary>
///     The combined result of authentication and authorization for a <see cref="EasySocket" /> connection
/// </summary>
public struct EasySocketAuthenticationResult
{
    /// <inheritdoc cref="EasySocketAuthenticationResult(bool, string, string)" />
    public EasySocketAuthenticationResult(bool isAuthenticated)
        : this(isAuthenticated, null, null)
    {
    }

    /// <inheritdoc cref="EasySocketAuthenticationResult(bool, string, string)" />
    public EasySocketAuthenticationResult(bool isAuthenticated, string? roomId)
        : this(isAuthenticated, roomId, null)
    {
    }

    /// <summary>
    ///     Creates a new <see cref="EasySocketAuthenticationResult" /> instance with the given parameters
    /// </summary>
    /// <param name="isAuthenticated">
    ///     <para>
    ///         Whether the authentication result indicated success or failure.
    ///     </para>
    ///     <para>
    ///         Failure (<c>false</c>) will result in a 403 status code. <br />
    ///         Success (<c>true</c>) will result in the websocket being accepted.
    ///     </para>
    /// </param>
    /// <param name="roomId">The id of the room the user will connect to</param>
    /// <param name="userId">The unique user id for the room</param>
    public EasySocketAuthenticationResult(bool isAuthenticated, string? roomId, string? userId)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        RoomId = roomId;
    }

    /// <summary>
    ///     <para>
    ///         Whether the authentication result indicated success or failure.
    ///     </para>
    ///     <para>
    ///         Failure (<c>false</c>) will result in a 401 status code. <br />
    ///         Success (<c>true</c>) will result in the websocket being accepted.
    ///     </para>
    ///     <para>
    ///         The default value is <c>false</c>. <br />
    ///         The default behavior can be changed in the settings when calling the
    ///         <see cref="AppBuilderExtensions.UseEasySockets(IApplicationBuilder, Action{EasySocketMiddlewareOptions})" />
    ///         <br />
    ///         and can be overridden for a individual connections when calling the
    ///         <see cref="EasySocketBuilder.AddEasySocket{TEasySocket}(string, Action{EasySocketOptions})" />
    ///     </para>
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    ///     <para>
    ///         The unique user id for a room
    ///     </para>
    ///     <para>
    ///         The default is a random Guid.<br />
    ///         This behavior can be changed in the settings when calling the
    ///         <see cref="AppBuilderExtensions.UseEasySockets(IApplicationBuilder, Action{EasySocketMiddlewareOptions})" />
    ///     </para>
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    ///     <para>
    ///         The room id the user should join when authenticated
    ///     </para>
    ///     <para>
    ///         The default is <c>"__0"</c>.<br />
    ///         This behavior can be changed in the settings when calling the
    ///         <see cref="AppBuilderExtensions.UseEasySockets(IApplicationBuilder, Action{EasySocketMiddlewareOptions})" />
    ///     </para>
    /// </summary>
    public string? RoomId { get; set; }

    /// <summary>
    ///     Implicitly converts a <see cref="bool" /> into a <see cref="EasySocketAuthenticationResult" /> with the
    ///     <see cref="IsAuthenticated" /> set to the indicated value
    /// </summary>
    /// <param name="authenticationResult">Whether the result should indicate success or failure</param>
    public static implicit operator EasySocketAuthenticationResult(bool authenticationResult)
    {
        return new EasySocketAuthenticationResult(authenticationResult);
    }

    /// <summary>
    ///     Implicitly converts a <see cref="EasySocketAuthenticationResult" /> into a <see cref="bool" />  with the
    ///     <see cref="IsAuthenticated" /> value as the result
    /// </summary>
    public static implicit operator bool(EasySocketAuthenticationResult? authenticationResult)
    {
        return authenticationResult?.IsAuthenticated == true;
    }
}