namespace EncoderConsoleApp.Utils;

public class StringUtils
{
    public static List<string> Split8BitsList(List<string> list)
    {
        var newList = new  List<string>();
        string strControl;
        
        foreach (var item in list)
        {
            strControl = item;
            while (strControl.Length >= 8)
            {
                newList.Add(strControl[..8]);
                strControl = strControl[8..];
            }
        }

        return newList;
    }
    
    public static IList<string> Split8BitsList(string str)
    {
        var newList = new  List<string>();
        var strControl = str;
        
        while (strControl.Length >= 8)
        {
            newList.Add(strControl[..8]);
            strControl = strControl[8..];
        }

        var fillWithZero = strControl.PadRight(8, '0');
        newList.Add(fillWithZero);

        return newList;
    }
}