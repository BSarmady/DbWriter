using System;
using System.Text.RegularExpressions;

namespace DbWriter {

    internal static class Extensions {

        #region public static bool ToBoolean(...)
        public static bool ToBoolean(this object obj, bool Default = false) {
            try {
                return Convert.ToBoolean(obj);
            } catch {
                return Default;
            }
        }
        #endregion

        #region public static Int32 ToInt32(...)
        public static Int32 ToInt32(this object obj, Int32 Default = 0) {
            try {
                return Convert.ToInt32(obj);
            } catch {
                return Default;
            }
        }
        #endregion

        #region public static DateTime ToDateTime(...)
        public static DateTime ToDateTime(this object data, DateTime default_value) {
            try {
                return Convert.ToDateTime(data);
            } catch {
                return default_value;
            }
        }
        #endregion

        #region public static string PadStringAtNewLines(...)
        public static string PadStringAtNewLines(this object inString, int Padding = 4) {
            return inString.ToString().Trim('\n', '\r', ' ').Replace(Environment.NewLine, Environment.NewLine + "".PadRight(Padding));
        }
        #endregion

        #region public static string AddTrailingBackSlashes(...)
        public static string AddTrailingBackSlashes(this string inString) {
            if (inString[inString.Length - 1] == '\\')
                return inString;
            return inString + "\\";
        }
        #endregion

        #region public static string SplitAtWords(...)
        public static string SplitAtWords(this string inString, int maxlen = 120) {
            return Regex.Replace(inString, @"(.{1," + maxlen + @"})(?:\s|$)", "$1" + Environment.NewLine);
        }
        #endregion

        #region public static string ToDateTimeStr(...)
        /// <summary>
        /// Returns string representation of the date returned from table row data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format">DateTime Format parameter</param>
        /// <returns>if value is null, it will return empty string otherwise formatted datetime string </returns>
        public static string ToDateTimeStr(this object data, string format) {
            return data == DBNull.Value ? "" : Convert.ToDateTime(data).ToString(format);
        }
        #endregion

        #region public static string DecodeHTML(
        public static string DecodeHTML(this string inString) {
            return System.Web.HttpUtility.HtmlDecode(inString).TrimEnd(new char[] { ' ', '\r', '\n' });
        }
        #endregion
    }
}