


1151VR 作業四 Unity專案製作說明


一、遊戲設計
這次的遊戲是一架自動往前飛的飛機，玩家按住螢幕拖曳來控制飛機的左右與高度，在飛行過程中閃避或撞上障礙物。場景中有三個音效，分別是飛行時持續播放的引擎聲、撞到障礙物的撞擊聲，以及順利通過障礙物的通過聲。程式分成兩支，PlaneController 負責控制飛機、偵測碰撞與播放音效，Obstacle 負責判斷飛機是否順利通過。
二、製作流程
首先在 Unity Hub 建立 Universal 3D 專案，匯入飛機模型與 fly.mp3、crash.mp3、pass.mp3 三個音效檔。
場景配置方面，先建立一個空物件 Airplane，Position 為 0, 4, 0，Scale 為 1, 1, 1。飛機模型 plane 設為它的子物件，Position 為 0, 1, 0，Scale 為 0.07, 0.07, 0.07。Main Camera 同樣設為 Airplane 的子物件，Position 為 0, 3, -12，Rotation 為 10, 0, 0，這樣相機會自動跟著飛機移動，不需要另外寫跟隨程式。障礙物使用內建的 Cylinder，Position 為 3, 6, 40，Scale 為 3, 8, 2。地面使用 3D Object 的 Plane，命名為 Ground，Position 為 0, 0, 30，Scale 為 2, 1, 10。
碰撞設定上，在 plane 加上 Box Collider，它會自動包住機身，Airplane 掛上 PlaneController，Unity 會自動為它加上 Rigidbody，腳本會在 Start 時把它設為 Kinematic 並關閉重力。Cylinder 的 Capsule Collider 勾選 Is Trigger，並掛上 Obstacle 腳本。
PlaneController 的運作方式是，每個影格讓飛機沿 Z 軸往前移動，同時用 MoveTowards 讓 X 與 Y 平滑追向手指位置。手指位置會先換算成螢幕上的比例，再對應到飛行範圍，左右在正負 8 之間，高度在 2 到 16 之間。輸入的部分用 #if ENABLE_INPUT_SYSTEM 同時支援新舊兩種 Input System，並同時處理手機觸控與電腦滑鼠。飛機碰到障礙物時由 OnTriggerEnter 偵測，Hits 加 1 並播放撞擊音效。Obstacle 則每個影格檢查飛機的 Z 座標是否已經超過障礙物，超過且沒有撞到的話，Passed 加 1 並播放通過音效。音效由腳本在 Awake 時建立兩個 AudioSource，一個循環播放飛行聲，另一個用 PlayOneShot 播放撞擊與通過音效。飛到終點後遊戲停止並關閉飛行聲，點一下螢幕會重新載入場景。畫面左上的 Passed 與 Hits 文字用 OnGUI 顯示，並以 Screen.safeArea 避開手機的圓角與打孔鏡頭。
Airplane 上 Plane Controller 的參數設定為 Forward Speed 10、Move Speed 15、X Limit 8、Y Min 2、Y Max 16、End Z 70，三個音效欄位分別拖入 fly.mp3、crash.mp3 與 pass.mp3。
在電腦上測試時，按下 Play 後按住滑鼠左鍵拖曳，確認飛行聲持續播放，撞上圓柱會有撞擊聲且 Hits 加 1，閃過圓柱會有通過聲且 Passed 加 1，飛到終點後會停止。
輸出到手機時，先在手機上上開啟 USB 偵錯、關閉 Auto Blocker，使用可傳輸資料的線連接電腦，USB 模式選擇傳輸檔案並允許偵錯授權。接著Default Orientation 設為 Landscape Left。在 Build Profiles 選擇 Android，最後按下 Build And Run，遊戲就會自動安裝並在手機上開啟。

三、主要問題與解決方式
第一個問題是遊玩時很快就撞到障礙物。原因是自動產生的 Box Collider 比機身大，障礙物又擺在飛行路徑上。解決方式是把 Box Collider 縮小到 60%，障礙物位置調到 X 等於 3 的一側，並把 Y Max 提高到 16，讓飛機有更多空間閃避。
第二個問題是手機上 Passed 與 Hits 文字被圓角與鏡頭切到，因為文字貼著螢幕邊緣。解決方式是改用 Screen.safeArea 計算文字位置，並用 Simulator 的 Safe Area 預覽確認。

四、成果
遊戲在電腦與 安著手機實機上都能正常執行。飛機自動前進，按住螢幕拖曳可以控制左右與高度，飛行聲持續播放，撞到障礙物會播放撞擊聲，順利通過會播放通過聲，畫面並顯示通過與撞擊的次數。

<img width="959" height="500" alt="螢幕擷取畫面 2026-10-31 194338" src="https://github.com/user-attachments/assets/8767c5fa-f348-4e74-bd14-344bc68ed18a" />


影片連結:　https://www.youtube.com/watch?si=1B7W7hSqVVhX7WIE&v=ks5Xclst56I&feature=youtu.be
Github連結: https://github.com/yorushikasuisnbuna/1151VR-HW4-414261278-wjf
