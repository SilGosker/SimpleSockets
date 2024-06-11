using System.Text;

namespace EasySockets.Builder;

public struct ReadonlyEasySocketOptions : IReadonlyEasySocketOptions
{
    public int ReceiveBufferSize { get; init; }

    public int SendBufferSize { get; init; }

    public Encoding Encoding { get; init; }

    public string ClosingStatusDescription { get; init; }

    public bool LoggingEnabled { get; init; }
}