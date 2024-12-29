namespace Firework.Models.Data;

public class SettingsItem : IEquatable<SettingsItem>, ICloneable
{
    public string UniqueKey { get; set; }

    public string DisplayName { get; set; }
    public string Value { get; set; }

    public Action<string> Handler { get; set; }

    public bool Equals(SettingsItem? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return UniqueKey == other.UniqueKey && DisplayName == other.DisplayName && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SettingsItem)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UniqueKey, DisplayName, Value, Handler);
    }

    public object Clone()
    {
        return new SettingsItem
        {
            UniqueKey = UniqueKey,
            DisplayName = DisplayName,
            Handler = Handler,
            Value = Value
        };
    }
}