namespace SimpleSockets.DataModels;

internal readonly struct SimpleSocketEventComparer
{
    public bool Equals(SimpleSocketEventComparer other)
    {
        return _type == other._type && _event == other._event;
    }

    public override bool Equals(object? obj)
    {
        return obj is SimpleSocketEventComparer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_type, _event);
    }

    private readonly Type _type;
    private readonly string _event;
    public SimpleSocketEventComparer(Type type, string @event)
    {
        _type = type;
        _event = @event;
    }

    public static bool operator ==(SimpleSocketEventComparer? left, SimpleSocketEventComparer? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Value._type == right.Value._type && left.Value._event == right.Value._event;
    }

    public static bool operator !=(SimpleSocketEventComparer? left, SimpleSocketEventComparer? right)
    {
        return !(left == right);
    }
}