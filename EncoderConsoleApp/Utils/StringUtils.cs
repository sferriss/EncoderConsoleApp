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
}