namespace Octokit.Webhooks.Extensions;

using System;
using System.Globalization;
using System.Text.Json;
using JetBrains.Annotations;

[PublicAPI]
public struct StringEnum<TEnum> : IEquatable<StringEnum<TEnum>>
    where TEnum : struct
{
    private TEnum? parsedValue;

    public StringEnum(TEnum parsedValue)
    {
        if (!Enum.IsDefined(typeof(TEnum), parsedValue))
        {
            throw GetArgumentException(parsedValue.ToString());
        }

        this.StringValue = JsonSerializer.Serialize(parsedValue);
        this.parsedValue = parsedValue;
    }

    public StringEnum(string stringValue)
    {
        this.StringValue = stringValue;
        this.parsedValue = null;
    }

    public string StringValue { get; }

    public TEnum Value => this.parsedValue ?? (this.parsedValue = this.ParseValue()).Value;

    public static implicit operator StringEnum<TEnum>(string value) => new(value);

    public static implicit operator StringEnum<TEnum>(TEnum parsedValue) => new(parsedValue);

    public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right) => left.Equals(right);

    public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right) => !left.Equals(right);

    public bool TryParse(out TEnum value)
    {
        if (this.parsedValue.HasValue)
        {
            value = this.parsedValue.Value;
            return true;
        }

        try
        {
            value = JsonSerializer.Deserialize<TEnum>(this.StringValue);
            this.parsedValue = value;
            return true;
        }
        catch (ArgumentException)
        {
            value = default;
            return false;
        }
    }

    public bool Equals(StringEnum<TEnum> other)
    {
        if (this.TryParse(out var value) && other.TryParse(out var otherValue))
        {
            return value.Equals(otherValue);
        }

        return string.Equals(this.StringValue, other.StringValue, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        return obj is StringEnum<TEnum> @enum && this.Equals(@enum);
    }

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.StringValue);

    public override string ToString() => this.StringValue;

    private static ArgumentException GetArgumentException(string? value) => new(string.Format(
        CultureInfo.InvariantCulture,
        "Value '{0}' is not a valid '{1}' enum value.",
        value,
        typeof(TEnum).Name));

    private TEnum ParseValue()
    {
        if (this.TryParse(out var value))
        {
            return value;
        }

        throw GetArgumentException(this.StringValue);
    }
}
