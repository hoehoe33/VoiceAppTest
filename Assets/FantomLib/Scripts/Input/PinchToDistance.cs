using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantomLib
{
    /// <summary>
    /// ピンチで距離を操作する
    /// 2018/01/09 Fantom (Unity 5.6.3p1)
    /// http://fantom1x.blog130.fc2.com/blog-entry-288.html
    ///（使い方）
    ///・カメラなどの GameObject にアタッチして、インスペクタから PinchInput のコールバックを登録すれば使用可。
    ///・距離は target からの直線距離となる。
    /// </summary>
    public class PinchToDistance : MonoBehaviour
    {
        public Transform target;            //視点となるオブジェクト
        public float speed = 2f;            //変化速度
        public float minDistance = 1.0f;    //近づける最小距離
        public bool lookAt = true;          //オブジェクトの方を向く

        //LocalValues
        float initDistance;                 //起動初期距離（リセット用）


        // Use this for initialization
        private void Start()
        {
            if (target != null)
            {
                Vector3 dir = target.position - transform.position;
                initDistance = dir.magnitude;
                if (lookAt)
                    transform.LookAt(target.position);
            }
        }

        // Update is called once per frame
        //private void Update()
        //{

        //}


        //width: ピンチ幅, center: ピンチの2本指の中心の座標
        public void OnPinchStart(float width, Vector2 center)
        {
        }

        //width: ピンチ幅, delta: 直前のピンチ幅の差, ratio: ピンチ幅の開始時からの伸縮比(1:ピンチ開始時, 1以上拡大, 1より下(1/2,1/3,...)縮小)
        public void OnPinch(float width, float delta, float ratio)
        {
            if (target == null)
                return;

            Vector3 dir = target.position - transform.position;
            float distance = Math.Max(minDistance, dir.magnitude - delta * speed);
            Vector3 pos = target.position - dir.normalized * distance;
            transform.position = pos;
            if (lookAt)
                transform.LookAt(target.position);
        }

        //初期の距離に戻す
        public void ResetDistance()
        {
            if (target == null)
                return;

            Vector3 dir = target.position - transform.position;
            Vector3 pos = target.position - dir.normalized * initDistance;
            transform.position = pos;
            if (lookAt)
                transform.LookAt(target.position);
        }
    }
}