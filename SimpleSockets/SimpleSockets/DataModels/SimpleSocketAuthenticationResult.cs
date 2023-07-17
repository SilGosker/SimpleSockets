namespace SimpleSockets.DataModels;

public class SimpleSocketAuthenticationResult
{
    public SimpleSocketAuthenticationResult(bool isAuthenticated, string? userId, string? roomId)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        RoomId = roomId;
    }
    /// <summary>
    /// Whether the user is authenticated
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// The unique user id
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// The room id the user should join when authenticated
    /// </summary>
    public string? RoomId { get; set; }
}