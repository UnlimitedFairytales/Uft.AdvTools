using System.Collections.Generic;
using System.Linq;
using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;
using Uft.AdvTools.Loader;
using Uft.AdvTools.View;
using Uft.FadeEffects;
using Uft.UnityUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/*
# 仕様

## 各設定シート

## Characterシート : 宴4に対する制限事項

- CharacterName, NameText, Pattern, X, Y, Pivot, Scale, FileNameのみ機能する
- NameTextはキャラごとの1行目に必須。paramは非対応
- Pivotの数値指定は未対応

## Textureシート : 宴4に対する制限事項

- Label, Type, X, Y, Pivot, Scale, FileNameのみ機能する
- Typeでは、Bg, Spriteのみ対応
- Pivotの数値指定は未対応

## Soundシート : 宴4に対する制限事項

- Label, Type, FileNameのみ対応
- Typeでは、Se, Bgmのみ対応
- AdvRootのAllowsVoiceLabelをtrueにすると、SoundシートでVoiceタイプを認識し、使用できるようになります
    - この機能は宴4互換ではないため、デフォルトではfalseになっています

## Paramシート : 宴4に対する制限事項

- Label, Valueのみ機能する
- TypeはIntのみ固定
- 変数置換に string.Replace を使用しているため、変数名が部分一致で変換される危険があります。

## 宴4互換フォルダ構成

- Resources/ProjectName/Sound/BGM
- Resources/ProjectName/Sound/SE
- Resources/ProjectName/Sound/Voice
- Resources/ProjectName/Texture/BG
- Resources/ProjectName/Texture/Character
- Resources/ProjectName/Texture/Sprite

## シナリオとコマンド

### Scenarioシート

- WaitType, WindowTypeは非対応
- 宴レイヤー機能はなく、簡易実装
    - Layerシートはない。Bgは専用、SpriteとCharacterはそれぞれ(奥)0～7(手前)を指定する
    - SpriteとCharacterが同じレイヤー番号の場合、Characterが手前
    - 構造としては、Bg用のCanvasBg、SpriteやCharacter用のCanvasSprite、メッセージウィンドウなどのCanvasUIの3つのCanvasに分かれている
- TODO: Voice
- Skipの挙動が宴4と異なる。Wait全般がWaitを待つ

### コマンド詳細

コマンド         |カテゴリ|制限事項
-----------------|--------|--------------------------------------------------------
空欄(Arg1未指定) |Text    |PageCtrlはInput系3種とNextのみ対応。利用可能なタグはTMPが自動的に対応するもののみ
空欄(Arg1あり)   |Text    |レイヤーは0～7を指定する簡易実装。Arg1～Arg6でのparamタグ、Characterタグなどは、Arg2へのOffタグ以外は全て非対応。PageCtrlやText等はArg1未指定と同様の制限事項。Voiceは後述
CharacterOff     |Object  |Arg1はキャラクター指定のみ対応
Bg               |Object  |レイヤーに非対応
BgOff            |Object  |-
Sprite           |Object  |スプライト名とラベルに区別がなく、インスタンス化できない。レイヤーは0～7を指定する簡易実装
SpriteOff        |Object  |スプライト名とラベルに区別がなく、インスタンス化できない
Se               |Object  |ループに非対応。また、StopSeは未対応なため、止める手段がない
Bgm              |Sound   |-
StopBgm          |Sound   |-
Wait             |Effect  |-
FadeIn           |Effect  |カメラ、ルール画像・境界線フェード、アニメ指定は非対応
FadeOut          |Effect  |カメラ、ルール画像・境界線フェード、アニメ指定は非対応
ImageEffect      |Effect  |カメラ、アニメ指定は非対応。GrayScale、Sepiaのみ指定可能
ImageEffectOff   |Effect  |カメラ、アニメ指定は非対応。GrayScale、Sepiaのみ指定可能
ShowMessageWindow|UI      |-
HideMessageWindow|UI      |-
Param            |Logic   |宴4互換で使用可能な演算子が少ない。詳細後述。変数にも制限あり
Jump             |Logic   |宴4互換で使用可能な演算子が少ない。詳細後述。変数にも制限あり
Selection        |Logic   |プレハブ、X、Yの個別指定は非対応。宴と仕様が異なり、直前のテキスト内容は非表示にならない。また、選択肢を出した後にテキストをさらに出すことも出来ない

.

### Voiceについて

- デフォルトでは宴4互換でVoiceフォルダ以下からのファイル名のみ許容します
- AdvRootのAllowsVoiceLabelをtrueにすると、SoundシートでVoiceタイプを認識し、使用できるようになります
    - この機能は宴4互換ではないため、デフォルトではfalseになっています

### 宴4互換で使用可能な変数、演算子

変数について

- Paramシート定義はintのみ可能。なお計算式中は自動的にdouble相当になり、除算結果も小数になる。文字列等は非対応
- string.Replace を使用しているため、変数名が部分一致で変換される危険があります。

演算子について

- 四則演算   : 宴4に準ずる
- 比較演算子 : 宴4に準ずる
- 単項演算   : 非対応
- 括弧       : 宴4に準ずる
- 論理演算   : 宴4に準ずる
- 代入演算   : = のみ可能
- 組込み関数 : 全て不可

※ DataTable.Compute()を使用しているため、宴4にはない演算子やキーワードが解釈される場合があります。
いわゆる、SQLに登場するキーワード (AND, OR, NOT, LIKE, IN など) です。
後で宴4などに乗り換える想定がある場合は、使用しないほうが無難です。

### 補足

- 改ページ直後の演出コマンド時、自動的にWindowをHide。（宴準拠）
    - AdvRootのEmulatesUtageEffectCommandをfalseにすると、演出コマンドに対してWindowが非表示にならなくなります
- Text時、自動的にWindowをShow。（宴準拠）
*/

namespace Uft.AdvTools
{
    public class AdvRoot : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected bool _emulatesUtageEffectCommand = true; public bool EmulatesUtageEffectCommand => this._emulatesUtageEffectCommand;
        [SerializeField] protected bool _allowsVoiceLabel = false; public bool AllowsVoiceLabel => this._allowsVoiceLabel;

        [SerializeField] protected SimplePostEffectCollection _wideCameraSimplePostEffectCollection; public SimplePostEffectCollection WideCameraSimplePostEffectCollection => this._wideCameraSimplePostEffectCollection;
        public SimplePostEffectConfig WideCameraDirectionalGhostPostEffect
        {
            get
            {
                var index =  0;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }
        public SimplePostEffectConfig WideCameraGrayscalePostEffect
        {
            get
            {
                var index =  1;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }
        public SimplePostEffectConfig WideCameraSepiaPostEffect
        {
            get
            {
                var index =  2;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }

        [SerializeField] protected Bg _bg; public Bg Bg => this._bg;

        [SerializeField] protected SpriteManager _spriteManager; public SpriteManager SpriteManager => this._spriteManager;
        [SerializeField] protected SoundManager _soundManager; public SoundManager SoundManager => this._soundManager;
        [SerializeField] protected SelectionList _selectionList; public SelectionList SelectionList => this._selectionList;

        [SerializeField] protected LogController _logController; public bool LogControllerIsVisible => this._logController.gameObject.activeSelf;

        [SerializeField] protected Toggle _tglAutoNext;
        [SerializeField] protected Toggle _tglLogView;

        [SerializeField] protected string[] _cameraPrefixes = new[] { "WideCamera", "ACamera", "BCamera" };
        [SerializeField] protected string _uiEffectPrefix = "UIEffect_";
        [SerializeField] protected string _defaultParentName = "Stage"; public string DefaultParentName => this._defaultParentName;
        [SerializeField] protected FadeEffect _fadeEffect; public FadeEffect FadeEffect => this._fadeEffect;

        // Status

        public PostEffectManager PostEffectManager { get; protected set; }
        public AutoNext AutoNext { get; protected set; }
        public LogManager LogManager { get; protected set; }

        public string ResourcesFolderPathPart { get; protected set; }
        public string VoiceRoot => this.ResourcesFolderPathPart + "Sound/Voice/";

        public Dictionary<string, Character> CharacterDictionary { get; protected set; }
        public Dictionary<string, TextureRow> BgDictionary { get; protected set; }
        public Dictionary<string, TextureRow> SpriteDictionary { get; protected set; }

        public Dictionary<string, AudioClip> BgmDictionary { get; protected set; }
        public Dictionary<string, AudioClip> SeDictionary { get; protected set; }
        public Dictionary<string, AudioClip> VoiceDictionary { get; protected set; }

        public Dictionary<string, Param> ParamDictionary { get; protected set; }

        public ScenarioExecutor ScenarioExecutor { get; protected set; }
        public bool IsPausingScenario { get; protected set; } = false;
        public bool IsAutoInputOnce { get; set; } = false;
        public MessageArea MessageArea { get; protected set; } // NOTE: Setup() 自動検出
        // public Dictionary<string, (Camera, List<CinemachineCamera>)> CameraDictionary { get; protected set; } = new Dictionary<string, (Camera, List<CinemachineCamera>)>();
        public List<Animator> UiEffectList { get; protected set; } = new List<Animator>(); // NOTE: Setup() 自動検出
        public Dictionary<string, (Object, Animator, SpriteRenderer)> ObjectDictionary { get; protected set; } = new Dictionary<string, (Object, Animator, SpriteRenderer)>(); // NOTE: 調整中

        // Methods

        void Awake()
        {
            this._tglAutoNext.onValueChanged.AddListener((isOn) => this.ChangeAutoMode(isOn));
            this._tglLogView.onValueChanged.AddListener((isOn) => this.ChangeLogView(isOn));
        }

        protected virtual void Update()
        {
            if (this.ScenarioExecutor == null) return;
            if (this.IsPausingScenario) return;

            // IsAutoNextReady制御
            var isCountable = !this.MessageArea.IsTypewriting && !this.SoundManager.IsAnyVoicePlaying;
            this.AutoNext.UpdateFrame(isCountable, Time.deltaTime);

            this.ScenarioExecutor.UpdateFrame(this);
            if (!this.ScenarioExecutor.IsWaiting && this.ScenarioExecutor.IsWaitingForInput && !this.MessageArea.IsTypewriting)
            {
                if (this.IsAutoInputOnce)
                {
                    this.IsAutoInputOnce = false;
                    this.Next();
                }
                this.MessageArea.EnableImgNextSymbol();
            }
            else
            {
                this.MessageArea.DisableImgNextSymbol();
            }
        }

        public virtual void Setup(string scenarioCsvText, string characterCsvText, string textureCsvText, string soundCsvText, string paramCsvText, string resourcesFolderPathPart,
            ScenarioCsvLoader scenarioCsvLoader = null)
        {
            scenarioCsvLoader ??= new ScenarioCsvLoader();

            this.Cleanup();

            this.AutoNext = new AutoNext();
            this.LogManager = new LogManager();
            this._tglLogView.SetIsOnWithoutNotify(false);

            this.PostEffectManager = new PostEffectManager(this);

            this.ResourcesFolderPathPart = resourcesFolderPathPart;

            this.CharacterDictionary = new CharacterCsvLoader().Load(characterCsvText, resourcesFolderPathPart);

            var textures = new TextureCsvLoader().Load(textureCsvText, resourcesFolderPathPart);
            this.BgDictionary = textures._bgDict;
            this.SpriteDictionary = textures._spriteDict;

            var sounds = new SoundCsvLoader().Load(soundCsvText, resourcesFolderPathPart);
            this.BgmDictionary = sounds._bgmDict;
            this.SeDictionary = sounds._seDict;
            this.VoiceDictionary = sounds._voiceDict;

            this.ParamDictionary = new ParamCsvLoader().Load(paramCsvText);

            this.ScenarioExecutor = new ScenarioExecutor(scenarioCsvLoader.Load(scenarioCsvText, "test"));
            this._tglAutoNext.SetIsOnWithoutNotify(this.ScenarioExecutor.IsAutoNext);
            this.MessageArea = this.GetComponentInChildren<MessageArea>();
            foreach (var cameraPrefix in this._cameraPrefixes)
            {
                var camera =
                    this.GetComponentsInChildrenOrderByName<Camera>(component => component.gameObject.name.StartsWith(cameraPrefix))
                    .FirstOrDefault();
                if (camera != null)
                {
                    // var vCameraList = this.GetComponentsInChildrenOrderByName<CinemachineCamera>(component => component.gameObject.name.StartsWith(cameraPrefix));
                    // this.CameraDictionary.Add(camera.gameObject.name, (camera, vCameraList));
                }
            }
            this.UiEffectList = this.GetComponentsInChildrenOrderByName<Animator>(component => component.gameObject.name.StartsWith(this._uiEffectPrefix));
        }
        public virtual void Cleanup()
        {
            this.PostEffectManager = null;
            this.ScenarioExecutor = null;
            this.IsPausingScenario = false;
            this.MessageArea = null;
            // this.CameraDictionary.Clear();
            this.ObjectDictionary.Clear();
            this.UiEffectList.Clear();
        }

        public virtual void PauseScenario() => this.IsPausingScenario = true;
        public virtual void ResumeScenario() => this.IsPausingScenario = false;

        public virtual void ChangeAutoMode(bool isOn)
        {
            this.ScenarioExecutor.IsAutoNext = isOn;
            this._tglAutoNext.SetIsOnWithoutNotify(isOn);
        }

        public virtual void ChangeLogView(bool isOn)
        {
            if (isOn)
            {
                this.ChangeAutoMode(false); // NOTE: Logを表示したら自動的にAutoNextをオフにする
                _ = this._logController.ShowAsync(this.LogManager.LogItemList);
            }
            else
            {
                _ = this._logController.CloseAsync();
            }

            this._tglLogView.SetIsOnWithoutNotify(isOn);
        }

        public virtual void Next(bool playsNextSound = true)
        {
            if (this.MessageArea.IsTypewriting)
            {
                this.MessageArea.EndTypewriting();
                return;
            }
            this.ScenarioExecutor.IsWaitingForInput = false;
        }

        public virtual void HideUI()
        {
            this.MessageArea.Hide();
            this._tglAutoNext.gameObject.SetActive(false);
            this._tglLogView.gameObject.SetActive(false);
        }

        public virtual void ShowUI()
        {
            this.MessageArea.Show();
            this._tglAutoNext.gameObject.SetActive(true);
            this._tglLogView.gameObject.SetActive(true);
        }

        public virtual void SetText(Character character, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            var lastPageCtrl = this.MessageArea.LastPageCtrl;
            this.LogManager.Add(lastPageCtrl, character, name, text);
            this.MessageArea.SetText(this, name, text, pageCtrl, windowType);
        }
    }
}
