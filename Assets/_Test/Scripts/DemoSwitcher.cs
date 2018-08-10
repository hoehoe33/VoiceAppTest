using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// ピンチやドラッグの機能切り替え
// 2018/01/09 Fantom (Unity 5.6.3p1)
public class DemoSwitcher : MonoBehaviour
{

    public Toggle scaleToggle;
    public PinchToScale pinchToScale;
    public SmoothFollow3 smoothFollow;
    public Toggle dragToggle;


    // Use this for initialization
    private void Start()
    {
        if (dragToggle != null)
            OnDraggable(dragToggle.isOn);
    }

    // Update is called once per frame
    //private void Update () {

    //}

    //width: ピンチ幅, center: ピンチの2本指の中心の座標
    //http://fantom1x.blog130.fc2.com/blog-entry-288.html
    public void OnPinchStart(float width, Vector2 center)
    {
        if (scaleToggle != null && scaleToggle.isOn && pinchToScale != null)
            pinchToScale.OnPinchStart(width, center);
    }

    //width: ピンチ幅, delta: 直前のピンチ幅の差, ratio: ピンチ幅の開始時からの伸縮比(1:ピンチ開始時, 1以上拡大, 1より下(1/2,1/3,...)縮小)
    //http://fantom1x.blog130.fc2.com/blog-entry-288.html
    public void OnPinch(float width, float delta, float ratio)
    {
        if (scaleToggle != null && scaleToggle.isOn && pinchToScale != null)
            pinchToScale.OnPinch(width, delta, ratio);
        else if (smoothFollow != null)
            smoothFollow.OnPinch(width, delta, ratio);
    }

    public void OnReset()
    {
        if (smoothFollow != null)
            smoothFollow.ResetOperations();

        if (pinchToScale != null)
            pinchToScale.ResetScale();
    }

    public void OnDraggable(bool enable)
    {
        if (smoothFollow != null)
        {
            smoothFollow.angleOperation.dragEnable = enable;
            smoothFollow.heightOperation.dragEnable = enable;
        }
    }
}

