namespace EncoderConsoleApp.Utils;

public static class BytesUtils
{
    public static byte[] AddOneInEachByte(byte[] bytes)
    {
        return bytes.Select(Sum).ToArray();
    }
    
    public static byte[] SubOneInEachByte(byte[] bytes)
    {
        return bytes.Select(Sub).ToArray();
    }

    public static byte[] ToByteArray(string str) {

        var codewordsSplit = new List<string>(str.Split("(?<=/G.{8})"));
        var bits = new byte[codewordsSplit.Count];
        for (var i = 0; i < codewordsSplit.Count; i++) {
            bits[i] = ConvertToByte(codewordsSplit[i]);
        }
        return bits;
    }

    public static string IntegerToStringBinary(int integer, int finalLengthOfBinary) {
        var binary = Convert.ToString(integer, 2);
        return binary.PadLeft(finalLengthOfBinary, '0');
    }
    
    public static byte Count(string str)
    {
        if (str.StartsWith('0'))
        {
            return (byte) str.Split('1')[0].Length;
        }
        return (byte) str.Split('0')[0].Length;
    }

    private static byte ConvertToByte(string str) {
        return Convert.ToByte(str, 2);
    }
    
    public static byte BitwiseEncode(byte b)
    {
        return (byte)(b >> 1);
    }

    public static byte BitwiseDecode(byte b)
    {
        return (byte)(b << 1);
    }
    
    private static byte Sum (byte value)
    {
        var te = (byte)(value + 1);
        return (byte)(value + 1);
    }
    
    private static byte Sub (byte value)
    {
        return (byte) (value - 1);
    }
}