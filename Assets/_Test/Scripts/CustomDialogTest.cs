using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// カスタムダイアログのデモ
// 2017/12/03 Fantom (Unity 5.6.3p1)
// http://fantom1x.blog130.fc2.com/blog-entry-282.html
public class CustomDialogTest : MonoBehaviour
{

    public Toggle toggleJson;

    public VolumeSliderDialogController volumeController;


#pragma warning disable 0219, 0649    //Variable, Field is assigned but its value is never used

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    //private void Update()
    //{

    //}


    //JSON 受信用
    [Serializable]
    class Data
    {
        public bool utc;
        public bool pronama;
        public bool query;
        public bool reset;
        public string zokusei = "tsundere";
        public int master = 100;
        public int bgm = 50;
        public int voice = 50;
        public int se = 50;

        public override string ToString()
        {
            return "utc = " + utc + ", pronama = " + pronama + ", query = " + query + ", zokusei = " + zokusei
                + ", master = " + master + ", bgm = " + bgm + ", voice = " + voice + ", se = " + se + ", reset = " + reset;
        }
    }

    const string JSON_PREF = "_json";   //JSON とは保存データを分ける


    public void OpenDialog()
    {
        if (toggleJson != null && toggleJson.isOn)  //JSON 形式
        {
#if UNITY_ANDROID
            Data data = new Data();
            XPlayerPrefs.GetObjectOverwrite(gameObject.name + JSON_PREF, ref data); //保存されたデータがないときはデフォ値となる

            XDebug.Clear(); //※ヒエラルキーにある「DebugConsole」をオンにすると内容が見れます。
            XDebug.Log("(PlayerPrefs or init)");
            XDebug.Log(data, 3);

            DivisorItem divisorItem = new DivisorItem(1);
            TextItem textItem = new TextItem("あんなことやこんなことを設定できます。");
            TextItem textItem1 = new TextItem("嫁のブレンド・S");

            SwitchItem switchItem = new SwitchItem("ユニティちゃん", "utc", data.utc);
            SwitchItem switchItem2 = new SwitchItem("プロ生ちゃん", "pronama", data.pronama);
            SwitchItem switchItem3 = new SwitchItem("クエリちゃん", "query", data.query);

            TextItem textItem2 = new TextItem("あなたの属性");

            ToggleItem toggleItem = new ToggleItem(
                    new String[] { "ツンデレ", "ヤンデレ", "しょびっち" },
                    "zokusei",
                    new String[] { "tsundere", "yandere", "syobicchi" },
                    data.zokusei);

            TextItem textItem3 = new TextItem("サウンド設定");

            Dictionary<string, int> vols;
            if (volumeController != null)
                vols = volumeController.GetVolumes();
            else
                vols = new Dictionary<string, int>() { { "master", 100 }, { "bgm", 50 }, { "voice", 50 }, { "se", 50 } };

            SliderItem sliderItem = new SliderItem("マスター", "master", vols["master"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem1 = new SliderItem("音楽", "bgm", vols["bgm"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem2 = new SliderItem("ボイス", "voice", vols["voice"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem3 = new SliderItem("効果音", "se", vols["se"], 0, 100, 0, 0, "PreviewVolume");

            TextItem textItem4 = new TextItem("リセットすると保存された設定が全て消去されます", Color.red);
            SwitchItem switchItem4 = new SwitchItem("設定のリセット", "reset", false, Color.blue);

            DialogItem[] items = new DialogItem[] {
                        textItem, divisorItem,
                        textItem1, switchItem, switchItem2, switchItem3, divisorItem,
                        textItem2, toggleItem, divisorItem,
                        textItem3, sliderItem, sliderItem1, sliderItem2, sliderItem3, divisorItem,
                        switchItem4, textItem4, divisorItem,
                    };
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidPlugin.ShowCustomDialog("設定いろいろ", "", items, gameObject.name, "OnReceiveResult", true, "決定", "キャンセル");
#endif
        }
        else  //キーと値ペアの形式
        {
#if UNITY_ANDROID
            //Param は基本的に Dictionary と同じもので、値の型変換とデフォルト値を簡単に扱うために用意したラッパー的なクラスと考えて良い
            Param pref = Param.GetPlayerPrefs(gameObject.name, new Param());    //保存されたデータがないときは新規に生成（中身は空）

            XDebug.Clear(); //※ヒエラルキーにある「DebugConsole」をオンにすると内容が見れます。
            XDebug.Log("(PlayerPrefs or init)");
            XDebug.Log(pref, 3);

            DivisorItem divisorItem = new DivisorItem(1);
            TextItem textItem = new TextItem("あんなことやこんなことを設定できます。");
            TextItem textItem1 = new TextItem("嫁のブレンド・S", AndroidColor.WHITE, XColor.ToIntARGB("#ff1493"), "center");  //※色の形式はどれでも同じです（テストで色々試してるだけ）

            SwitchItem switchItem = new SwitchItem("ユニティちゃん", "utc", pref.GetBool("utc", false));
            SwitchItem switchItem2 = new SwitchItem("プロ生ちゃん", "pronama", pref.GetBool("pronama", false));
            SwitchItem switchItem3 = new SwitchItem("クエリちゃん", "query", pref.GetBool("query", false));

            TextItem textItem2 = new TextItem("あなたの属性", XColor.ToColor("#fff"), XColor.ToColor("0x1e90ff"), "center");  //※色の形式はどれでも同じです（テストで色々試してるだけ）

            ToggleItem toggleItem = new ToggleItem(
                    new String[] { "ツンデレ", "ヤンデレ", "しょびっち" },
                    "zokusei",
                    new String[] { "tsundere", "yandere", "syobicchi" },
                    pref.GetString("zokusei", "tsundere"));

            TextItem textItem3 = new TextItem("サウンド設定", XColor.ToIntARGB(Color.white), XColor.ToIntARGB(0x3c, 0xb3, 0x71), "center");//"#3cb371" ※色の形式はどれでも同じです（テストで色々試してるだけ）

            Dictionary<string, int> vols;
            if (volumeController != null)
                vols = volumeController.GetVolumes();
            else
                vols = new Dictionary<string, int>() { { "master", 100 }, { "bgm", 50 }, { "voice", 50 }, { "se", 50 } };

            SliderItem sliderItem = new SliderItem("マスター", "master", vols["master"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem1 = new SliderItem("音楽", "bgm", vols["bgm"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem2 = new SliderItem("ボイス", "voice", vols["voice"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem3 = new SliderItem("効果音", "se", vols["se"], 0, 100, 0, 0, "PreviewVolume");

            TextItem textItem4 = new TextItem("リセットすると保存された設定が全て消去されます", Color.red);
            SwitchItem switchItem4 = new SwitchItem("設定のリセット", "reset", false, Color.blue);

            DialogItem[] items = new DialogItem[] {
                        textItem, divisorItem,
                        textItem1, switchItem, switchItem2, switchItem3, divisorItem,
                        textItem2, toggleItem, divisorItem,
                        textItem3, sliderItem, sliderItem1, sliderItem2, sliderItem3, divisorItem,
                        switchItem4, textItem4, divisorItem,
                    };
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidPlugin.ShowCustomDialog("設定いろいろ", "", items, gameObject.name, "OnReceiveResult", false, "決定", "キャンセル");
#endif
        }
    }


    //設定完了（「OK」時）のコールバックハンドラ
    private void OnReceiveResult(string message)
    {
        XDebug.Clear(); //※ヒエラルキーにある「DebugConsole」をオンにすると内容が見れます。
        XDebug.Log("(OnReceiveResult message)");
        XDebug.Log(message, 3);

        if (!string.IsNullOrEmpty(message))
        {
            Dictionary<string, int> vols = volumeController.GetVolumes();  //音量の保存用

            if (toggleJson != null && toggleJson.isOn)  //JSON 形式
            {
                Data data = JsonUtility.FromJson<Data>(message);
                XDebug.Log("(Parse to Data [from JSON])");
                XDebug.Log(data, 3);

                if (data.reset)  //設定のリセットを実行
                {
                    PlayerPrefs.DeleteKey(gameObject.name + JSON_PREF);
                    volumeController.ResetVolumes();     //初期状態に戻す

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast("設定がリセットされました");
#endif
                }
                else  //値の更新と保存
                {
                    //音量設定の更新（※スライダーの変化コールバック：PreviewVolume() をしている場合は、リアルタイムで反映されているので無くても可）。
                    vols["master"] = data.master;
                    vols["bgm"] = data.bgm;
                    vols["voice"] = data.voice;
                    vols["se"] = data.se;

                    //PlayerPrefs の更新
                    XPlayerPrefs.SetObject(gameObject.name + JSON_PREF, data);
                    volumeController.SetPrefs(vols);
                    PlayerPrefs.Save();

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast(message);
#endif
                }
            }
            else  //キーと値ペアの形式
            {
                Param pref = Param.Parse(message);
                XDebug.Log("(Parse to Param [from key=value])");
                XDebug.Log(pref, 3);

                if (pref["reset"].ToLower() == "true")  //設定のリセットを実行（※値は文字列）
                {
                    PlayerPrefs.DeleteKey(gameObject.name);
                    volumeController.ResetVolumes();     //初期状態に戻す

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast("設定がリセットされました");
#endif
                }
                else  //値の更新と保存
                {
                    //音量設定の更新（※スライダーの変化コールバック：PreviewVolume() をしている場合は、リアルタイムで反映されているので無くても可）。
                    foreach (var key in vols.Keys.ToArray())
                    {
                        vols[key] = int.Parse(pref[key]);
                        pref.Remove(key);   //保存に不要なパラメタを削除
                    }
                    pref.Remove("reset");   //保存に不要なパラメタを削除

                    //PlayerPrefs の更新
                    if (pref.Count > 0)
                        Param.SetPlayerPrefs(gameObject.name, pref);
                    volumeController.SetPrefs(vols);
                    PlayerPrefs.Save();

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast(message);
#endif
                }
            }
        }
    }


    //プレビュー再生コールバックハンドラ（キーが必要）
    private void PreviewVolume(string message)
    {
#if UNITY_EDITOR
        Debug.Log("PreviewVolume : " + message);
#endif
        if (!string.IsNullOrEmpty(message) && volumeController != null)
        {
            string[] param = message.Split('=');  //key=value の形式
            if (param.Length > 1)
            {
                //スライダーのキーから AudioSource を選択
                string key = param[0];
                volumeController.Play(key);

                //音量設定
                float vol = float.Parse(param[1]);
                volumeController.SetVolume(key, vol);
            }
        }
    }

}
