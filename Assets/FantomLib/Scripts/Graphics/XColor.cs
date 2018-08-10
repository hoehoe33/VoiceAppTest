using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FantomLib
{
    /// <summary>
    /// 色関連ユーティリティ
    ///・主に Unity の Color と ARGB 形式の int 値(Android[Java]等) の相互変換する。
    /// [Unity の Color] https://docs.unity3d.com/ja/current/ScriptReference/Color.html
    /// [Android の Color(=int)] https://developer.android.com/reference/android/graphics/Color.html
    /// 2017/12/16 Fantom (Unity 5.6.3p1)
    ///（更新履歴）
    /// 17/11/16・新規リリース
    /// 17/12/03・ToIntARGB(), ToColor() にHTML カラー文字列（"#RRGGBBAA"等）からの変換のオーバーロードを追加。
    /// 17/12/16・RedValue(Color), GreenValue(Color), BlueValue(Color), AlphaValue(Color), ToIntARGB(Color), ToColor() の処理を計算式に変更。
    /// </summary>
    public static class XColor
    {
        /// <summary>
        /// ARGB 形式の int 値に変換する
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>ARGB 形式の int 値</returns>
        public static int ToIntARGB(this Color color)
        {
            //string htmlColor = ColorUtility.ToHtmlStringRGBA(color);    //"RRGGBBAA"
            //int r = RedValue(htmlColor);
            //int g = GreenValue(htmlColor);
            //int b = BlueValue(htmlColor);
            //int a = AlphaValue(htmlColor);

            int r = RedValue(color);
            int g = GreenValue(color);
            int b = BlueValue(color);
            int a = AlphaValue(color);
            return (a << 24) | (r << 16) | (g << 8) | b;    //ARGB
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等） → ARGB 形式の int 値に変換する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>ARGB 形式の int 値</returns>
        public static int ToIntARGB(string htmlString)
        {
            int r = RedValue(htmlString);
            int g = GreenValue(htmlString);
            int b = BlueValue(htmlString);
            int a = AlphaValue(htmlString);
            return (a << 24) | (r << 16) | (g << 8) | b;    //ARGB
        }


        /// <summary>
        /// 色成分(0～255) → ARGB 形式の int 値に変換する
        /// </summary>
        /// <param name="r">赤成分 (0～255)</param>
        /// <param name="g">緑成分 (0～255)</param>
        /// <param name="b">青成分 (0～255)</param>
        /// <param name="a">アルファ成分 (0～255)</param>
        /// <returns>ARGB 形式の int 値</returns>
        public static int ToIntARGB(int r, int g, int b, int a = 255)
        {
            return ((a & 0xff) << 24) | ((r & 0xff) << 16) | ((g & 0xff) << 8) | (b & 0xff);    //ARGB
        }


        /// <summary>
        /// Unity の Color から赤成分の int値を抽出する
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>赤成分の int値 (0～255)</returns>
        public static int RedValue(this Color color)
        {
            //string htmlColor = ColorUtility.ToHtmlStringRGBA(color);    //"RRGGBBAA"
            //return RedValue(htmlColor);
            return Mathf.RoundToInt(color.r * 255);
        }


        /// <summary>
        /// Unity の Color から緑成分の int値を抽出する
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>緑成分の int値 (0～255)</returns>
        public static int GreenValue(this Color color)
        {
            //string htmlColor = ColorUtility.ToHtmlStringRGBA(color);    //"RRGGBBAA"
            //return GreenValue(htmlColor);
            return Mathf.RoundToInt(color.g * 255);
        }


        /// <summary>
        /// Unity の Color から青成分の int値を抽出する
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>青成分の int値 (0～255)</returns>
        public static int BlueValue(this Color color)
        {
            //string htmlColor = ColorUtility.ToHtmlStringRGBA(color);    //"RRGGBBAA"
            //return BlueValue(htmlColor);
            return Mathf.RoundToInt(color.b * 255);
        }


        /// <summary>
        /// Unity の Color からアルファ成分の int値を抽出する
        /// </summary>
        /// <param name="color">UnityEngine.Color</param>
        /// <returns>アルファ成分の int値 (0～255)</returns>
        public static int AlphaValue(this Color color)
        {
            //string htmlColor = ColorUtility.ToHtmlStringRGBA(color);    //"RRGGBBAA"
            //return AlphaValue(htmlColor);
            return Mathf.RoundToInt(color.a * 255);
        }



        /// <summary>
        /// カラーコードとして認識できる文字列を抽出する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="htmlString">HTML カラー文字列</param>
        /// <returns>認識できたカラーコード部分（"ffffffff", "ffffff", "ffff", "fff"等）/認識できなかったとき=空文字("")</returns>
        public static string GetColorCodeString(string htmlString)
        {
            if (htmlString.ToLower().StartsWith("0x"))
                htmlString = htmlString.Substring(2);
            else if (htmlString.StartsWith("#"))
                htmlString = htmlString.Substring(1);

            if (!Regex.IsMatch(htmlString, "^[0-9a-fA-F]{3,8}$"))
                return "";
            if (htmlString.Length == 5 || htmlString.Length == 7)
                return "";

            return htmlString;
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等）から赤成分の int値を抽出する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="htmlString">HTML カラー文字列</param>
        /// <returns>赤成分の int値 (0～255)/認識できなかったとき=0</returns>
        public static int RedValue(string htmlString)
        {
            htmlString = GetColorCodeString(htmlString);
            if (string.IsNullOrEmpty(htmlString))
                return 0;

            if (htmlString.Length == 8 || htmlString.Length == 6)  //"RRGGBBAA" or "RRGGBB"
            {
                string hex = htmlString.Substring(0, 2);
                return Convert.ToInt32(hex, 16);
            }
            if (htmlString.Length == 4 || htmlString.Length == 3)  //"RGBA" or "RGB"
            {
                string hex = htmlString.Substring(0, 1);
                return Convert.ToInt32(hex + hex, 16);
            }
            return 0;
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等）から緑成分の int値を抽出する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="htmlString">HTML カラー文字列</param>
        /// <returns>緑成分の int値 (0～255)/認識できなかったとき=0</returns>
        public static int GreenValue(string htmlString)
        {
            htmlString = GetColorCodeString(htmlString);
            if (string.IsNullOrEmpty(htmlString))
                return 0;

            if (htmlString.Length == 8 || htmlString.Length == 6)  //"RRGGBBAA" or "RRGGBB"
            {
                string hex = htmlString.Substring(2, 2);
                return Convert.ToInt32(hex, 16);
            }
            if (htmlString.Length == 4 || htmlString.Length == 3)  //"RGBA" or "RGB"
            {
                string hex = htmlString.Substring(1, 1);
                return Convert.ToInt32(hex + hex, 16);
            }
            return 0;
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等）から青成分の int値を抽出する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="htmlString">HTML カラー文字列</param>
        /// <returns>青成分の int値 (0～255)/認識できなかったとき=0</returns>
        public static int BlueValue(string htmlString)
        {
            htmlString = GetColorCodeString(htmlString);
            if (string.IsNullOrEmpty(htmlString))
                return 0;

            if (htmlString.Length == 8 || htmlString.Length == 6)  //"RRGGBBAA" or "RRGGBB"
            {
                string hex = htmlString.Substring(4, 2);
                return Convert.ToInt32(hex, 16);
            }
            if (htmlString.Length == 4 || htmlString.Length == 3)  //"RGBA" or "RGB"
            {
                string hex = htmlString.Substring(2, 1);
                return Convert.ToInt32(hex + hex, 16);
            }
            return 0;
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等）からアルファ成分の int値を抽出する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff" (8, 4文字) / "ffff", "fff" (6, 3 文字は必ず 255 になる)
        /// </summary>
        /// <param name="htmlString">HTML カラー文字列</param>
        /// <returns>アルファ成分の int値 (0～255)/認識できなかったとき=0</returns>
        public static int AlphaValue(string htmlString)
        {
            htmlString = GetColorCodeString(htmlString);
            if (string.IsNullOrEmpty(htmlString))
                return 0;

            if (htmlString.Length == 6 || htmlString.Length == 3)  //"RRGGBB", "RGB"
            {
                return 0xff;
            }
            if (htmlString.Length == 8)  //"RRGGBBAA"
            {
                string hex = htmlString.Substring(6, 2);
                return Convert.ToInt32(hex, 16);
            }
            if (htmlString.Length == 4)  //"RGBA"
            {
                string hex = htmlString.Substring(3, 1);
                return Convert.ToInt32(hex + hex, 16);
            }
            return 0;
        }



        /// <summary>
        /// ARGB 形式の int 値 → Unity の Color に変換する
        /// ※小数での表現（Color）は完全に一致はしないので注意。
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>Unity の Color 形式</returns>
        public static Color ToColor(int argb)
        {
            //string htmlString = ToHtmlString(argb);
            //Color color;
            //if (ColorUtility.TryParseHtmlString(htmlString, out color))
            //    return color;
            //return Color.clear;     //変換失敗（無色透明）

            int r = RedValue(argb);
            int g = GreenValue(argb);
            int b = BlueValue(argb);
            int a = AlphaValue(argb);
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }


        /// <summary>
        /// 色成分(0～255) → Unity の Color に変換する
        /// </summary>
        /// <param name="r">赤成分 (0～255)</param>
        /// <param name="g">緑成分 (0～255)</param>
        /// <param name="b">青成分 (0～255)</param>
        /// <param name="a">アルファ成分 (0～255)</param>
        /// <returns>Unity の Color 形式</returns>
        public static Color ToColor(int r, int g, int b, int a = 255)
        {
            //return ToColor(ToIntARGB(r, g, b, a));
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }


        /// <summary>
        /// HTML カラー文字列（"#RRGGBBAA"等）→ Unity の Color に変換する
        /// (対応書式)
        /// [先頭] "0x～", "#～", (なし)
        /// [色部分] "ffffffff", "ffffff", "ffff", "fff" (8, 6, 4, 3 文字)
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>Unity の Color 形式</returns>
        public static Color ToColor(string htmlString)
        {
            //return ToColor(ToIntARGB(htmlString));

            int r = RedValue(htmlString);
            int g = GreenValue(htmlString);
            int b = BlueValue(htmlString);
            int a = AlphaValue(htmlString);
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }


        /// <summary>
        /// ARGB 形式の int 値を HTML カラー文字列に変換する
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <param name="addSharp">頭に'#'を付けて返す（デフォルト）</param>
        /// <returns>HTML カラー文字列（"#ffffffff", "ffffffff"等）</returns>
        public static string ToHtmlString(int argb, bool addSharp = true)
        {
            int r = RedValue(argb);
            int g = GreenValue(argb);
            int b = BlueValue(argb);
            int a = AlphaValue(argb);
            string htmlString = r.ToString("x2") + g.ToString("x2") + b.ToString("x2") + a.ToString("x2");  //"RRGGBBAA"
            return addSharp ? ("#" + htmlString) : htmlString;
        }


        /// <summary>
        /// ARGB 形式の int 値から赤成分の int値を抽出する
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>赤成分の int値 (0～255)</returns>
        public static int RedValue(int argb)
        {
            return ((argb & 0x00ff0000) >> 16);
        }


        /// <summary>
        /// ARGB 形式の int 値から緑成分の int値を抽出する
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>緑成分の int値 (0～255)</returns>
        public static int GreenValue(int argb)
        {
            return ((argb & 0x0000ff00) >> 8);
        }


        /// <summary>
        /// ARGB 形式の int 値から青成分の int値を抽出する
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>青成分の int値 (0～255)</returns>
        public static int BlueValue(int argb)
        {
            return (argb & 0x000000ff);
        }


        /// <summary>
        /// ARGB 形式の int 値からアルファ成分の int値を抽出する
        /// </summary>
        /// <param name="argb">ARGB 形式の int 値</param>
        /// <returns>アルファ成分の int値 (0～255)</returns>
        public static int AlphaValue(int argb)
        {
            return ((argb >> 24) & 0x000000ff); //符号を消す（C#には">>>"がないためマスクする）
        }

    }
}