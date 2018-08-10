using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FantomLib
{
    /// <summary>
    /// ピンチ操作を取得してコールバックする
    /// 2018/01/09 Fantom (Unity 5.6.3p1)
    /// http://fantom1x.blog130.fc2.com/blog-entry-288.html
    ///（使い方）
    ///・適当な GameObject にアタッチして、インスペクタから OnPinchStart, OnPinch にコールバックする関数を登録すれば使用可。
    ///・またはプロパティ IsPinching, Width, Delta, Ratio をフレーム毎監視しても良い（こちらの場合は使用してない状態（IsPinching=false, Width=0, Delta=0, Ratio=1）も含まれる）。
    ///（仕様説明）
    ///・内部的には画面でタッチされた2本の指の間隔をピクセル単位で取得する。ただし戻り値は画面幅で割った正規化された値とピクセルそのもので返すかを選べる（isNormalized）。
    ///・ピンチの操作は1本→2本となったときのみ認識する。3本以上→2本になったときは無効。
    ///・タッチデバイスを UNITY_ANDROID, UNITY_IOS としているので、他のデバイスも加えたい場合は #if の条件文にデバイスを追加する（Input.touchCount が取得できるもののみ）。
    /// </summary>
    public class PinchInput : MonoBehaviour
    {
        public bool isNormalized = true;        //画面幅（or 高さ）で正規化した値でコールバックする（false=ピクセル単位で返す）
        public bool widthReference = true;      //isNormalized=true のとき、画面幅（Screen.width）を基準にする（false=高さ（Screen.height）を基準）[単位が px/Screen.width のようになる]

        //認識する画面上の領域（0.0～1.0）[(0,0):画面左下, (1,1):画面右上]
        public Rect validArea = new Rect(0, 0, 1, 1);

        //ピンチ検出プロパティ（フレーム毎取得用）
        //・ピンチ操作中フラグ（指2本のみ。3～は無効）。
        public bool IsPinching {
            get; private set;
        }

        //ピンチ幅(距離) プロパティ（フレーム毎取得用）
        //・isNormalized=true のときは画面幅で正規化した値で、false のときは px 単位になる。
        public float Width {
            get; private set;
        }

        //ピンチ幅(距離)の直前との差分 プロパティ（フレーム毎取得用）
        //・isNormalized=true のときは画面幅で正規化した値で、false のときは px 単位になる。
        //・線形的な相対量のようになる（相対移動操作などに良い）。
        public float Delta {
            get; private set;
        }

        //ピンチ幅(距離)の変化比 プロパティ（フレーム毎取得用）
        //・ピンチ開始時の幅(距離)を1とし、現在の幅の比を返す（指を開く→ 1.0以上（1,2,3,...倍[小数含む]/指を閉じる→ 1.0より下(1/2, 1/3, 1/4,...倍[負にはならない])）
        //・物理的に指を開くより指を閉じる方が変化しやすいので注意（スケール操作などに良い）。
        public float Ratio {
            get; private set;
        }


        //ピンチ開始コールバック
        [Serializable]
        public class PinchStartHandler : UnityEvent<float, Vector2> { } //Width, center（２指間の中心座標）が返る
        public PinchStartHandler OnPinchStart;

        //ピンチ中コールバック（伸縮率とその差分）
        [Serializable]
        public class PinchHandler : UnityEvent<float, float, float> { } //Width, Delta, Ratio が返る
        public PinchHandler OnPinch;


        //Local Values
        float startDistance;            //ピンチ開始時の指の距離（px）
        float oldDistance;              //直前の伸縮距離（px）


        //アクティブになったら、初期化する（アプリの中断などしたときはリセットする）
        void OnEnable()
        {
            IsPinching = false;
        }

        // Update is called once per frame
        void Update()
        {
            //プロパティはフレーム毎にリセット
            Width = 0; Delta = 0; Ratio = 1;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)   //タッチで取得したいプラットフォームのみ
            if (Input.touchCount == 2) //ピンチでの操作（2本指のみ）
            {
                //※fingerId と touches[] のインデクスは必ずしも一致しないらしいので fingerId=1 となっている方を取得（指1本→2本になったとき可能とするため）
                Touch touch = (Input.touches[1].fingerId == 1) ? Input.touches[1] : Input.touches[0];
                if (!IsPinching && touch.phase == TouchPhase.Began)   //新しく認識したときのみ
                {
                    //認識する画面上の領域内か？（2本の指の中心の座標を基準にする）
                    Vector2 center = (Input.touches[0].position + Input.touches[1].position) / 2;
                    if (validArea.xMin * Screen.width <= center.x && center.x <= validArea.xMax * Screen.width && 
                        validArea.yMin * Screen.height <= center.y && center.y <= validArea.yMax * Screen.height)
                    {
                        IsPinching = true;      //ピンチ開始

                        //fingerId=0～1 のみ（必ず最初と2本目の指）。指3本→2本（0-2 など）は不可とする。
                        Width = startDistance = oldDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        if (isNormalized)
                        {
                            float unit = widthReference ? Screen.width : Screen.height;
                            Width /= unit;      //画面幅で正規化すれば、解像度に依存しなくなる
                            center /= unit;
                        }

                        if (OnPinchStart != null)
                            OnPinchStart.Invoke(Width, center); //開始時は必ず Delta=0, Ratio=1 となる
                    }
                }
                else if (IsPinching)  //既に認識されているときのみ：3本→2本になったときは無効になる
                {
                    float endDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                    Width = endDistance;
                    Delta = endDistance - oldDistance;      //直前との差分
                    Ratio = endDistance / startDistance;    //開始時のピンチ幅(px距離)を基準にした倍率になる
                    oldDistance = endDistance;

                    if (isNormalized)
                    {
                        float unit = widthReference ? Screen.width : Screen.height;
                        Width /= unit;      //画面幅で正規化すれば、解像度に依存しなくなる
                        Delta /= unit;
                    }

                    if (OnPinch != null)
                        OnPinch.Invoke(Width, Delta, Ratio);
                }
            }
            else  //タッチが2つでないときは全て無効にする
#endif
            {
                IsPinching = false;
            }
        }
    }
}
