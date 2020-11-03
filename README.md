# UnityTool_Object2Terrain
選択中のオブジェクトの上部に、Terrainの地形を合わせるツール

# 概要
ほぼ次のサイトの写経
- [オブジェクトをTerrain（地形）に転写](https://eiki.hatenablog.jp/entry/20140705/1404651350)

APIが古い書き方と怒られた部分の変更と、Unity習得のためにコードを解析してメモしている。

こんな短いコードでEditorの拡張出来ちゃう、Unity偉い、凄い。

Heightmapさえ作れれば良いのだから、Terrainの編集は結構色々出来そう。

# 設定

Assets/Editor以下に任意の名前で、create/C# Scriptで空のスクリプト作成して、コードを貼り付ける。

Terrainが一つだけのシーンで、地形の上にPanelみたいなコライダーが付いているGameObjectを配置して、
選択した状態で、メニューのCustom/Edit/Object To Terrainを選択するか、<kbd>Alt</kbd> + <kbd>Q</kbd>で
地形が変化する。

# 備考

Unity習得のための自分用のツールだから何の保証もありませぬ。ご利用は慎重に。
