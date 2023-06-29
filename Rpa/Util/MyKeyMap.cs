using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rpa.Util
{
    class MyKeyMap
    {
        #region "コールバック"
        //どこの処理やっているか
        public delegate void LineCallback(string line, int staus);

        private static LineCallback _lineCallback;

        public static LineCallback lineCallback { get { return _lineCallback; } set { _lineCallback = value; } }

        #endregion


        #region " WIN32 dll http://pgcenter.web.fc2.com/contents/csharp_sendinput.html"

        // マウスイベント(mouse_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // キーボードイベント(keybd_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // ハードウェアイベント
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        // 各種イベント(SendInputの引数データ)
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)] public int type;
            [FieldOffset(4)] public MOUSEINPUT mi;
            [FieldOffset(4)] public KEYBDINPUT ki;
            [FieldOffset(4)] public HARDWAREINPUT hi;
        };

        [DllImport("user32.dll")]
        static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(int wCode, int wMapType);
        #endregion

        #region "定義"

        private const int INPUT_MOUSE = 0;                  // マウスイベント
        private const int INPUT_KEYBOARD = 1;               // キーボードイベント
        private const int INPUT_HARDWARE = 2;               // ハードウェアイベント

        private const int MOUSEEVENTF_MOVE = 0x1;           // マウスを移動する
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;    // 絶対座標指定
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;       // 左　ボタンを押す
        private const int MOUSEEVENTF_LEFTUP = 0x4;         // 左　ボタンを離す
        private const int MOUSEEVENTF_RIGHTDOWN = 0x8;      // 右　ボタンを押す
        private const int MOUSEEVENTF_RIGHTUP = 0x10;       // 右　ボタンを離す
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;    // 中央ボタンを押す
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;      // 中央ボタンを離す
        private const int MOUSEEVENTF_WHEEL = 0x800;        // ホイールを回転する
        private const int WHEEL_DELTA = 120;                // ホイール回転値

        private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
        private const int KEYEVENTF_KEYUP = 0x2;            // キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード
        private const int VK_SHIFT = 0x10;                  // SHIFTキー
        private static readonly string[,] v = new string[2, 2]
                    {
                { "a", "55" },
                { "a", "55" }

                };

        #endregion

        #region "キーコードフィールド"
        /*
A	65	
A キー。
Add	107	
Add キー
Alt	262144	
Alt 修飾子キー
Apps	93	
アプリケーション キー (Microsoft Natural Keyboard)
Attn	246	
Attn キー。
B	66	
B キー。
Back	8	
BackSpace キー。
BrowserBack	166	
ブラウザーの戻るキー。
BrowserFavorites	171	
ブラウザーのお気に入りキー。
BrowserForward	167	
ブラウザーの進むキー。
BrowserHome	172	
ブラウザーのホーム キー。
BrowserRefresh	168	
ブラウザーの更新キー。
BrowserSearch	170	
ブラウザーの検索キー。
BrowserStop	169	
ブラウザーの中止キー。
C	67	
C キー。
Cancel	3	
Cancel キー
Capital	20	
CAPS LOCK キー
CapsLock	20	
CAPS LOCK キー
Clear	12	
Clear キー。
Control	131072	
Ctrl 修飾子キー
ControlKey	17	
CTRL キー
Crsel	247	
Crsel キー。
D	68	
D キー。
D0	48	
0 キー。
D1	49	
1 キー。
D2	50	
2 キー。
D3	51	
3 キー。
D4	52	
4 キー。
D5	53	
5 キー。
D6	54	
6 キー。
D7	55	
7 キー。
D8	56	
8 キー。
D9	57	
9 キー。
Decimal	110	
小数点キー
Delete	46	
DEL キー
Divide	111	
除算記号 (/) キー
Down	40	
↓キー。
E	69	
E キー。
End	35	
End キー。
Enter	13	
Enter キー。
EraseEof	249	
Erase Eof キー。
Escape	27	
Esc キー。
Execute	43	
Execute キー。
Exsel	248	
Exsel キー。
F	70	
F キー。
F1	112	
F1 キー。
F10	121	
F10 キー。
F11	122	
F11 キー。
F12	123	
F12 キー。
F13	124	
F13 キー。
F14	125	
F14 キー。
F15	126	
F15 キー。
F16	127	
F16 キー。
F17	128	
F17 キー。
F18	129	
F18 キー。
F19	130	
F19 キー。
F2	113	
F2 キー。
F20	131	
F20 キー。
F21	132	
F21 キー。
F22	133	
F22 キー。
F23	134	
F23 キー。
F24	135	
F24 キー。
F3	114	
F3 キー。
F4	115	
F4 キー。
F5	116	
F5 キー。
F6	117	
F6 キー。
F7	118	
F7 キー。
F8	119	
F8 キー。
F9	120	
F9 キー。
FinalMode	24	
IME Final モード キー
G	71	
G キー。
H	72	
H キー。
HanguelMode	21	
IME ハングル モード キー (互換性を保つために保持されています。HangulMode を使用します)
HangulMode	21	
IME ハングル モード キー。
HanjaMode	25	
IME Hanja モード キー。
Help	47	
Help キー。
Home	36	
Home キー。
I	73	
I キー。
IMEAccept	30	
IME Accept キー (IMEAceept の代わりに使用します)
IMEAceept	30	
IME Accept キー 互換性を維持するために残されています。代わりに IMEAccept を使用してください。
IMEConvert	28	
IME 変換キー
IMEModeChange	31	
IME モード変更キー
IMENonconvert	29	
IME 無変換キー
Insert	45	
INS キー
J	74	
J キー。
JunjaMode	23	
IME Junja モード キー。
K	75	
K キー。
KanaMode	21	
IME かなモード キー。
KanjiMode	25	
IME 漢字モード キー。
KeyCode	65535	
キー値からキー コードを抽出するビット マスク。
L	76	
L キー。
LaunchApplication1	182	
カスタム ホット キー 1。
LaunchApplication2	183	
カスタム ホット キー 2。
LaunchMail	180	
メールの起動キー。
LButton	1	
マウスの左ボタン
LControlKey	162	
左 Ctrl キー。
Left	37	
←キー。
LineFeed	10	
ライン フィード キー
LMenu	164	
左 Alt キー。
LShiftKey	160	
左の Shift キー
LWin	91	
左 Windows ロゴ キー (Microsoft Natural Keyboard)。
M	77	
M キー。
MButton	4	
マウスの中央ボタン (3 ボタン マウスの場合)
MediaNextTrack	176	
メディアの次のトラック キー。
MediaPlayPause	179	
メディアの再生/一時停止キー。
MediaPreviousTrack	177	
メディアの前のトラック キー。
MediaStop	178	
メディアの停止キー。
Menu	18	
Alt キー。
Modifiers	-65536	
キー値から修飾子を抽出するビット マスク。
Multiply	106	
乗算記号 (*) キー
N	78	
N キー。
Next	34	
Page Down キー。
NoName	252	
将来使用するために予約されている定数。
None	0	
押されたキーがありません。
NumLock	144	
NUM LOCK キー
NumPad0	96	
0 キー (テンキー)。
NumPad1	97	
1 キー (テンキー)。
NumPad2	98	
2 キー (テンキー)。
NumPad3	99	
3 キー (テンキー)。
NumPad4	100	
4 キー (テンキー)。
NumPad5	101	
5 キー (テンキー)。
NumPad6	102	
6 キー (テンキー)。
NumPad7	103	
7 キー (テンキー)。
NumPad8	104	
8 キー (テンキー)。
NumPad9	105	
9 キー (テンキー)
O	79	
O キー。
Oem1	186	
OEM 1 キー。
Oem102	226	
OEM 102 キー。
Oem2	191	
OEM 2 キー。
Oem3	192	
OEM 3 キー。
Oem4	219	
OEM 4 キー。
Oem5	220	
OEM 5 キー。
Oem6	221	
OEM 6 キー。
Oem7	222	
OEM 7 キー。
Oem8	223	
OEM 8 キー。
OemBackslash	226	
RT 102 キーのキーボード上の OEM 山かっこキーまたは円記号キー。
OemClear	254	
Clear キー。
OemCloseBrackets	221	
米国標準キーボード上の OEM 右角かっこキー。
Oemcomma	188	
国または地域別キーボード上の OEM コンマ キー。
OemMinus	189	
国または地域別キーボード上の OEM マイナス キー。
OemOpenBrackets	219	
米国標準キーボード上の OEM 左角かっこキー。
OemPeriod	190	
国または地域別キーボード上の OEM ピリオド キー。
OemPipe	220	
米国標準キーボード上の OEM パイプ キー。
Oemplus	187	
国または地域別キーボード上の OEM プラス キー。
OemQuestion	191	
米国標準キーボード上の OEM 疑問符キー。
OemQuotes	222	
米国標準キーボード上の OEM 一重/二重引用符キー。
OemSemicolon	186	
米国標準キーボード上の OEM セミコロン キー。
Oemtilde	192	
米国標準キーボード上の OEM チルダ キー。
P	80	
P キー。
Pa1	253	
PA1 キー。
Packet	231	
Unicode 文字がキーストロークであるかのように渡されます。 Packet のキー値は、キーボード以外の入力手段に使用される 32 ビット仮想キー値の下位ワードです。
PageDown	34	
Page Down キー。
PageUp	33	
Page Up キー。
Pause	19	
Pause キー。
Play	250	
Play キー。
Print	42	
Print キー。
PrintScreen	44	
Print Screen キー。
Prior	33	
Page Up キー。
ProcessKey	229	
ProcessKey キー
Q	81	
Q キー。
R	82	
R キー。
RButton	2	
マウスの右ボタン
RControlKey	163	
右 Ctrl キー。
Return	13	
Return キー
Right	39	
→キー。
RMenu	165	
右 Alt キー。
RShiftKey	161	
右の Shift キー
RWin	92	
右 Windows ロゴ キー (Microsoft Natural Keyboard)。
S	83	
S キー。
Scroll	145	
ScrollLock キー
Select	41	
Select キー。
SelectMedia	181	
メディアの選択キー。
Separator	108	
区切り記号キー
Shift	65536	
Shift 修飾子キー
ShiftKey	16	
Shift キー
Sleep	95	
コンピューターのスリープ キー
Snapshot	44	
Print Screen キー。
Space	32	
Space キー。
Subtract	109	
減算記号 (-) キー
T	84	
T キー。
Tab	9	
Tab キー。
U	85	
U キー。
Up	38	
↑キー。
V	86	
V キー。
VolumeDown	174	
音量下げるキー。
VolumeMute	173	
音量ミュート キー。
VolumeUp	175	
音量上げるキー。
W	87	
W キー。
X	88	
X キー。
XButton1	5	
x マウスの 1 番目のボタン (5 ボタン マウスの場合)
XButton2	6	
x マウスの 2 番目のボタン (5 ボタン マウスの場合)
Y	89	
Y キー。
Z	90	
Z キー。
Zoom	251	
Zoom キー。
         */
        #endregion

        struct ENUMKEY
        {
            public string name;
            public string no;
            public string memo;
        }

        //https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.forms.keys?view=netcore-3.1
        //static readonly ENUMKEY[] EnumKyes = new ENUMKEY[] {
        static readonly string[,] EnumKyes = new string[,] {
            {"A","65","A キー。"},
            {"Add","107","Add キー"},
            {"Alt","262144","Alt 修飾子キー"},
            {"Apps","93","アプリケーション キー (Microsoft Natural Keyboard)"},
            {"Attn","246","Attn キー。"},
            {"B","66","B キー。"},
            {"Back","8","BackSpace キー。"},
            {"BrowserBack","166","ブラウザーの戻るキー。"},
            {"BrowserFavorites","171","ブラウザーのお気に入りキー。"},
            {"BrowserForward","167","ブラウザーの進むキー。"},
            {"BrowserHome","172","ブラウザーのホーム キー。"},
            {"BrowserRefresh","168","ブラウザーの更新キー。"},
            {"BrowserSearch","170","ブラウザーの検索キー。"},
            {"BrowserStop","169","ブラウザーの中止キー。"},
            {"C","67","C キー。"},
            {"Cancel","3","Cancel キー"},
            {"Capital","20","CAPS LOCK キー"},
            {"CapsLock","20","CAPS LOCK キー"},
            {"Clear","12","Clear キー。"},
            {"Control","131072","Ctrl 修飾子キー"},
            {"ControlKey","17","CTRL キー"},
            {"Crsel","247","Crsel キー。"},
            {"D","68","D キー。"},
            {"D0","48","0 キー。"},
            {"D1","49","1 キー。"},
            {"D2","50","2 キー。"},
            {"D3","51","3 キー。"},
            {"D4","52","4 キー。"},
            {"D5","53","5 キー。"},
            {"D6","54","6 キー。"},
            {"D7","55","7 キー。"},
            {"D8","56","8 キー。"},
            {"D9","57","9 キー。"},
            {"Decimal","110","小数点キー"},
            {"Delete","46","DEL キー"},
            {"Divide","111","除算記号 (/) キー"},
            {"Down","40","↓キー。"},
            {"E","69","E キー。"},
            {"End","35","End キー。"},
            {"Enter","13","Enter キー。"},
            {"EraseEof","249","Erase Eof キー。"},
            {"Escape","27","Esc キー。"},
            {"Execute","43","Execute キー。"},
            {"Exsel","248","Exsel キー。"},
            {"F","70","F キー。"},
            {"F1","112","F1 キー。"},
            {"F10","121","F10 キー。"},
            {"F11","122","F11 キー。"},
            {"F12","123","F12 キー。"},
            {"F13","124","F13 キー。"},
            {"F14","125","F14 キー。"},
            {"F15","126","F15 キー。"},
            {"F16","127","F16 キー。"},
            {"F17","128","F17 キー。"},
            {"F18","129","F18 キー。"},
            {"F19","130","F19 キー。"},
            {"F2","113","F2 キー。"},
            {"F20","131","F20 キー。"},
            {"F21","132","F21 キー。"},
            {"F22","133","F22 キー。"},
            {"F23","134","F23 キー。"},
            {"F24","135","F24 キー。"},
            {"F3","114","F3 キー。"},
            {"F4","115","F4 キー。"},
            {"F5","116","F5 キー。"},
            {"F6","117","F6 キー。"},
            {"F7","118","F7 キー。"},
            {"F8","119","F8 キー。"},
            {"F9","120","F9 キー。"},
            {"FinalMode","24","IME Final モード キー"},
            {"G","71","G キー。"},
            {"H","72","H キー。"},
            {"HanguelMode","21","IME ハングル モード キー (互換性を保つために保持されています。HangulMode を使用します)"},
            {"HangulMode","21","IME ハングル モード キー。"},
            {"HanjaMode","25","IME Hanja モード キー。"},
            {"Help","47","Help キー。"},
            {"Home","36","Home キー。"},
            {"I","73","I キー。"},
            {"IMEAccept","30","IME Accept キー (IMEAceept の代わりに使用します)"},
            {"IMEAceept","30","IME Accept キー 互換性を維持するために残されています。代わりに IMEAccept を使用してください。"},
            {"IMEConvert","28","IME 変換キー"},
            {"IMEModeChange","31","IME モード変更キー"},
            {"IMENonconvert","29","IME 無変換キー"},
            {"Insert","45","INS キー"},
            {"J","74","J キー。"},
            {"JunjaMode","23","IME Junja モード キー。"},
            {"K","75","K キー。"},
            {"KanaMode","21","IME かなモード キー。"},
            {"KanjiMode","25","IME 漢字モード キー。"},
            {"KeyCode","65535","キー値からキー コードを抽出するビット マスク。"},
            {"L","76","L キー。"},
            {"LaunchApplication1","182","カスタム ホット キー 1。"},
            {"LaunchApplication2","183","カスタム ホット キー 2。"},
            {"LaunchMail","180","メールの起動キー。"},
            {"LButton","1","マウスの左ボタン"},
            {"LControlKey","162","左 Ctrl キー。"},
            {"Left","37","←キー。"},
            {"LineFeed","10","ライン フィード キー"},
            {"LMenu","164","左 Alt キー。"},
            {"LShiftKey","160","左の Shift キー"},
            {"LWin","91","左 Windows ロゴ キー (Microsoft Natural Keyboard)。"},
            {"M","77","M キー。"},
            {"MButton","4","マウスの中央ボタン (3 ボタン マウスの場合)"},
            {"MediaNextTrack","176","メディアの次のトラック キー。"},
            {"MediaPlayPause","179","メディアの再生/一時停止キー。"},
            {"MediaPreviousTrack","177","メディアの前のトラック キー。"},
            {"MediaStop","178","メディアの停止キー。"},
            {"Menu","18","Alt キー。"},
            {"Modifiers","-65536","キー値から修飾子を抽出するビット マスク。"},
            {"Multiply","106","乗算記号 (*) キー"},
            {"N","78","N キー。"},
            {"Next","34","Page Down キー。"},
            {"NoName","252","将来使用するために予約されている定数。"},
            {"None","0","押されたキーがありません。"},
            {"NumLock","144","NUM LOCK キー"},
            {"NumPad0","96","0 キー (テンキー)。"},
            {"NumPad1","97","1 キー (テンキー)。"},
            {"NumPad2","98","2 キー (テンキー)。"},
            {"NumPad3","99","3 キー (テンキー)。"},
            {"NumPad4","100","4 キー (テンキー)。"},
            {"NumPad5","101","5 キー (テンキー)。"},
            {"NumPad6","102","6 キー (テンキー)。"},
            {"NumPad7","103","7 キー (テンキー)。"},
            {"NumPad8","104","8 キー (テンキー)。"},
            {"NumPad9","105","9 キー (テンキー)"},
            {"O","79","O キー。"},
            {"Oem1","186","OEM 1 キー。"},
            {"Oem102","226","OEM 102 キー。"},
            {"Oem2","191","OEM 2 キー。"},
            {"Oem3","192","OEM 3 キー。"},
            {"Oem4","219","OEM 4 キー。"},
            {"Oem5","220","OEM 5 キー。"},
            {"Oem6","221","OEM 6 キー。"},
            {"Oem7","222","OEM 7 キー。"},
            {"Oem8","223","OEM 8 キー。"},
            {"OemBackslash","226","RT 102 キーのキーボード上の OEM 山かっこキーまたは円記号キー。"},
            {"OemClear","254","Clear キー。"},
            {"OemCloseBrackets","221","米国標準キーボード上の OEM 右角かっこキー。"},
            {"Oemcomma","188","国または地域別キーボード上の OEM コンマ キー。"},
            {"OemMinus","189","国または地域別キーボード上の OEM マイナス キー。"},
            {"OemOpenBrackets","219","米国標準キーボード上の OEM 左角かっこキー。"},
            {"OemPeriod","190","国または地域別キーボード上の OEM ピリオド キー。"},
            {"OemPipe","220","米国標準キーボード上の OEM パイプ キー。"},
            {"Oemplus","187","国または地域別キーボード上の OEM プラス キー。"},
            {"OemQuestion","191","米国標準キーボード上の OEM 疑問符キー。"},
            {"OemQuotes","222","米国標準キーボード上の OEM 一重/二重引用符キー。"},
            {"OemSemicolon","186","米国標準キーボード上の OEM セミコロン キー。"},
            {"Oemtilde","192","米国標準キーボード上の OEM チルダ キー。"},
            {"P","80","P キー。"},
            {"Pa1","253","PA1 キー。"},
            {"Packet","231","Unicode 文字がキーストロークであるかのように渡されます。 Packet のキー値は、キーボード以外の入力手段に使用される 32 ビット仮想キー値の下位ワードです。"},
            {"PageDown","34","Page Down キー。"},
            {"PageUp","33","Page Up キー。"},
            {"Pause","19","Pause キー。"},
            {"Play","250","Play キー。"},
            {"Print","42","Print キー。"},
            {"PrintScreen","44","Print Screen キー。"},
            {"Prior","33","Page Up キー。"},
            {"ProcessKey","229","ProcessKey キー"},
            {"Q","81","Q キー。"},
            {"R","82","R キー。"},
            {"RButton","2","マウスの右ボタン"},
            {"RControlKey","163","右 Ctrl キー。"},
            {"Return","13","Return キー"},
            {"Right","39","→キー。"},
            {"RMenu","165","右 Alt キー。"},
            {"RShiftKey","161","右の Shift キー"},
            {"RWin","92","右 Windows ロゴ キー (Microsoft Natural Keyboard)。"},
            {"S","83","S キー。"},
            {"Scroll","145","ScrollLock キー"},
            {"Select","41","Select キー。"},
            {"SelectMedia","181","メディアの選択キー。"},
            {"Separator","108","区切り記号キー"},
            {"Shift","65536","Shift 修飾子キー"},
            {"ShiftKey","16","Shift キー"},
            {"Sleep","95","コンピューターのスリープ キー"},
            {"Snapshot","44","Print Screen キー。"},
            {"Space","32","Space キー。"},
            {"Subtract","109","減算記号 (-) キー"},
            {"T","84","T キー。"},
            {"Tab","9","Tab キー。	"},
            {"U","85","U キー。	"},
            {"Up","38","↑キー。	"},
            {"V","86","V キー。	"},
            {"VolumeDown","174","音量下げるキー。	"},
            {"VolumeMute","173","音量ミュート キー。	"},
            {"VolumeUp","175","音量上げるキー。	"},
            {"W","87","W キー。	"},
            {"X","88","X キー。	"},
            {"XButton1","5","x マウスの 1 番目のボタン (5 ボタン マウスの場合)	"},
            {"XButton2","6","x マウスの 2 番目のボタン (5 ボタン マウスの場合)	"},
            {"Y","89","Y キー。	"},
            {"Z","90","Z キー。	"},
            {"Zoom","251","Zoom キー。	"}
        };


        private static Dictionary<int, string> _d = new Dictionary<int, string>();

        private static Dictionary<string, Keys> _d2 = new Dictionary<string, Keys>();

        public static  Dictionary<int, string> d { get { return _d; }set { _d = value; } }
        public static Dictionary<string, Keys> d2 { get { return _d2; } set { _d2 = value; } }

        /// <summary>
        /// キースキャンコード
        /// https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-6.0/aa299374(v=vs.60)?redirectedfrom=MSDN
        /// </summary>
        public static void MapKeyData()
        {
            int KEYS_SIZE = 20000;
            int[] mapKey = new int[KEYS_SIZE];
            /*
             MAPVK_VK_TO_CHAR
            2
            uCode is a virtual-key code and is translated into an unshifted character value in the low-order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.
            MAPVK_VK_TO_VSC
            0
            uCode is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.
            MAPVK_VSC_TO_VK
            1
            uCode is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.
            MAPVK_VSC_TO_VK_EX
            3
            uCode is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.
                         */

            setEnumKey();

            string strbuf;
            int cnt = 0;
            int i = 0;
            for (i = 0; i < KEYS_SIZE; i++)
            {
                //KEYS_SIZE=255 
                mapKey[i] = MapVirtualKey(i, 0);//mapKey is unsigned char array 
                //Keys.KanjiMode
                if (!d.ContainsKey(mapKey[i]))
                {
                    Console.WriteLine("{0} : {1} -", i, mapKey[i]);
                }
                else
                {
                    //Keys.LWin.ToString()
                    strbuf = string.Format("{0} : {1} {2}", cnt, mapKey[i], d[mapKey[i]].ToString());

                    Console.WriteLine(strbuf);
                    
                    lineCallback(strbuf, 0);

                    cnt++;
                }
            }

            lineCallback("-----------------", 0);
            //cnt = Enum.GetValues(typeof(Keys)).Length;
            //for (i=0; cnt>i; i++)
            //{
            //    strbuf = string.Format("{0} : {1} ", cnt, Keys. );

            //    Console.WriteLine(strbuf);

            //    lineCallback(strbuf);
            //}

            foreach (Keys Value in Enum.GetValues(typeof(Keys)))
            {
                string name = Enum.GetName(typeof(Keys), Value);

                strbuf = string.Format("{0}：{1}", name, (int)Value);

                Console.WriteLine(strbuf);

                lineCallback(strbuf,0);
            }



        }

        private static void setEnumKey()
        {   int cnt = EnumKyes.Length/3;

            for(int i=0; cnt>i; i++)
            {
                string key = EnumKyes[i, 1];
                string val = EnumKyes[i, 0] + "," + EnumKyes[i, 2]+","+ getEnumKeyStr(key);
                //まだ未登録＆Noneではない場合登録
                if (!d.ContainsKey(int.Parse(key)) && EnumKyes[i, 0] != "None")
                {
                    d[int.Parse(key)] = val;
                }
            }
        }

        /// <summary>
        /// RのEnuｍ、　Keys.R を逆引き
        /// </summary>
        public static void setEnumKey2Str()
        {
            foreach (Keys Value in Enum.GetValues(typeof(Keys)))
            {
                string name = Enum.GetName(typeof(Keys), Value);

                //strbuf = string.Format("{0}：{1}", name, (int)Value);

                //Console.WriteLine(strbuf);

                d2[name] = Value;

                //lineCallback(strbuf);
            }

        }


        private static string getEnumKeyStr(string key)
        {
            string rlt = "";
            //Keys.RWin

            return rlt;
        }

        public static void send(Keys[] k)
        {
            // 自ウインドウを非表示(キーボード操作対象のウィンドウへフォーカスを移動するため)
            //this.Visible = false;

            int i = 0;
            int cnt = k.Count();
            // キーボード操作実行用のデータ
            int num = cnt * 2;
            INPUT[] inp = new INPUT[num];

            if (num == 0) return;

            for (i=0; cnt>i; i++)
            {
                // キーボードを押す
                inp[i].type = INPUT_KEYBOARD;
                inp[i].ki.wVk = (short)k[i];
                inp[i].ki.wScan = (short)MapVirtualKey(inp[i].ki.wVk, 0);
                inp[i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
                inp[i].ki.dwExtraInfo = 0;
                inp[i].ki.time = 0;

                // キーボードを離す
                inp[cnt + i].type = INPUT_KEYBOARD;
                inp[cnt + i].ki.wVk = (short)k[i];
                inp[cnt + i].ki.wScan = (short)MapVirtualKey(inp[cnt + i].ki.wVk, 0);
                inp[cnt + i].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
                inp[cnt + i].ki.dwExtraInfo = 0;
                inp[cnt + i].ki.time = 0;
            }


            // キーボード操作実行
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

            // 1000ミリ秒スリープ
            System.Threading.Thread.Sleep(1000);

            // 自ウインドウを表示
            //this.Visible = true;
        }

        private void send(string s)
        {
            // 自ウインドウを非表示(キーボード操作対象のウィンドウへフォーカスを移動するため)
            //this.Visible = false;

            // キーボード操作実行用のデータ
            const int num = 4;
            INPUT[] inp = new INPUT[num];

            // (1)キーボード(SHIFT)を押す
            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_SHIFT;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            // (2)キーボード(A)を押す
            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = (short)Keys.A;
            inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
            inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[1].ki.dwExtraInfo = 0;
            inp[1].ki.time = 0;

            // (3)キーボード(A)を離す
            inp[2].type = INPUT_KEYBOARD;
            inp[2].ki.wVk = (short)Keys.A;
            inp[2].ki.wScan = (short)MapVirtualKey(inp[2].ki.wVk, 0);
            inp[2].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[2].ki.dwExtraInfo = 0;
            inp[2].ki.time = 0;

            // (4)キーボード(SHIFT)を離す
            inp[3].type = INPUT_KEYBOARD;
            inp[3].ki.wVk = VK_SHIFT;
            inp[3].ki.wScan = (short)MapVirtualKey(inp[3].ki.wVk, 0);
            inp[3].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[3].ki.dwExtraInfo = 0;
            inp[3].ki.time = 0;

            // キーボード操作実行
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

            // 1000ミリ秒スリープ
            System.Threading.Thread.Sleep(1000);

            // 自ウインドウを表示
            //this.Visible = true;
        }


    }
}
