using System;
using System.Text;
using System.Text.RegularExpressions;

namespace support_live_stream.Utilis
{
    public class Utilis
    {
        public static string ConvertToUnicode(string x_strInput)
        {
            try
            {
                var strData = x_strInput.Split("data:");
                if (x_strInput.Contains(":ping") || strData.Length < 2)
                {
                    return "no_data";
                }
                string strPattern = @"\\u....";
                StringBuilder strInput = new StringBuilder(strData[1]);
                MatchCollection matchCollection = Regex.Matches(strInput.ToString(), strPattern);
                string unicodeCode, text;
                foreach (Match match in matchCollection)
                {
                    unicodeCode = match.Value.Substring(2, 4);
                    text = Char.ConvertFromUtf32(int.Parse(unicodeCode, System.Globalization.NumberStyles.HexNumber));
                    strInput.Replace(@"\u" + unicodeCode, text);
                }
                return strInput.ToString().Trim();
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return "has_emoji";
            }

        }
    }
}
