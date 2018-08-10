using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FantomLib
{
    /// <summary>
    /// ハードウェア音量操作クラス（AndroidPlugin を使用する）
    /// http://fantom1x.blog130.fc2.com/blog-entry-283.html
    /// 2017/12/03 Fantom (Unity 5.6.3p1)
    ///※メディア音量のみ
    ///※ハードウェアキーのイベントを取得するには「AndroidManifest.xml」で「HardVolKeyOnUnityPlayerActivity」または「FullPluginOnUnityPlayerActivity」を使用する必要あり。
    ///（更新履歴）
    /// 17/10/01・新規リリース
    /// 17/12/03・端末での音量操作可否のプロパティ（HardOperation）を追加。
    ///         ・volume プロパティに set を追加。
    /// </summary>
    public class HardVolumeController : MonoBehaviour
    {

        public bool enableHardKey = false;      //ハードウェア音量キーでこのスクリプトを使用する（true=ハードウェアキーのイベントを受信して利用/false=イベントを無視する）

        [SerializeField] private bool hardOperation = true; //端末自身での音量操作可否（true=端末での音量操作可/false=端末での音量操作を無効化[=Unity側だけで操作可]）（※実行時は HardOperation プロパティで切り替える）

        public bool showUI = true;              //Unity から操作したとき、UIを表示する（※エディタ上では無視）



        //ハードウェア音量が操作されたときのコールバック
        //・ハードウェアキーの操作、またはこのクラスの音量操作メソッドが呼ばれたときコールバックする（→ UI表示などに利用する）。
        //引数：int は操作後の音量
        [Serializable]
        public class VolumeCalledHandler : UnityEvent<int> { }
        public VolumeCalledHandler OnVolumeCalled;

        //ハードウェアキーで音量を上げる操作されたときのコールバック
        public UnityEvent OnHardVolumeKeyUp;

        //ハードウェアキーで音量を下げる操作されたときのコールバック
        public UnityEvent OnHardVolumeKeyDown;



#if UNITY_EDITOR
        //デバッグ用
        public int debugVolume = 2;                                 //デバッグ用 現在音量
        public bool debugIncreasement = true;                       //デバッグ用 ↓テストキーを押したとき debugVolume を増減させる
        public KeyCode debugVolumeUpKey = KeyCode.KeypadPlus;       //デバッグ用 ハードウェアキー音量上げるシュミレート
        public KeyCode debugVolumeDownKey = KeyCode.KeypadMinus;    //デバッグ用 ハードウェアキー音量下げるシュミレート
#endif



        //最大音量の取得プロパティ
        private int mMaxVolume = -1;    //キャッシュ（通常は変わらないので取得は１回にする）

        public int maxVolume {
            get {
                if (mMaxVolume < 0)
                {
#if UNITY_EDITOR
                    mMaxVolume = 15;    //デバッグ用値
#elif UNITY_ANDROID
                    mMaxVolume = AndroidPlugin.GetMediaMaxVolume();
#endif
                }
                return mMaxVolume;
            }
        }


        //現在の音量の取得プロパティ
        public int volume {
            get {
                int vol = -1;
#if UNITY_EDITOR
                vol = debugVolume;    //デバッグ用値
#elif UNITY_ANDROID
                vol = AndroidPlugin.GetMediaVolume();
#endif
                return vol;
            }
            set {
                int vol = Mathf.Clamp(value, 0, maxVolume);
#if UNITY_ANDROID && !UNITY_EDITOR
                vol = AndroidPlugin.SetMediaVolume(vol, showUI);
#endif
#if UNITY_EDITOR
                debugVolume = vol;    //デバッグ用値
#endif
                if (OnVolumeCalled != null)
                    OnVolumeCalled.Invoke(vol);
            }
        }


        //ハードウェアボタン押下による、端末自身での音量操作を有効/無効化する
        public bool HardOperation {
            get { return hardOperation; }
            set {
                hardOperation = value;
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidPlugin.HardKey.SetVolumeOperation(value);
#endif
            }
        }



        protected void OnEnable()
        {
#if UNITY_EDITOR
            Debug.Log("HardKey Listener registered.");
#elif UNITY_ANDROID
            //ハードウェア音量キーのリスナー登録
            AndroidPlugin.HardKey.SetKeyVolumeUpListener(this.gameObject.name, "HardVolumeKeyChange", "VolumeUp");
            AndroidPlugin.HardKey.SetKeyVolumeDownListener(this.gameObject.name, "HardVolumeKeyChange", "VolumeDown");
#endif
            HardOperation = hardOperation;    //インスペクタの設定を反映
        }


        protected void OnDisable()
        {
#if UNITY_EDITOR
            Debug.Log("HardKey Listener removed.");
#elif UNITY_ANDROID
            //ハードウェア音量キーの全リスナー解除
            AndroidPlugin.HardKey.RemoveAllListeners();
#endif
        }


        // Use this for initialization
        protected void Start()
        {
        }


        // Update is called once per frame
        protected void Update()
        {
#if UNITY_EDITOR
            //デバッグ用のキー操作（※エディタのみ）
            if (Input.GetKeyDown(debugVolumeUpKey))
            {
                if (debugIncreasement)
                    debugVolume = Mathf.Clamp(++debugVolume, 0, maxVolume);

                if (OnHardVolumeKeyUp != null)
                    OnHardVolumeKeyUp.Invoke();
            }
            else if (Input.GetKeyDown(debugVolumeDownKey))
            {
                if (debugIncreasement)
                    debugVolume = Mathf.Clamp(--debugVolume, 0, maxVolume);

                if (OnHardVolumeKeyDown != null)
                    OnHardVolumeKeyDown.Invoke();
            }
#endif
        }


        //ハードウェア音量キーのイベントハンドラ（Android ネイティブから呼ばれる）
        protected void HardVolumeKeyChange(string message)
        {
            if (enableHardKey)
            {
                if (message == "VolumeUp")
                {
                    //VolumeNow()[通常アプリ] or VolumeUp() [VRアプリ] を登録する等
                    if (OnHardVolumeKeyUp != null)
                        OnHardVolumeKeyUp.Invoke();
                }
                else if (message == "VolumeDown")
                {
                    //VolumeNow()[通常アプリ] or VolumeDown() [VRアプリ] を登録する等
                    if (OnHardVolumeKeyDown != null)
                        OnHardVolumeKeyDown.Invoke();
                }
            }
        }


        //音量を上げる（maxVolumeまで）
        public void VolumeUp()
        {
            int vol = -1;
#if UNITY_EDITOR
            vol = Mathf.Clamp(++debugVolume, 0, maxVolume);    //デバッグ用値
#elif UNITY_ANDROID
            vol = AndroidPlugin.AddMediaVolume(1, showUI);  //音量+１
#endif
            if (OnVolumeCalled != null)
                OnVolumeCalled.Invoke(vol);
        }


        //音量を下げる（0 まで）
        public void VolumeDown()
        {
            int vol = -1;
#if UNITY_EDITOR
            vol = Mathf.Clamp(--debugVolume, 0, maxVolume);    //デバッグ用値
#elif UNITY_ANDROID
            vol = AndroidPlugin.AddMediaVolume(-1, showUI);  //音量-1
#endif
            if (OnVolumeCalled != null)
                OnVolumeCalled.Invoke(vol);
        }


        //音量をミュートする（0 にする）
        public void VolumeMute()
        {
            int vol = -1;
#if UNITY_EDITOR
            vol = debugVolume = 0;    //デバッグ用値
#elif UNITY_ANDROID
            vol = AndroidPlugin.SetMediaVolume(0, showUI);  //音量=0
#endif
            if (OnVolumeCalled != null)
                OnVolumeCalled.Invoke(vol);
        }


        //現在の音量を取得
        public void VolumeNow()
        {
            int vol = volume;   //プロパティから

            if (OnVolumeCalled != null)
                OnVolumeCalled.Invoke(vol);
        }

    }

}
