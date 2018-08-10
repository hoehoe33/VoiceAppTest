using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using FantomLib;

// Android の音声認識のデモ
// 2018/02/03 Fantom (Unity 5.6.3p1)
// http://fantom1x.blog130.fc2.com/blog-entry-273.html
//（更新履歴）
// 17/10/01・新規リリース
// 17/10/28・音声認識の戻値の最後の改行を削除するように変更（※プラグイン内部処理）。
//         ・選択肢ダイアログのサンプルを追加（音声認識→Webサーチ時にも呼び出される）。
// 17/11/06・選択肢ダイアログ（ShowSelectDialog(), ShowSingleChoiceDialog(), ShowMultiChoiceDialog()）に対応する結果配列の要素を返すオーバーロードのサンプルを追加（OnResult() ※ただし、コメントアウトされている）。
// 18/02/03・サポートチェック済みフラグ（speechChecked）が無効になっていた不具合を修正。
public class SpeechRecognizerTest : MonoBehaviour {

    public GameObject receiveObject;
    public Button speechButton;
    public Animator circleAnimator;
    public Animator voiceAnimator;
    public Text displayText;
    public Toggle webSearchToggle;



    //音声認識のサポートプロパティ
    private bool mIsSpeechSupport = false;  //キャッシュ
    private bool speechChecked = false;     //チェック済み

    public bool isSpeechSupport {
        get {
            if (!speechChecked)
            {
#if UNITY_EDITOR
                mIsSpeechSupport = true;    //デバッグ用
#elif UNITY_ANDROID
                mIsSpeechSupport = AndroidPlugin.IsSupportedSpeechRecognizer();
#endif
                speechChecked = true;
            }
            return mIsSpeechSupport;
        }
    }



    // Use this for initialization
    private void Start () {
        if (receiveObject == null)
            receiveObject = this.gameObject;

        if (displayText != null)
            displayText.text = "isSpeechSupport = " + isSpeechSupport;
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
    //private void Update () {

    //}


    //==========================================================
    //ダイアログありの例

    //ダイアログありの音声認識
    public void ShowSpeechRecognizer()
    {
#if UNITY_EDITOR
        Debug.Log("ShowSpeechRecognizer called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowSpeechRecognizer(receiveObject.name, "ResultSpeechRecognizer", "じゃーんーけーん");
#endif
        if (displayText != null)
            displayText.text = "";
    }


    //ダイアログありの音声認識からの結果受信
    private void ResultSpeechRecognizer(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ResultSpeechRecognizer called");
#endif
        SetDisplayText(message);

        string[] keywords = message.Split('\n');

        if (webSearchToggle != null && webSearchToggle.isOn)
        {
#if UNITY_EDITOR
            StartWebSearch(keywords[0]);    //最初の１つ
#elif UNITY_ANDROID
            if (keywords.Length > 1)
                AndroidPlugin.ShowSelectDialog("検索する単語を選択", keywords, receiveObject.name, "StartWebSearch");
                //AndroidPlugin.ShowSingleChoiceDialog("検索する単語を選択", keywords, 0, receiveObject.name, "StartWebSearch");
            else
                StartWebSearch(keywords[0]);    //１つしかないとき
#endif
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (keywords.Length > 1)
                AndroidPlugin.ShowSelectDialog("単語を選択", keywords, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSingleChoiceDialog("単語を選択", keywords, 0, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowMultiChoiceDialog("単語を選択", keywords, null, receiveObject.name, "SetDisplayText");
            else
                SetDisplayText(keywords[0]);    //１つしかないとき
#endif
        }
    }


    //==========================================================
    //ダイアログなしの例

    //ダイアログなしの音声認識
    public void StartSpeechRecognizer()
    {
        if (displayText != null)
            displayText.text = "音声認識を起動してます…";

#if UNITY_EDITOR
        Debug.Log("StartSpeechRecognizer");
        StartCoroutine(DebugSimulate());
#elif UNITY_ANDROID
        AndroidPlugin.StartSpeechRecognizer(receiveObject.name, "OnResult", "OnError", "OnReady", "OnBegin");
#endif
        if (speechButton != null)
            speechButton.interactable = false;
    }


    //ダイアログなしの音声認識のマイク待機状態
    private void OnReady(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnReady");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("ready");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("ready");

        if (displayText != null)
            displayText.text = "音声を待機中…";
    }


    //ダイアログなしの音声認識のマイク取得状態
    private void OnBegin(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnBegin");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("speech");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("speech");

        if (displayText != null)
            displayText.text = "音声を取得しています…";
    }


    //ダイアログなしの音声認識の結果受信（成功時）
    private void OnResult(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnResult");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        SetDisplayText(message);

        string[] keywords = message.Split('\n');

        if (webSearchToggle != null && webSearchToggle.isOn)
        {
#if UNITY_EDITOR
            StartWebSearch(keywords[0]);    //最初の１つ
#elif UNITY_ANDROID
            if (keywords.Length > 1)
                //AndroidPlugin.ShowSelectDialog("検索する単語を選択", keywords, receiveObject.name, "StartWebSearch");
                AndroidPlugin.ShowSingleChoiceDialog("検索する単語を選択", keywords, 0, receiveObject.name, "StartWebSearch");
            else
                StartWebSearch(keywords[0]);    //１つしかないとき
#endif
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (keywords.Length > 1)
                //AndroidPlugin.ShowSelectDialog("単語を選択", keywords, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSingleChoiceDialog("単語を選択", keywords, 0, receiveObject.name, "SetDisplayText");
                AndroidPlugin.ShowMultiChoiceDialog("単語を選択", keywords, null, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSelectDialog("単語を選択", keywords, receiveObject.name, "SetDisplayText", keywords.Select((e, i) => i + ":" + e).ToArray());
                //AndroidPlugin.ShowSingleChoiceDialog("単語を選択", keywords, 0, receiveObject.name, "SetDisplayText", keywords.Select((e, i) => i + ":" + e).ToArray());
                //AndroidPlugin.ShowMultiChoiceDialog("単語を選択", keywords, null, receiveObject.name, "SetDisplayText", keywords.Select((e,i)=>i+":"+e).ToArray());
            else
                SetDisplayText(keywords[0]);    //１つしかないとき
#endif
        }
    }


    //ダイアログなしの音声認識のエラー受信（失敗時）
    private void OnError(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnError");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        if (displayText != null)
            displayText.text = message;
    }


    //音声認識を中断
    public void StopSpeechRecognizer()
    {
#if UNITY_EDITOR
        Debug.Log("StopSpeechRecognizer");
#elif UNITY_ANDROID
        AndroidPlugin.ReleaseSpeechRecognizer();
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        if (displayText != null)
            displayText.text = "音声認識はキャンセルされました。";
    }


#if UNITY_EDITOR
    //デバッグ用（自動でイベントを発生させてるだけ。ランダムでエラーも表示される）
    private IEnumerator DebugSimulate()
    {
        OnReady("onReadyForSpeech");
        yield return new WaitForSeconds(2f);

        OnBegin("onBeginningOfSpeech");
        yield return new WaitForSeconds(5f);

        if (Random.Range(0, 10) == 0)   //エラー発生率
            OnError("ERROR_NO_MATCH");
        else
            OnResult("テスト\nてすと\ntest");
    }
#endif


    //単語をWeb検索する
    public void StartWebSearch(string query)
    {
#if UNITY_EDITOR
        Debug.Log("StartWebSearch : query = " + query);
#elif UNITY_ANDROID
        AndroidPlugin.StartWebSearch(query);    //Web検索
        //AndroidPlugin.StartAction("android.intent.action.WEB_SEARCH", "query", query);    //Web検索（※同じ）
#endif
    }


    //テキスト表示（入れ替え）
    public void SetDisplayText(string message)
    {
        if (displayText != null)
            displayText.text = message;
    }

}
