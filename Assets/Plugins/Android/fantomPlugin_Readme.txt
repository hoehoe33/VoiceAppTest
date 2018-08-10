・ネイティブプラグイン「fantomPlugin.aar」は Minimum API Level：Android 4.2 (API 17) 以上で使用して下さい。

(※) ストレージのテキストファイル読み書き機能「StorageLoadTextController」「StorageSaveTextController」を利用するには「Android 4.4 (API 19)」以上にする必要があります

(※) センサーの値を取得するには各センサーの要求 API Level 以上にする必要があります。詳細は公式のドキュメントまたは、センサー関連メソッド・定数などのコメントを参照して下さい。
https://developer.android.com/reference/android/hardware/Sensor.html#TYPE_ACCELEROMETER

・プラグインのハードウェア音量キーのイベント取得、ダイアログ付きの音声認識、WIFIの設定を開く、Bluetooth接続要求（ダイアログ）、ストレージのテキストファイルの読み書き、ギャラリーの画像パス取得、MediaScannerの更新機能、バッテリーのステータス取得、画面回転の変化取得、センサーの値取得、デバイス認証、QRコードスキャナからのテキスト取得を利用する場合には、「AndroidManifest.xml」で Uniy 標準のアクティビティ「UnityPlayerActivity」をオーバーライドする必要があります（https://docs.unity3d.com/ja/current/Manual/AndroidUnityPlayerActivity.html）。「AndroidManifest-FullPlugin～.xml」はプラグイン独自のアクティビティ「FullPluginOnUnityPlayerActivity」がオーバーライドされています。リネームして使用して下さい。

・使用する機能によっては Android のパーミッションや属性が必要になります（https://developer.android.com/guide/topics/security/permissions.html）。パーミッションについては「Permission_ReadMe.txt」にまとめてあります。必要なパーミッションを「AndroidManifest.xml」にコピペして下さい。

・「AndroidManifest.xml」の"_Landscape"または"_Portrait","_Sensor"は画面回転の属性（screenOrientation）を指定しているだけで内容は同じです。任意に変更しても構いません。Unity側の設定に合わせて使って下さい（https://developer.android.com/guide/topics/manifest/activity-element.html#screen）。

(※) 警告「Unable to find unity activity in manifest. You need to make sure orientation attribute is set to sensorPortrait manually.」は独自の「AndroidManifest.xml」を使うと出るので無視して下さい（orientation＝画面回転はUnity側のアプリの設定に合わせた方が良い）。

------------------------------------------------
■デモについて

・デモをビルドするときは「AndroidManifest_test.xml」を「AndroidManifest.xml」にリネームして使って下さい（「Build Settings...」に「Assets/_Test/Scenes/」にあるシーンを追加して、「Switch Platform」で「Android」にしてビルドして下さい）。

・Unity2018.1.0～1.6では「Build System」を「Internal」にしてビルドして下さい。

※Unity2018.1.0～1.6でのビルドにおいて、「Build Settings...」で「Build System」を「Gradle」にした場合、「AndroidManifest.xml」にパッケージ名が追加されず、ビルドエラーが出ることが確認されてます（Unity2018.1.7以降はバグFixされてます）。その場合、「AndroidManifest.xml」の「manifest」タグに「package="(アプリのパッケージ名)"」（= Edit＞Project Settings＞Player＞Other Settings＞Identification＞Package Name）を記述すればビルドできます（Unity2017.4.2までは自動で追加されます）。
http://fantom1x.blog130.fc2.com/#unity2018_CommandInvokationFailure_packageName

※「GalleryPickTest」のデモには全天球のメッシュ（360 degrees）は含まれてません。必要であれば以下のURLから「Sphere100.fbx」ダウンロードし、ヒエラルキーの「GalleryPickTest(Script)」の「Sphere」にセットして下さい。また「Sphere100」の Material に「TextureMat」をセットして下さい。全天球は内側から覗く感じになるので、スケールの X にマイナス値を与えると画像を反転できます（大きさは任意。デモビデオではメッシュの Scale Factor=1000×Transform の Scale=-1）。セットアップ方法は以下の記事を参考にして下さい。

(360度「全天球」のセットアップ)
http://fantom1x.blog130.fc2.com/blog-entry-297.html
(全天球メッシュ：Sphere100.fbx)
http://warapuri.com/post/131599525953/
(Demo video：Vimeo)
https://vimeo.com/255712215


　C# の AndroidPlugin のメソッドを使用したとき、どんな Javaコード（プラグインを作った元のコード）が動いているかを簡単にブログにまとめてあります。簡易マニュアルにもなっているので、解説・詳細な仕様・注意点などを知りたい場合には参考にして下さい。

（ブログ記事）
http://fantom1x.blog130.fc2.com/blog-entry-273.html
http://fantom1x.blog130.fc2.com/blog-entry-293.html

------------------------------------------------
■使用ライブラリのライセンス等

このプラグインには Apache License, Version 2.0 のライセンスで配布されている成果物を含んでいます。
http://www.apache.org/licenses/LICENSE-2.0

ZXing ("Zebra Crossing") open source project (google). [ver.3.3.2] (QR Code Scan)
https://github.com/zxing/zxing

------------------------------------------------
By Fantom

[Blog] http://fantom1x.blog130.fc2.com/
[Twitter] https://twitter.com/fantom_1x
[SoundCloud] https://soundcloud.com/user-751508071
[Picotune] http://picotune.me/?@Fantom
[Monappy] https://monappy.jp/u/Fantom
[E-Mail] fantom_1x@yahoo.co.jp

