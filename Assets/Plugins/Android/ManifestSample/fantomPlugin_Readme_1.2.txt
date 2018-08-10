・ネイティブプラグイン「fantomPlugin.aar」は Minimum API Level：Android 4.2 (API 17) 以上で使用して下さい。

・「AndroidManifest.xml」はハードウェア音量キーのイベント取得、または音声認識でダイアログを表示する場合にオーバーライドする必要があります。両方使うには「AndroidManifest-FullPlugin～.xml」をリネームして使用して下さい。

・ハードウェア音量操作のイベントを受け取るには、「AndroidManifest-HardVolKey～.xml」または「AndroidManifest-FullPlugin～.xml」を使用して下さい。イベントを受け取らず、音量の取得と設定だけなら必要ありません。

・音声認識でダイアログを表示するには、「AndroidManifest-Speech～.xml」または「AndroidManifest-FullPlugin～.xml」を指定して下さい。ダイアログなしの場合は必要ありませんが、録音パーミッション「RECORD_AUDIO」は必要です（https://developer.android.com/reference/android/Manifest.permission.html#RECORD_AUDIO）。

・「AndroidManifest.xml」の"_API17"はテーマに「Theme.DeviceDefault.Light.NoActionBar.Fullscreen」を指定、"_API21"はテーマに「Theme.Material.Light.NoActionBar.Fullscreen」を指定しているだけで内容は同じです。元の「UnityPlayerActivity」をオーバーライドして起動すると、Unityが完全に起動するまでにアクションバーなどが表示されることがあるので、それを回避するために使っています。任意に変更しても構いません。ちなみに".Light"が付いているテーマは白系ベースで、付いてないものは黒系ベースです（https://developer.android.com/reference/android/R.style.html#Theme）。

・「AndroidManifest.xml」の"_Landscape"または"_Portrait"は画面回転の属性（screenOrientation）を指定しているだけで内容は同じです。任意に変更しても構いません。Unity側の設定に合わせて使って下さい（https://developer.android.com/guide/topics/manifest/activity-element.html#screen）。

・デモをビルドするときは「AndroidManifest_test.xml」を「AndroidManifest.xml」にリネームして使って下さい（「Build Settings...」に Assets/_Test/Scenes/ にあるシーンを追加してビルドして下さい）。

(※) 警告「Unable to find unity activity in manifest. You need to make sure orientation attribute is set to sensorPortrait manually.」は独自の「AndroidManifest.xml」を使うと出るので無視して下さい（orientation＝画面回転はUnity側のアプリの設定に合わせた方が良い）。

　C# の AndroidPlugin のメソッドを使用したとき、どんな Javaコード（プラグインを作った元のコード）が動いているかを簡単にブログにまとめてあります。簡易マニュアルにもなっているので、解説・詳細な仕様・注意点などを知りたい場合には参考にして下さい。

（ブログ記事）
http://fantom1x.blog130.fc2.com/blog-entry-273.html

------------------------------------------------
By Fantom

[Blog] http://fantom1x.blog130.fc2.com/
[Twitter] https://twitter.com/fantom_1x
[Monappy] https://monappy.jp/u/Fantom

