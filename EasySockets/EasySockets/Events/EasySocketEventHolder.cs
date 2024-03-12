using EasySockets.DataModels;

namespace EasySockets.Events;
internal static class EasySocketEventHolder
{
    internal static readonly Dictionary<EasySocketEventComparer, Func<string, Task>> Events = new();
}