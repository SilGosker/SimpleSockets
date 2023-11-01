using EasySockets.Enums;

namespace EasySockets;

public interface IInternalEasySocket
{
	internal void SetRoomId(string roomId);

	internal void SetUserId(string userId);

	internal Func<IEasySocket, BroadCastFilter, string, Task>? Emit { set; }

	internal Action<IEasySocket>? DisposeAtSocketHandler { set; }
}