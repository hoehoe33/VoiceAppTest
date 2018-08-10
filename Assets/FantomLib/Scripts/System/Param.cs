using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FantomLib
{
    /// <summary>
    /// Dictionary の値取得などを少し楽に使うクラス
    ///・キーと値は全て string 型で保持されている。取得時に型変換とデフォルト値を指定できる。
    ///・"key1=value1" のようなテキスト形式を変換（Parse(), ParseToDictionary()）して簡単に扱うために作ったラッパー的なクラス。
    /// </summary>
    public class Param : Dictionary<string, string>
    {

        //====================================================================
        //コンストラクタ

        public Param() : base() { }

        public Param(Dictionary<string, string> dic) : base(dic) { }


        //====================================================================
        //値の取得＆セット

        public string GetString(string key, string def = "")
        {
            return ContainsKey(key) ? this[key] : def;
        }

        public int GetInt(string key, int def = 0)
        {
            try {
                return ContainsKey(key) ? int.Parse(this[key]) : def;
            }
            catch {
                return def;
            }
        }

        public float GetFloat(string key, float def = 0)
        {
            try {
                return ContainsKey(key) ? float.Parse(this[key]) : def;
            }
            catch {
                return def;
            }
        }

        public bool GetBool(string key, bool def = false)
        {
            try {
                return ContainsKey(key) ? bool.Parse(this[key]) : def;
            }
            catch {
                return def;
            }
        }

        public void Set(string key, object value)
        {
            this[key] = value.ToString();
        }


        //====================================================================
        //その他

        public override string ToString()
        {
            if (Count > 0)
                return this.Select(e => e.Key + " => " + e.Value).Aggregate((s, a) => s + ", " + a).ToString();
            return "";
        }


        //====================================================================
        //静的関数など

        /// <summary>
        /// テキストをパースして辞書を生成する
        ///・文字列："key1=value1\nkey2=value2\nkey3=value3" などキーと値のペアになっているテキストを区切り文字で分割して辞書を作る。
        ///・不正なテキストはチェックしてないので注意（※キーが重複してるとエラー（戻値が null）となるので注意）。
        ///・生成された辞書はキーと値共に文字列型となる。
        /// </summary>
        /// <param name="text">パースするテキスト</param>
        /// <param name="itemSeparator">項目ごとの区切り文字</param>
        /// <param name="pairSeparator">キーと値の区切り文字</param>
        /// <returns>キーと値で作成された辞書</returns>
        public static Dictionary<string, string> ParseToDictionary(string text, char itemSeparator = '\n', char pairSeparator = '=')
        {
            if (string.IsNullOrEmpty(text))
                return null;

            try {
                return text.Split(new char[] { itemSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Split(new char[] { pairSeparator }, 2))
                    .ToDictionary(a => a[0], a => a[1]);    //※キーが重複してるとエラーとなるので注意
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// テキストをパースして Param（辞書）を生成する
        ///・文字列："key1=value1\nkey2=value2\nkey3=value3" などキーと値のペアになっているテキストを区切り文字で分割して辞書を作る。
        ///・不正なテキストはチェックしてないので注意。
        ///・生成された辞書はキーと値共に文字列型となる。
        /// </summary>
        /// <param name="text">パースするテキスト</param>
        /// <param name="itemSeparator">項目ごとの区切り文字</param>
        /// <param name="pairSeparator">キーと値の区切り文字</param>
        /// <returns>キーと値で作成された Param</returns>
        public static Param Parse(string text, char itemSeparator = '\n', char pairSeparator = '=')
        {
            Dictionary<string, string> dic = ParseToDictionary(text, itemSeparator, pairSeparator);
            return (dic != null) ? new Param(dic) : null;
        }


        /// <summary>
        /// 辞書として JSON 形式（文字列型）に変換して PlayerPrefs に保存する
        ///※Param は基本的に辞書と同じでパラメタを簡単に扱うクラス（Dictionary を継承している）ため、内容的には XPlayerPrefs.SetDictionary() と同じ。
        ///・JSON ではキー配列と値配列として保存される（＝TryGetArrayPair()でペアの配列としても取得できる）。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="param">保存する辞書</param>
        public static void SetPlayerPrefs(string key, Param param)
        {
            XPlayerPrefs.SetDictionary(key, param);
        }


        /// <summary>
        /// PlayerPrefs に JSON 形式（文字列型）で保存された要素を辞書として生成して返す
        ///※Param は基本的に辞書と同じでパラメタを簡単に扱うクラス（Dictionary を継承している）ため、内容的には XPlayerPrefs.GetDictionary() と同じ。
        ///・JSON では辞書もキー配列と値配列として保存されるため、XPlayerPrefs.SetArrayPair() 保存したペア配列も辞書として取得できる。
        /// </summary>
        /// <param name="key">保存キー</param>
        /// <param name="def">デフォルト値</param>
        /// <returns>保存されていた辞書（新規に生成）</returns>
        public static Param GetPlayerPrefs(string key, Param def = null)
        {
            Dictionary<string, string> dic = XPlayerPrefs.GetDictionary<string, string>(key);
            return (dic != null) ? new Param(dic) : def;
        }

    }

}