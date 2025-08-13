using System.Reflection;
using System.Text;
using GreenDonut.Data.Cursors.Serializers;

namespace RealTaskManager.GraphQL.Extensions;

public class EnumCursorKeySerializer<TEnum> : ICursorKeySerializer
    where TEnum : struct, Enum
{
    public bool IsSupported(Type type)
    {
        return type == typeof(TEnum);
    }

    public MethodInfo GetCompareToMethod(Type type)
    {
        return typeof(TEnum).GetMethod(nameof(IComparable.CompareTo), new[] { typeof(object) })!;
    }

    public object Parse(ReadOnlySpan<byte> formattedKey)
    {
        var str = Encoding.UTF8.GetString(formattedKey);
        if (Enum.TryParse<TEnum>(str, out var value))
        {
            return value;
        }

        throw new FormatException($"Cannot parse '{str}' into enum {typeof(TEnum).Name}");
    }

    public bool TryFormat(object key, Span<byte> buffer, out int written)
    {
        if (key is TEnum enumValue)
        {
            var str = enumValue.ToString();
            byte[] encoded = Encoding.UTF8.GetBytes(str);

            if (encoded.Length > buffer.Length)
            {
                written = 0;
                return false;
            }

            encoded.CopyTo(buffer);
            written = encoded.Length;
            return true;
        }

        written = 0;
        return false;
    }
}