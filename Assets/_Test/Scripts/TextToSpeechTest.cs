using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using FantomLib;

// Android のテキスト読み上げのデモ
// http://fantom1x.blog130.fc2.com/blog-entry-275.html
// 2018/02/03 Fantom (Unity 5.6.3p1)
// 17/10/28・新規リリース
// 17/11/03・複数行テキスト入力ダイアログでのテキスト編集サンプルを追加。
// 18/02/03・初期化メソッド名の変更による修正（InitTextToSpeech()）。
public class TextToSpeechTest : MonoBehaviour
{

    public GameObject receiveObject;
    public Text displayText;
    public Text statusText;
    public Animator statusAnimator;

    public Text speedText;
    public Text pitchText;
    public float speakPicthStep = 0.25f;    //テキスト読み上げ音程変化量
    public float speakSpeedStep = 0.25f;    //テキスト読み上げ速度変化量



    // Use this for initialization
    private void Start()
    {
        if (receiveObject == null)
            receiveObject = this.gameObject;

#if UNITY_EDITOR
        Debug.Log("InitTextToSpeech");
#elif UNITY_ANDROID
        AndroidPlugin.InitTextToSpeech(receiveObject.name, "OnStatus"); //起動ステータスの表示
#endif
    }

    // Update is called once per frame
    //private void Update () {

    //}



    //現在表示されているテキストの読み上げ（ボタン用）
    public void PlayTextToSpeech()
    {
        if (displayText != null && !string.IsNullOrEmpty(displayText.text))
            StartTextToSpeech(displayText.text);
    }


    //テキスト読み上げを開始
    public void StartTextToSpeech(string message)
    {
#if UNITY_EDITOR
        Debug.Log("StartTextToSpeech : message = " + message);
        if (!string.IsNullOrEmpty(message))
            StartCoroutine(DebugSimulate());
#elif UNITY_ANDROID
        AndroidPlugin.StartTextToSpeech(message, receiveObject.name, "OnStatus", "OnStart", "OnDone", "OnStop");
#endif
    }


    //テキスト読み上げのステータスコールバックハンドラ
    private void OnStatus(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStatus");
#endif
        if (statusText != null)
            statusText.text = message;

        if (displayText != null)
        {
            if (message.StartsWith("SUCCESS_INIT"))
                displayText.text += "\nテキスト読み上げが利用できます。";
            else if (message.StartsWith("ERROR_LOCALE_NOT_AVAILABLE"))
                displayText.text += "\nテキスト読み上げの初期化に失敗しました。利用できない言語です。";
            else if (message.StartsWith("ERROR_INIT"))
                displayText.text += "\nテキスト読み上げの初期化に失敗しました。";
        }
    }

    //読み上げ開始コールバックハンドラ
    private void OnStart(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStart");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("blink");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "発声中";
        }
    }

    //読み上げ終了コールバックハンドラ
    private void OnDone(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnDone");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "発声終了";
        }
    }

    //読み上げ中断コールバックハンドラ
    private void OnStop(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStop");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "発声中断" + (message.StartsWith("INTERRUPTED") ? "(interrupted)" : "");
        }
    }


    //テキスト読み上げを中断
    public void StopTextToSpeech()
    {
#if UNITY_EDITOR
        Debug.Log("StopTextToSpeech called");
#elif UNITY_ANDROID
        AndroidPlugin.StopTextToSpeech();
#endif
    }


#if UNITY_EDITOR
    //デバッグ用（自動でイベントを発生させてるだけ）
    private IEnumerator DebugSimulate()
    {
        OnStart("onStart");
        yield return new WaitForSeconds(3f);

        OnDone("onDone");
    }
#endif


    //テキスト読み上げの速度を上げる
    public void SpeakSpeedUp()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedUp called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.AddTextToSpeechSpeed(speakSpeedStep));
#endif
    }


    //テキスト読み上げの速度を下げる
    public void SpeakSpeedDown()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedDown called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.AddTextToSpeechSpeed(-speakSpeedStep));
#endif
    }


    //テキスト読み上げの速度を元に戻す
    public void SpeakSpeedReset()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedReset called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.ResetTextToSpeechSpeed());
#endif
    }


    //テキスト読み上げの音程を上げる
    public void SpeakPitchUp()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchUp called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.AddTextToSpeechPitch(speakPicthStep));
#endif
    }


    //テキスト読み上げの音程を下げる
    public void SpeakPitchDown()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchDown called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.AddTextToSpeechPitch(-speakPicthStep));
#endif
    }


    //テキスト読み上げの音程を元に戻す
    public void SpeakPitchReset()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchReset called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.ResetTextToSpeechPitch());
#endif
    }



    //速度の表示
    private void SetSpeedText(float speed)
    {
        if (speedText != null)
            speedText.text = string.Format("Speed : {0:F2}", speed);
    }

    //音程の表示
    private void SetPitchText(float pitch)
    {
        if (pitchText != null)
            pitchText.text = string.Format("Pitch : {0:F2}", pitch);
    }



    //テキスト編集ダイアログの呼び出し
    public void EditText()
    {
        if (displayText != null)
        {
#if UNITY_EDITOR
            Debug.Log("EditText called");
#elif UNITY_ANDROID
            AndroidPlugin.ShowMultiLineTextDialog("テキストを編集", displayText.text, 0, 9, receiveObject.name, "OnEditText");
#endif
        }
    }

    //テキスト編集ダイアログのコールバックハンドラ
    private void OnEditText(string message)
    {
        if (displayText != null)
        {
            displayText.text = message.Trim();
        }
    }

}
