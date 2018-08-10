using System;
using UnityEngine;

namespace FantomLib
{
    /// <summary>
    /// SmoothFollow に左右回転アングルと高さと距離の遠近機能を追加したもの ver.3
    /// 2018/01/09 Fantom (Unity 5.6.3p1)
    /// http://fantom1x.blog130.fc2.com/blog-entry-289.html
    /// （SmoothFollow2 からの変更点）
    /// http://fantom1x.blog130.fc2.com/blog-entry-163.html
    ///・SwipeInput のコールバックでのスワイプで一定角度の旋回を追加。
    ///・PinchInput のコールバックでのピンチで距離の操作を追加（モバイル用）。
    ///・起動時に設定された対象（target）から、距離（distance）、高さ（height）、角度（preAngle）を算出するオプションを追加。
    ///・初期状態へのリセットメソッド（ResetOperations()）を追加。
    ///・ドラッグの認識する画面上の領域（validArea）を追加。
    ///・各設定をクラスで分けたので、変数名が変更された（機能は全て同じ）。
    ///（使い方）
    ///・カメラなどの GameObject にアタッチして、インスペクタから target に視点となるオブジェクトを登録すれば使用可。
    ///（仕様説明）
    ///・画面全体を(0,0)-(1,1)とし、有効領域内（Valid Area）でタッチまたはマウスでクリックしたとき認識する。
    ///・タッチ操作は指１本のみ（かつ最初の１本）の操作が有効となる（2本以上→１本になったときは認識しない）。
    ///・指でのドラッグとスワイプ操作を分けるため、AngleOperation.dragWidthLimit の値（画面幅による比率）より大きいときは（=指を素早く動かしたときは）ドラッグとして認識しない
    /// （スワイプは SwipeInput.validWidth の値で認識）。
    ///・タッチデバイスを UNITY_ANDROID, UNITY_IOS としているので、他のデバイスも加えたい場合は #if の条件文にデバイスを追加する（Input.touchCount が取得できるもののみ）。
    /// </summary>
    public class SmoothFollow3 : MonoBehaviour
    {
        public Transform target;                    //追従するオブジェクト

        public bool autoInitOnPlay = true;          //distance, height, preAngle を起動時に target 位置から自動算出する
        public float distance = 2.0f;               //XZ平面の距離
        public float height = 0f;                   //Y軸の高さ
        public float preAngle = 0f;                 //カメラアングル初期値

        public bool widthReference = true;          //画面幅（Screen.width）サイズを比率の基準にする（false=高さ（Screen.height）を基準）

        //認識する画面上の領域
        public Rect validArea = new Rect(0, 0, 1, 1);   //認識する画面領域（0.0～1.0）[(0,0):画面左下, (1,1):画面右上]


        //回転操作
        [Serializable]
        public class AngleOperation
        {
            public float damping = 3.0f;            //左右回転のスムーズ移動速度

            //キー入力
            public bool keyEnable = true;           //回転のキー操作の ON/OFF 
            public float keySpeed = 45f;            //左右回転速度
            public KeyCode keyLeft = KeyCode.Z;     //左回転キー
            public KeyCode keyRight = KeyCode.X;    //右回転キー

            //ドラッグ
            public bool dragEnable = true;          //回転のドラッグ操作の ON/OFF 
            public float dragSpeed = 10f;           //ドラッグ操作での回転速度
            public float dragWidthLimit = 0.1f;     //ドラッグとして認識できる幅（0 のとき制限なし ～ 1 のとき画面幅）。この幅以上は認識しない（スワイプと区別するため）。
        }
        public AngleOperation angleOperation;


        //旋回（一定角度回転）
        [Serializable]
        public class TurnOperation
        {
            public float angle = 90f;                       //旋回の角度

            //キー入力
            public bool keyEnable = true;                   //旋回キーの ON/OFF 
            public KeyCode keyLeft = KeyCode.KeypadMinus;   //左旋回キー
            public KeyCode keyRight = KeyCode.KeypadPlus;   //右旋回キー

            //スワイプ
            public bool swipeEnable = true;                 //スワイプで旋回の ON/OFF 
        }
        public TurnOperation turnOperation;


        //高さの操作
        [Serializable]
        public class HeightOperation
        {
            public float damping = 2.0f;            //上下高さのスムーズ移動速度

            //キー入力
            public bool keyEnable = true;           //高さのキー操作の ON/OFF
            public float keySpeed = 1.5f;           //キー操作での移動速度
            public KeyCode keyUp = KeyCode.C;       //高さ上へキー
            public KeyCode keyDown = KeyCode.V;     //高さ下へキー

            //ドラッグ
            public bool dragEnable = true;          //高さのドラッグ操作での ON/OFF
            public float dragSpeed = 0.5f;          //ドラッグ操作での高さ移動速度
        }
        public HeightOperation heightOperation;


        //距離の操作
        [Serializable]
        public class DistanceOperation
        {
            public float damping = 1.0f;            //距離のスムーズ移動速度（キーとホイール）
            public float min = 1.0f;                //XZ平面での最小距離

            //キー入力
            public bool keyEnable = true;           //距離のキー操作の ON/OFF
            public float keySpeed = 0.5f;           //距離の移動速度
            public KeyCode keyNear = KeyCode.B;     //近くへキー
            public KeyCode keyFar = KeyCode.N;      //遠くへキー

            //ホイール
            public bool wheelEnable = true;         //距離のホイール操作の ON/OFF
            public float wheelSpeed = 7f;           //ホイール１目盛りの速度

            //ピンチ
            public bool pinchEnable = true;         //ピンチで距離を操作する
            public float pinchDamping = 5f;         //ピンチでの距離のスムーズ移動速度（キーとホイールでの操作と分けるため）
            public float pinchSpeed = 40f;          //ピンチでの距離の変化速度
        }
        public DistanceOperation distanceOperation;


        //初期状態リセット操作
        [Serializable]
        public class ResetOperation
        {
            public bool keyEnable = true;               //初期状態リセットキーの ON/OFF
            public KeyCode key = KeyCode.KeypadPeriod;  //初期状態リセットキー
        }
        public ResetOperation resetOperation;


        //Local Values
        float angle;                                //カメラアングル(XZ平面)
        Vector3 startPos;                           //マウス移動始点
        float wantedDistance;                       //変化先距離
        float resetDistance;                        //初期距離保存用
        float resetHeight;                          //初期位置高さ保存用
        bool pinched = false;                       //ピンチで操作したフラグ（distanceDamping と pinchDistanceDamping を切り替える）
        bool dragging = false;                      //ドラッグの操作中フラグ


        // Use this for initialization
        void Start()
        {
            if (autoInitOnPlay && target != null)
            {
                height = transform.position.y - target.position.y;
                Vector3 dir = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);
                distance = dir.magnitude;
                preAngle = AngleXZWithSign(target.forward, dir);
            }

            angle = preAngle;
            resetDistance = wantedDistance = distance;
            resetHeight = height;
        }

        // Update is called once per frame
        void Update()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)   //タッチで取得したいプラットフォームのみ（モバイル等）
            if (Input.touchCount != 1 || Input.touches[0].fingerId != 0) //最初の指１本の操作に限定する
            {
                dragging = false;
                return;
            }
#endif

            //回転のキー操作
            if (angleOperation.keyEnable)
            {
                if (Input.GetKey(angleOperation.keyLeft))
                    angle = Mathf.Repeat(angle + angleOperation.keySpeed * Time.deltaTime, 360f);

                if (Input.GetKey(angleOperation.keyRight))
                    angle = Mathf.Repeat(angle - angleOperation.keySpeed * Time.deltaTime, 360f);
            }

            //旋回（一定角度回転）キー操作
            if (turnOperation.keyEnable)
            {
                if (Input.GetKeyDown(turnOperation.keyLeft))
                    TurnLeft();

                if (Input.GetKeyDown(turnOperation.keyRight))
                    TurnRight();
            }

            //高さのキー操作
            if (heightOperation.keyEnable)
            {
                if (Input.GetKey(heightOperation.keyUp))
                    height += heightOperation.keySpeed * Time.deltaTime;

                if (Input.GetKey(heightOperation.keyDown))
                    height -= heightOperation.keySpeed * Time.deltaTime;
            }

            //ドラッグ操作
            if (angleOperation.dragEnable || heightOperation.dragEnable)
            {
                Vector3 movePos = Vector3.zero;

                if (!dragging && Input.GetMouseButtonDown(0))
                {
                    startPos = Input.mousePosition;
                    if (validArea.xMin * Screen.width <= startPos.x && startPos.x <= validArea.xMax * Screen.width &&
                        validArea.yMin * Screen.height <= startPos.y && startPos.y <= validArea.yMax * Screen.height)
                    {
                        dragging = true;
                    }
                }
                else if (dragging)
                {
                    if (Input.GetMouseButton(0))
                    {
                        movePos = Input.mousePosition - startPos;
                        startPos = Input.mousePosition;

                        //ドラッグ幅で制限する（スワイプと分別するため）
                        if (angleOperation.dragWidthLimit > 0)
                        {
                            float limit = (widthReference ? Screen.width : Screen.height) * angleOperation.dragWidthLimit;
                            float d = Mathf.Max(Mathf.Abs(movePos.x), Mathf.Abs(movePos.y));  //大きい方で判定
                            if (d > limit)
                            {
                                movePos = Vector3.zero; //操作を無効にする
                                dragging = false;
                            }
                        }
                    }
                    else //Input.GetMouseButtonUp(0), exit
                    {
                        dragging = false;
                    }
                }

                if (movePos != Vector3.zero)
                {
                    //回転のドラッグ操作
                    if (angleOperation.dragEnable)
                        angle = Mathf.Repeat(angle + movePos.x * angleOperation.dragSpeed * Time.deltaTime, 360f);

                    //高さのドラッグ操作
                    if (heightOperation.dragEnable)
                        height -= movePos.y * heightOperation.dragSpeed * Time.deltaTime;
                }
            }

            //距離のキー操作
            if (distanceOperation.keyEnable)
            {
                if (Input.GetKey(distanceOperation.keyNear))
                {
                    wantedDistance = Mathf.Max(distanceOperation.min, distance - distanceOperation.keySpeed);
                    pinched = false;
                }

                if (Input.GetKey(distanceOperation.keyFar))
                {
                    wantedDistance = distance + distanceOperation.keySpeed;
                    pinched = false;
                }
            }

            //距離のホイール遠近
            if (distanceOperation.wheelEnable)
            {
                float mw = Input.GetAxis("Mouse ScrollWheel");
                if (mw != 0)
                {
                    wantedDistance = Mathf.Max(distanceOperation.min, distance - mw * distanceOperation.wheelSpeed); //0.1 x N倍
                    pinched = false;
                }
            }

            //初期状態リセット
            if (resetOperation.keyEnable)
            {
                if (Input.GetKeyDown(resetOperation.key))
                    ResetOperations();
            }
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            //追従先位置
            float wantedRotationAngle = target.eulerAngles.y + angle;
            float wantedHeight = target.position.y + height;

            //現在位置
            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            //追従先へのスムーズ移動距離(方向)
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle,
                angleOperation.damping * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightOperation.damping * Time.deltaTime);
            distance = Mathf.Lerp(distance, wantedDistance,
                (pinched ? distanceOperation.pinchDamping : distanceOperation.damping) * Time.deltaTime);

            //カメラの移動
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
            Vector3 pos = target.position - currentRotation * Vector3.forward * distance;
            pos.y = currentHeight;
            transform.position = pos;

            transform.LookAt(target);
        }


        //状態リセット（初期状態に戻す）
        public void ResetOperations()
        {
            height = resetHeight;
            distance = wantedDistance = resetDistance;
            angle = preAngle;
        }


        //ピンチで距離を操作（モバイル等）
        //http://fantom1x.blog130.fc2.com/blog-entry-288.html
        //・PinchInput を使用して距離を操作する。
        //width: ピンチ幅, delta: 直前のピンチ幅の差, ratio: ピンチ幅の開始時からの伸縮比(1:ピンチ開始時, 1以上拡大, 1より下(1/2,1/3,...)縮小)
        public void OnPinch(float width, float delta, float ratio)
        {
            if (!distanceOperation.pinchEnable)
                return;

            if (delta != 0)
            {
                wantedDistance = Mathf.Max(distanceOperation.min, distance - delta * distanceOperation.pinchSpeed);
                pinched = true;
            }
        }

        //スワイプで旋回
        //・SwipeInput を使用して旋回する。
        //http://fantom1x.blog130.fc2.com/blog-entry-250.html
        public void OnSwipe(Vector2 dir)
        {
            if (!turnOperation.swipeEnable)
                return;

            if (dir == Vector2.left)
                TurnLeft();
            else if (dir == Vector2.right)
                TurnRight();
        }


        //左旋回
        public void TurnLeft()
        {
            angle = Mathf.Repeat(MultipleCeil(angle - turnOperation.angle, turnOperation.angle), 360f);
        }

        //右旋回
        public void TurnRight()
        {
            angle = Mathf.Repeat(MultipleFloor(angle + turnOperation.angle, turnOperation.angle), 360f);
        }


        //以下、static method

        //より小さい倍数を求める（倍数で切り捨てられるような値）
        //http://fantom1x.blog130.fc2.com/blog-entry-248.html
        static float MultipleFloor(float value, float multiple)
        {
            return Mathf.Floor(value / multiple) * multiple;
        }

        //より大きい倍数を求める（倍数で繰り上がるような値）
        static float MultipleCeil(float value, float multiple)
        {
            return Mathf.Ceil(value / multiple) * multiple;
        }

        //2D（XY平面）での方向ベクトル同士の角度を符号付きで返す（度）
        //http://fantom1x.blog130.fc2.com/blog-entry-253.html#AngleWithSign
        static float AngleXZWithSign(Vector3 from, Vector3 to)
        {
            Vector3 projFrom = from;
            Vector3 projTo = to;
            projFrom.y = projTo.y = 0;  //y軸を無視する（XZ平面に投影する）
            float angle = Vector3.Angle(projFrom, projTo);
            float cross = CrossXZ(projFrom, projTo);
            return (cross != 0) ? angle * -Mathf.Sign(cross) : angle; //2D外積の符号を反転する
        }

        //2Dでの外積を求める（XY平面）
        //http://fantom1x.blog130.fc2.com/blog-entry-253.html#Cross2D
        static float CrossXZ(Vector3 a, Vector3 b)
        {
            return a.x * b.z - a.z * b.x;
        }
    }
}