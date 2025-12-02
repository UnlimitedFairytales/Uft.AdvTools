using System.Collections.Generic;
using System.Linq;
using Uft.AdvTools.Entities;
using Uft.AdvTools.Loader;
using Uft.AdvTools.View;
using Uft.FadeEffects;
using Uft.UnityUtils;
using UnityEngine;

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
    - 構造としては、Bg用のUIBg、SpriteやCharacter用のUISprite、メッセージウィンドウのUIの3つのCanvasに分かれている
- TODO: Voice
- Skipの挙動が宴4と異なる。Wait全般がWaitを待つ

### コマンド詳細

コマンド         |カテゴリ|制限事項
-----------------|--------|--------------------------------------------------------
空欄(Arg1未指定) |Text    |PageCtrlはInput系3種とNextのみ対応。利用可能なタグはTMPが自動的に対応するもののみ
空欄(Arg1あり)   |Text    |レイヤーは0～7を指定する簡易実装。Arg1～Arg6でのparamタグ、Characterタグなどは、Arg2へのOffタグ以外は全て非対応。PageCtrlやText等はArg1未指定と同様の制限事項
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
ShowMessageWindow|UI      |-
HideMessageWindow|UI      |-
Param            |Logic   |宴4互換で使用可能な演算子が少ない。詳細後述。変数にも制限あり
Jump             |Logic   |宴4互換で使用可能な演算子が少ない。詳細後述。変数にも制限あり
Selection        |Logic   |プレハブ、X、Yの個別指定は非対応。宴と仕様が異なり、直前のテキスト内容は非表示にならない。また、選択肢を出した後にテキストをさらに出すことも出来ない

.

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
- Text時、自動的にWindowをShow。（宴準拠）
*/

namespace Uft.AdvTools
{
    public class AdvRoot : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected bool _emulatesUtageEffectCommand = true; public bool EmulatesUtageEffectCommand => this._emulatesUtageEffectCommand;

        [SerializeField] protected Bg _bg; public Bg Bg => this._bg;

        [SerializeField] protected SpriteManager _spriteManager; public SpriteManager SpriteManager => this._spriteManager;
        [SerializeField] protected SoundManager _soundManager; public SoundManager SoundManager => this._soundManager;
        [SerializeField] protected SelectionList _selectionList; public SelectionList SelectionList => this._selectionList;

        [SerializeField] protected string[] _cameraPrefixes = new[] { "WideCamera", "ACamera", "BCamera" };
        [SerializeField] protected string _uiEffectPrefix = "UIEffect_";
        [SerializeField] protected string _defaultParentName = "Stage"; public string DefaultParentName => this._defaultParentName;
        [SerializeField] protected FadeEffect _fadeEffect; public FadeEffect FadeEffect => this._fadeEffect;

        // Status

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

        protected virtual void Update()
        {
            if (this.ScenarioExecutor == null) return;
            if (this.IsPausingScenario) return;

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

        public virtual void Setup(string scenarioCsvText, string characterCsvText, string textureCsvText, string soundCsvText, string paramCsvText, string resourcesFolderPathPart)
        {
            this.Cleanup();

            this.CharacterDictionary = new CharacterCsvLoader().Load(characterCsvText, resourcesFolderPathPart);

            var textures = new TextureCsvLoader().Load(textureCsvText, resourcesFolderPathPart);
            this.BgDictionary = textures._bgDict;
            this.SpriteDictionary = textures._spriteDict;

            var sounds = new SoundCsvLoader().Load(soundCsvText, resourcesFolderPathPart);
            this.BgmDictionary = sounds._bgmDict;
            this.SeDictionary = sounds._seDict;
            this.VoiceDictionary = sounds._voiceDict;

            this.ParamDictionary = new ParamCsvLoader().Load(paramCsvText);

            this.ScenarioExecutor = new ScenarioExecutor(new ScenarioCsvLoader().Load(scenarioCsvText, "test"));
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
            this.ScenarioExecutor = null;
            this.IsPausingScenario = false;
            this.MessageArea = null;
            // this.CameraDictionary.Clear();
            this.ObjectDictionary.Clear();
            this.UiEffectList.Clear();
        }

        public virtual void PauseScenario() => this.IsPausingScenario = true;
        public virtual void ResumeScenario() => this.IsPausingScenario = false;

        public virtual void Next()
        {
            if (this.MessageArea.IsTypewriting)
            {
                this.MessageArea.EndTypewriting();
                return;
            }
            this.ScenarioExecutor.IsWaitingForInput = false;
        }
    }
}
