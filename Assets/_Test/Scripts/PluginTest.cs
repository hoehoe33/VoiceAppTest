using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// Android ウィジェットたちのデモ
// 2018/02/03 Fantom (Unity 5.6.3p1)
// http://fantom1x.blog130.fc2.com/blog-entry-273.html
//（更新履歴）
// 17/10/01・新規リリース
// 17/10/28・StartActionURI() の例追加（TestOpenURL() ※ただし、コメントアウトされている）。
// 17/11/03・単一行テキスト入力ダイアログのサンプルを追加（TestDialogText()）。
// 17/11/10・複数行テキスト入力ダイアログのサンプルを追加（TestDialogTextMulitLine()）。
//         ・StartActionURI のサンプルを追加（TestActionURI()）。
//         ・数値/半角英数/パスワード入力ダイアログのサンプルを追加（TestDialogNumeric()/TestDialogAlphaNumeric()/TestDialogPassword()）。
// 17/11/12・チェックボックス付きのダイアログのサンプルを追加（TestDialogYesNo(), TestDialogOK()）。
//         ・日付/時刻選択ダイアログのサンプルを追加（TestDatePicker(), TestTimePicker(), OnReceiveChecked()）
// 17/11/16・int値 ARGB形式 と Unity の Color 形式の相互変換（XColor クラス）のサンプルを追加（エディタのみ）。
// 17/11/17・スイッチ選択ダイアログのサンプルを追加（TestDialogSwitch(), ReceiveSwitches()）。
// 17/11/22・選択系のダイアログのサンプルを追加（TestDialogSelect(), TestDialogSingleChoice(), TestDialogMultiChoice()）。
//         ・配列と真偽値の保存サンプルを追加（XPlayerPrefs.SetArray()/SetBool()）
// 17/12/03・ハードウェア音量キーの使用可否（起動時プロパティ同期用）のUIを追加（イベントは HardVolumeController のプロパティ変更に使われる）。
// 18/02/03・単一選択（SingleChoiceDialog）,複数選択（MultiChoiceDialog）の値変更コールバックのテストを追加（※コメントアウトされている）。
public class PluginTest : MonoBehaviour
{

    public Text languageText;               //システムの言語の表示用
    public string openURL;                  //Notification/ボタンで開くURL
    public Toggle ynCheckToggle;            //チェックボックス付きのYesNoダイアログにする
    public Toggle okCheckToggle;            //チェックボックス付きのOKダイアログにする
    public Toggle switchKeyToggle;          //スイッチダイアログの戻り値をキー＆値ペアにする
    public Toggle hardVolToggle;            //ハードウェア音量キーの使用可否（起動時プロパティ同期用）

    const string CHECKED_PREF = "_checked"; //チェックボックスの状態保存用

#if UNITY_EDITOR
    public Color debugColor;
    public Color outColor;
#endif

    // Use this for initialization
    private void Start()
    {
        //システム言語を取得
        if (languageText != null)
        {
            SystemLanguage lang = Application.systemLanguage;
            var strVal = Enum.GetName(typeof(SystemLanguage), lang);
            languageText.text = "Language = " + (int)lang + " (" + strVal + ")";
        }

        //チェックボックスの状態読み込み
        bool check = XPlayerPrefs.GetBool(gameObject.name + CHECKED_PREF, true);
        if (ynCheckToggle != null)
            ynCheckToggle.isOn = check;

        if (okCheckToggle != null)
            okCheckToggle.isOn = check;

        if (hardVolToggle != null)
            hardVolToggle.isOn = FindObjectOfType<HardVolumeController>().HardOperation;

#if UNITY_EDITOR
        //色変換のテスト
        string htmlString = ColorUtility.ToHtmlStringRGBA(debugColor);
        Debug.Log("htmlString = " + htmlString);
        Debug.Log("GetColorCodeString = " + XColor.GetColorCodeString(htmlString));
        Debug.Log("RedValue(html)  = " + XColor.RedValue(htmlString));
        Debug.Log("RedValue(Color) = " + debugColor.RedValue());
        Debug.Log("GreenValue(html)  = " + XColor.GreenValue(htmlString));
        Debug.Log("GreenValue(Color) = " + debugColor.GreenValue());
        Debug.Log("BlueValue(html)  = " + XColor.BlueValue(htmlString));
        Debug.Log("BlueValue(Color) = " + debugColor.BlueValue());
        Debug.Log("AlphaValue(html)  = " + XColor.AlphaValue(htmlString));
        Debug.Log("AlphaValue(Color) = " + debugColor.AlphaValue());
        int argb = debugColor.ToIntARGB();
        Debug.Log("ToIntARGB = " + argb + ", ToColor->html = " + ColorUtility.ToHtmlStringRGBA(XColor.ToColor(argb)));
        outColor = XColor.ToColor(argb);
#endif
    }


    public void OnDestroy()     //アプリ終了時にも呼び出すため public
    {
#if UNITY_EDITOR
        Debug.Log("AndroidPlugin.Release called");
#elif UNITY_ANDROID
        AndroidPlugin.Release(); //受信用リスナーも解除
#endif
    }


    // Update is called once per frame
    //private void Update()
    //{

    //}


    //トーストを表示
    public void ShowToast(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ShowToast : " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message);
#endif
    }


    //遅延でトーストを表示する
    private IEnumerator DelayedToast(string message, float seconds, bool longDuration = false)
    {
        yield return new WaitForSeconds(seconds);
#if UNITY_EDITOR
        Debug.Log("DelayedToast : message = " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message, longDuration);
#endif
    }


    //トーストのデモ（時刻を表示）
    public void TestToast()
    {
        DateTime dt = DateTime.Now;
        ShowToast(dt.ToString("現在の時刻は HH:mm:ss です"));
    }


    //トーストを表示を中断
    public void CancelToast()
    {
#if UNITY_EDITOR
        Debug.Log("CancelToast called");
#elif UNITY_ANDROID
        AndroidPlugin.CancelToast();
#endif
    }


    //Yes/No ダイアログのデモ
    public void TestDialogYesNo()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogYesNo called");
#elif UNITY_ANDROID
        if (ynCheckToggle != null && ynCheckToggle.isOn)
            AndroidPlugin.ShowDialogWithCheckBox("教えて！エロい人！", "大丈夫？ちっぱい揉む？", "今後このダイアログを表示しない", Color.cyan, ynCheckToggle.isOn, gameObject.name, "OnReceiveChecked", "はい", "ちっぱい言うなー！！", "いいえ", "変態紳士というわけですね…", "android:Theme.DeviceDefault.Dialog.Alert");
        else
            AndroidPlugin.ShowDialog("教えて！エロい人！", "大丈夫？ちっぱい揉む？", gameObject.name, "ShowToast", "はい", "ちっぱい言うなー！！", "いいえ", "変態紳士というわけですね…", "android:Theme.DeviceDefault.Light.Dialog.Alert");
#endif
    }


    //OK ダイアログのデモ
    public void TestDialogOK()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogOK called");
#elif UNITY_ANDROID
        if (okCheckToggle != null && okCheckToggle.isOn)
            AndroidPlugin.ShowDialogWithCheckBox("君は何のフレンズ？", "ホワイトなフレンズなんだね！", "今後このダイアログを表示しない", 0, okCheckToggle.isOn, gameObject.name, "OnReceiveChecked", "OK", "Jアラートで訓練やーめーてー！！", "android:Theme.DeviceDefault.Light.Dialog.Alert");
        else
            AndroidPlugin.ShowDialog("君は何のフレンズ？", "ブラックなフレンズなんだね！", gameObject.name, "ShowToast", "OK", "ミサイルやーめーてー！！", "android:Theme.DeviceDefault.Dialog.Alert");
#endif
    }


    //チェックボックス付きダイアログのコールバックハンドラ
    private void OnReceiveChecked(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnReceiveChecked : " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message);
#endif
        bool check = message.Contains("CHECKED_TRUE");

        if (ynCheckToggle != null)
            ynCheckToggle.isOn = !check;

        if (okCheckToggle != null)
            okCheckToggle.isOn = !check;

        XPlayerPrefs.SetBool(gameObject.name + CHECKED_PREF, !check);
        PlayerPrefs.Save();
    }


    //選択ダイアログのデモ
    public void TestDialogSelect()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogSelect called");
#elif UNITY_ANDROID
        string[] items = { "ツンデレ", "ヤンデレ", "クーデレ", "デレデレ", "しょびっち" };
        AndroidPlugin.ShowSelectDialog("あなたの好みはどんなタイプ？", items, gameObject.name, "ShowToast");
#endif
    }


    //単一選択ダイアログのデモ
    public void TestDialogSingleChoice()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogSingleChoice called");
#elif UNITY_ANDROID
        string[] items = { "ツンデレ", "ヤンデレ", "クーデレ", "デレデレ", "しょびっち" };
        //string[] values = { "tsun", "yan", "cool", "dere", "syobi" };
        AndroidPlugin.ShowSingleChoiceDialog("あなたの好みはどんなタイプ？", items, 0, gameObject.name, "ShowToast");
        //AndroidPlugin.ShowSingleChoiceDialog("あなたの好みはどんなタイプ？", items, 0, gameObject.name, "ShowToast", "SingleChoiceChanged", false);
        //AndroidPlugin.ShowSingleChoiceDialog("あなたの好みはどんなタイプ？", items, 0, gameObject.name, "ShowToast", "SingleChoiceChanged", true);
        //AndroidPlugin.ShowSingleChoiceDialog("あなたの好みはどんなタイプ？", items, 0, gameObject.name, "ShowToast", "SingleChoiceChanged", values);
#endif
    }

    //値変更のコールバックハンドラ
    private void SingleChoiceChanged(string result)
    {
        XDebug.Log("SingleChoiceChanged : " + result);
    }


    //複数選択ダイアログのデモ
    public void TestDialogMultiChoice()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogMultiChoice called");
#elif UNITY_ANDROID
        string[] items = { "ツンデレ", "ヤンデレ", "クーデレ", "デレデレ", "しょびっち" };
        //string[] values = { "tsun", "yan", "cool", "dere", "syobi" };
        AndroidPlugin.ShowMultiChoiceDialog("あなたの好みはどんなタイプ？", items, null, gameObject.name, "ShowToast");
        //AndroidPlugin.ShowMultiChoiceDialog("あなたの好みはどんなタイプ？", items, null, gameObject.name, "ShowToast", "MultiChoiceChanged", false);
        //AndroidPlugin.ShowMultiChoiceDialog("あなたの好みはどんなタイプ？", items, null, gameObject.name, "ShowToast", "MultiChoiceChanged", true);
        //AndroidPlugin.ShowMultiChoiceDialog("あなたの好みはどんなタイプ？", items, null, gameObject.name, "ShowToast", "MultiChoiceChanged", values);
#endif
    }

    //値変更のコールバックハンドラ
    private void MultiChoiceChanged(string result)
    {
        XDebug.Log("MultiChoiceChanged : " + string.Join(", ", result.Split('\n')));
    }


    //通知のデモ
    public void TestNotification()
    {
#if UNITY_EDITOR
        Debug.Log("TestNotification called");
#elif UNITY_ANDROID
        if (!string.IsNullOrEmpty(openURL))
            AndroidPlugin.ShowNotificationToOpenURL(Application.productName, "ホームページを開くよ！", openURL);
        else
            AndroidPlugin.ShowNotification(Application.productName, "すっごーい！たーのしー！！");

        AndroidPlugin.ShowToast("通知を見てね！");
#endif
    }


    //URLを開く
    public void TestOpenURL()
    {
#if UNITY_EDITOR
        Debug.Log("StartOpenURL : url = " + openURL);
        Application.OpenURL(openURL);
#elif UNITY_ANDROID
        AndroidPlugin.StartOpenURL(openURL);    //HPを開く
        //AndroidPlugin.StartActionURI("android.intent.action.VIEW", openURL);  //HPを開く（※同じ）
#endif
    }


    //URLを開く
    public void TestActionURI(int what = 0)
    {
#if UNITY_EDITOR
        Debug.Log("StartActionURI");
#elif UNITY_ANDROID
        switch (what)
        {
            default:
            case 0:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", openURL);  //HPを開く（※StartOpenURL() と同じ）
                break;
            case 1:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", "geo:37.7749,-122.4194?q=restaurants");   //マップ（検索:restaurants）
                break;
            case 2:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", "google.streetview:cbll=29.9774614,31.1329645&cbp=0,30,0,0,-15");   //ストリートビュー
                break;
            case 3:
                AndroidPlugin.StartActionURI("android.intent.action.SENDTO", "mailto:xxx@example.com?subject=Title&body=Message");   //メーラー起動
                break;
        }
#endif
    }


#pragma warning disable 0414    //The private field value is never used

    //テキスト入力ダイアログの呼び出しのデモ
    //※複数行のデモは「TextToSpeechTest.cs」（音声認識/テキスト読み上げのテキストの編集）にも使用。
    private string editText = "なんちゃらかんちゃら";

    //単一行のテキスト入力
    public void TestDialogText()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogText called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowSingleLineTextDialog("テキストを編集", editText, 16, this.gameObject.name, "OnReceiveText");
        AndroidPlugin.ShowSingleLineTextDialog("テキストを編集", "メッセージ付き", editText, 16, this.gameObject.name, "OnReceiveText");
#endif
    }


    //複数行のテキスト入力
    public void TestDialogTextMulitLine()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogTextMulitLine called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowMultiLineTextDialog("テキストを編集", editText, 0, 5, this.gameObject.name, "OnReceiveText");
        AndroidPlugin.ShowMultiLineTextDialog("テキストを編集", "メッセージ付き", editText, 0, 5, this.gameObject.name, "OnReceiveText");
#endif
    }


    //テキスト入力ダイアログのコールバックハンドラ
    private void OnReceiveText(string message)
    {
        editText = message.Trim();
        ShowToast(editText);
    }


    //数値入力のダイアログ
    public void TestDialogNumeric()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogNumeric called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowNumericTextDialog("数を入力", "符号なし整数のみ(6ケタまで)", 123, 6, false, false, this.gameObject.name, "ShowToast");
        //AndroidPlugin.ShowNumericTextDialog("符号あり小数を入力", -3.14f, 6, true, true, this.gameObject.name, "ShowToast");
#endif
    }


    //半角英数入力のダイアログ
    public void TestDialogAlphaNumeric()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogAlphaNumeric called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowAlphaNumericTextDialog("半角英数を入力", "記号なし", "", 8, "", this.gameObject.name, "ShowToast");
        AndroidPlugin.ShowAlphaNumericTextDialog("半角英数と記号（_-）を入力", "banana_ume-", 16, "_-", this.gameObject.name, "ShowToast");
#endif
    }


    //パスワード入力のダイアログ
    public void TestDialogPassword()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogPassword called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowPasswordTextDialog("パスワードを入力", "password", 16, false, this.gameObject.name, "ShowToast");
        //AndroidPlugin.ShowPasswordTextDialog("暗証番号を入力", "4ケタの数字", "", 4, true, this.gameObject.name, "ShowToast");
#endif
    }


    //日付選択ダイアログ
    public void TestDatePicker()
    {
#if UNITY_EDITOR
        Debug.Log("TestDatePicker called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowDatePickerDialog("", "yyyy/M/d", this.gameObject.name, "ShowToast");
#endif
    }


    //時刻選択ダイアログ
    public void TestTimePicker()
    {
#if UNITY_EDITOR
        Debug.Log("TestDatePicker called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowTimePickerDialog("", "H:mm", this.gameObject.name, "ShowToast");
#endif
    }


    //スイッチ選択ダイアログ用
    string[] switchItems = { "ユニティちゃん", "プロ生ちゃん", "クエリちゃん", "サファイアートちゃん", "ニコニ立体ちゃん", "東北ずん子", "おきゅたん" };
    string[] switchKeys = { "utc", "pronama", "query", "sapphiart", "alicia", "zunko", "ocutan" };
    bool[] switchChecks;                        //スイッチ状態の保存
    const string SWITCH_PREF = "_switches";     //PlayerPrefs 保存用

    //スイッチ選択ダイアログ
    public void TestDialogSwitch()
    {
        if (switchChecks == null)   //保存されているものがあれば読み込む（無ければ全オフで新規生成）
            switchChecks = XPlayerPrefs.GetArray(gameObject.name + SWITCH_PREF, new bool[switchItems.Length]);  //※配列は保存されている長さになるので注意

        XDebug.Clear(); //※ヒエラルキーにある「DebugConsole」をオンにすると内容が見れます。
        XDebug.Log("(PlayerPrefs or init)");
        XDebug.Log(switchChecks.Select(e => e.ToString()).Aggregate((s, a) => s + ", " + a), 3);

#if UNITY_EDITOR
        Debug.Log("TestDialogSwitch called");
#elif UNITY_ANDROID
        string[] keys = (switchKeyToggle != null && !switchKeyToggle.isOn) ? null : switchKeys;
        AndroidPlugin.ShowSwitchDialog("嫁のブレンド・Ｓ 設定", "ドＳのオン・オフができます。", switchItems, keys, switchChecks, 0, gameObject.name, "ReceiveSwitches", "これでいい", "やめる");
#endif
    }

    //スイッチ選択ダイアログのコールバックハンドラ
    private void ReceiveSwitches(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ReceiveSwitches : message = " + message);
#endif
        ShowToast(message);

        XDebug.Clear(); //※ヒエラルキーにある「DebugConsole」をオンにすると内容が見れます。
        XDebug.Log("(ReceiveSwitches : message)");
        XDebug.Log(message, 3);

        string str = "";
        string[] arr = message.Split('\n');
        for (int i = 0; i < arr.Length; i++)
        {
            switchChecks[i] = arr[i].EndsWith("true");
            if (switchChecks[i])
                str += switchItems[i] + "がドSになりました！\n";
        }

        XPlayerPrefs.SetArray(gameObject.name + SWITCH_PREF, switchChecks); //配列で保存
        StartCoroutine(DelayedToast(str.Trim(), 3));     //少し遅れてトースト表示
    }

}