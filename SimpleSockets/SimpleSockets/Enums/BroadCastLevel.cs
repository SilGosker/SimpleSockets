namespace SimpleSockets.Enums;
[Flags]
public enum BroadCastLevel
{
    Receiver = 1,
    /// <summary>
    /// All the other members that match your <see cref=""/>
    /// </summary>
    Members = 4,
    EveryOne = 5
}