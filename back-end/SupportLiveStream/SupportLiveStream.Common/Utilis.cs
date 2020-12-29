using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SupportLiveStream.Common
{
    public class Utilis
    {
        public static string ConvertToUnicode(string x_strInput)
        {
            try
            {
                var arr = x_strInput.Split("data:");
                if(arr == null || arr.Length < 2)
                {
                    return "no_data";
                }
                string strPattern = @"\\u....";
                StringBuilder strInput = new StringBuilder(arr[1]);
                MatchCollection matchCollection = Regex.Matches(strInput.ToString(), strPattern);
                string unicodeCode, text;
                foreach (Match match in matchCollection)
                {
                    unicodeCode = match.Value.Substring(2, 4);
                    text = Char.ConvertFromUtf32(int.Parse(unicodeCode, System.Globalization.NumberStyles.HexNumber));
                    strInput.Replace(@"\u" + unicodeCode, text);
                }
                return strInput.ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return "has_emoji";
            }

        }
    }
}
