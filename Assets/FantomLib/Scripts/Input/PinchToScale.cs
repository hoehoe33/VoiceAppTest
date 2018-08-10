using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantomLib
{
    /// <summary>
    /// ピンチでスケールを変化させる（ローカルスケール）
    /// 2018/01/09 Fantom (Unity 5.6.3p1)
    /// http://fantom1x.blog130.fc2.com/blog-entry-288.html
    ///（使い方）
    ///・伸縮したい GameObject にアタッチして、インスペクタから PinchInput のコールバックを登録すれば使用可。
    /// </summary>
    public class PinchToScale : MonoBehaviour
    {
        public Transform target;    //スケール変化させるオブジェクト

        //Local Values
        Vector3 startScale;         //ピンチ開始時スケール
        Vector3 initScale;          //起動初期スケール（リセット用）


        // Use this for initialization
        private void Start()
        {
            if (target == null)
                target = gameObject.transform;  //指定がないときは自身を対象とする

            initScale = target.localScale;
        }

        // Update is called once per frame
        //private void Update () {

        //}


        //width: ピンチ幅, center: ピンチの2本指の中心の座標
        public void OnPinchStart(float width, Vector2 center)
        {
            if (target != null)
                startScale = target.localScale;
        }

        //width: ピンチ幅, delta: 直前のピンチ幅の差, ratio: ピンチ幅の開始時からの伸縮比(1:ピンチ開始時, 1以上拡大, 1より下(1/2,1/3,...)縮小)
        public void OnPinch(float width, float delta, float ratio)
        {
            if (target != null)
                target.localScale = startScale * ratio;
        }

        //スケールを元に戻す
        public void ResetScale()
        {
            if (target != null)
                target.localScale = initScale;
        }
    }
}