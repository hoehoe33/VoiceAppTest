using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FantomLib
{
    /// <summary>
    /// 長押しを取得してコールバックする（UI上での判定に向いている。EventSystem と Graphics Raycaster が必要）
    /// 2018/01/09 Fantom (Unity 5.6.3p1)
    /// http://fantom1x.blog130.fc2.com/blog-entry-251.html
    ///（使い方）
    ///・Image や Text, Button などの UI を持つ GameObject にアタッチして、インスペクタから OnLongClick（引数なし）にコールバックする関数を登録すれば使用可。
    ///・シーンに EventSystem、(ルート)Canvas に Graphics Raycaster がアタッチされている必要がある。
    ///（仕様説明）
    ///・EventSystem からのイベント（OnPointerDown, OnPointerUp, OnPointerExit）を取得し、一定時間（Valid Time）押下され続けていたら長押しと認識する。
    ///・途中で有効領域外（UIから外れる）へ出たり、指を離したりしたときは無効。
    ///・はじめの指のみ認識（複数指の場合、ピンチの可能性があるため無効とする）。
    ///※スマホだとUIを透過にしていると、上手く認識できないようなので注意（可視できる画像等ならOK）。
    ///（更新履歴）
    /// 17/07/06・新規リリース。
    /// 18/01/09・進捗コールバックを追加。
    /// </summary>
    public class LongClickEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float validTime = 1.0f;      //長押しとして認識する時間（これより長い時間で長押しとして認識する）

        //Local Values
        float requiredTime;                 //長押し認識時刻（この時刻を超えたら長押しとして認識する）
        bool pressing = false;              //押下中フラグ（単一指のみの取得としても利用）

        //長押しイベントコールバック
        public UnityEvent OnLongClick;

        //長押し・進捗開始のイベントコールバック
        public UnityEvent OnStart;

        //進捗のイベントコールバック
        [Serializable] public class ProgressHandler : UnityEvent<float> { } //進捗 0～1f
        public ProgressHandler OnProgress;

        //進捗中断のイベントコールバック
        public UnityEvent OnCancel;


        // Update is called once per frame
        void Update()
        {
            if (pressing)  //はじめに押した指のみとなる
            {
                if (requiredTime < Time.time)   //一定時間過ぎたら認識
                {
                    if (OnLongClick != null)
                        OnLongClick.Invoke();   //UnityEvent

                    pressing = false;           //長押し完了したら無効にする
                }
                else
                {
                    if (OnProgress != null)
                    {
                        float amount = Mathf.Clamp01(1f - (requiredTime - Time.time) / validTime);  //0～1f
                        OnProgress.Invoke(amount);
                    }
                }
            }
        }

        //UI領域内で押下
        public void OnPointerDown(PointerEventData data)
        {
            if (!pressing)          //ユニークにするため
            {
                pressing = true;
                requiredTime = Time.time + validTime;

                if (OnStart != null)
                    OnStart.Invoke();   //UnityEvent
            }
            else
            {
                pressing = false;   //２本以上の指の場合、ピンチの可能性があるため無効にする
            }
        }

        //※スマホだとUIを透過にしていると、指を少し動かしただけでも反応してしまうので注意
        public void OnPointerUp(PointerEventData data)
        {
            if (pressing)           //はじめに押した指のみとなる
            {
                if (OnCancel != null)
                    OnCancel.Invoke();   //UnityEvent

                pressing = false;
            }
        }

        //UI領域から外れたら無効にする
        public void OnPointerExit(PointerEventData data)
        {
            if (pressing)           //はじめに押した指のみとなる
            {
                if (OnCancel != null)
                    OnCancel.Invoke();   //UnityEvent

                pressing = false;   //領域から外れたら無効にする
            }
        }
    }

}