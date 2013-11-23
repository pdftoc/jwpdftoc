* このプログラムについて

このプログラムは jw.org から PDF をダウンロードし、目次を追加します。



* 使い方

プログラムを起動し、言語と出版物を選択します。

「Download PDF」ボタンを押すと、 jw.org から PDF をダウンロードします。

「Convert PDF」ボタンを押すと、ダウンロードした PDF に目次データを反映した PDF ファイルを作成します。

出版物をダブルクリックすると、目次が追加された PDF が出力されるフォルダを開きます。



* 言語と出版物

publication.xml には言語と出版物が記述されています。
toc フォルダのテキストファイルには、各出版物の目次データが記述されています。

言語や出版物を追加するには、publication.xml を変更して目次ファイルを追加します。



* 要件と備考

.NET framework 2.0, .NET framework 3.0, .NET framework 3.5 のいずれかが必要です。

** jpdfbookmarks
目次の追加に jpdfbookmarks のコマンドラインツールを使用しています。
jpdfbookmarks の実行には Java Runtime Environment が必要です。

jpdfbookmarks はプログラムにより自動的にダウンロードされます。
http://sourceforge.net/projects/jpdfbookmarks/files/JPdfBookmarks-2.5.2/

jpdfbookmarks は内部的に以下のように呼び出されています。
jpdfbookmarks\jpdfbookmarks_cli.exe jw_org\bh_E.pdf -e UTF-8 -a toc\bh_E.txt -o output\bh_E.pdf

** Ionic.Zip.dll
Ionic.Zip.dll が同梱されています。
これはダウンロードした jpdfbookmarks の zip アーカイブを解凍するために使用されます。



* ライセンス

LISENCE.txt を参照
