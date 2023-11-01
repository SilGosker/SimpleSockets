using EasySockets.Enums;

namespace EasySockets;

public interface IInternalEasySocket
{
	internal string InternalRoomId { set; }
	internal string InternalUserId { set; }

	internal Func<IEasySocket, BroadCastFilter, string, Task>? Emit { set; }

	internal Action<IEasySocket>? DisposeAtSocketHandler { set; }
}