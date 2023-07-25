using SimpleSockets.Extensions;

namespace SimpleSockets.DataModels;

public sealed class SimpleSocketAuthenticationResult
{
    public SimpleSocketAuthenticationResult(bool isAuthenticated, string? userId, string? roomId)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        RoomId = roomId;
    }
    /// <summary>
    /// Whether the user is authenticated and allowed to connect to the server.
    ///
    /// Default is true. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets"/>
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// The unique user id
    ///
    /// Default is a random Guid. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets"/>
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// The room id the user should join when authenticated
    ///
    /// Default is '__0'. This behavior can be changed in the settings when calling the <see cref="AppBuilderExtensions.UseSimpleSockets"/>
    /// </summary>
    public string? RoomId { get; set; }
}