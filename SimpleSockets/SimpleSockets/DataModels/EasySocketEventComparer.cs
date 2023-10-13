namespace EasySockets.DataModels;

internal readonly struct EasySocketEventComparer
{
    public bool Equals(EasySocketEventComparer other)
    {
        return _type == other._type && _event == other._event;
    }

    public override bool Equals(object? obj)
    {
        return obj is EasySocketEventComparer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_type, _event);
    }

    private readonly Type _type;
    private readonly string _event;
    public EasySocketEventComparer(Type type, string @event)
    {
        _type = type;
        _event = @event;
    }

    public static bool operator ==(EasySocketEventComparer? left, EasySocketEventComparer? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Value._type == right.Value._type && left.Value._event == right.Value._event;
    }

    public static bool operator !=(EasySocketEventComparer? left, EasySocketEventComparer? right)
    {
        return !(left == right);
    }
}