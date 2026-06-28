#nullable enable

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;
using Uft.AdvTools.Loader;
using Uft.AdvTools.View;
using Uft.FadeEffects;
using Uft.UnityUtils;
using Uft.UnityUtils.Asset;
using Uft.UnityUtils.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools
{
    /// <summary>
    /// 選択肢が開く瞬間の把握は、<see cref="ScenarioExecutor.OnSelectionShowing"/><br/>
    /// 選択肢が閉じる瞬間の把握は、<see cref="ScenarioExecutor.OnSelectionHidden"/><br/>
    /// ログが開く瞬間の把握は、<see cref="OnLogShowing"/><br/>
    /// ログが閉じきった後の把握は、<see cref="OnLogHidden"/><br/>
    /// </summary>
    public class AdvRoot : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected bool _emulatesUtageEffectCommand = true; public bool EmulatesUtageEffectCommand => this._emulatesUtageEffectCommand;
        [SerializeField] protected bool _allowsVoiceLabel = false; public bool AllowsVoiceLabel => this._allowsVoiceLabel;

        [SerializeField] protected Bg _bg = null!; public Bg Bg => this._bg;

        [SerializeField] protected SpriteManager _spriteManager = null!; public SpriteManager SpriteManager => this._spriteManager;
        [SerializeField] protected SoundManager _soundManager = null!; public SoundManager SoundManager => this._soundManager;
        [SerializeField] protected PostEffectManager _postEffectManager = null!; public PostEffectManager PostEffectManager => this._postEffectManager;
        [SerializeField] protected SelectionListView _selectionListView = null!; public SelectionListView SelectionListView => this._selectionListView;

        [SerializeField] protected LogController _logController = null!; public bool LogControllerIsVisible => this._logController.gameObject.activeSelf;

        [SerializeField] protected Toggle _tglAutoNext = null!;
        [SerializeField] protected Toggle _tglLogView = null!;

        [SerializeField] protected string[] _cameraPrefixes = new[] { "WideCamera", "ACamera", "BCamera" };
        [SerializeField] protected string _uiEffectPrefix = "UIEffect_";
        [SerializeField] protected string _defaultParentName = "Stage"; public string DefaultParentName => this._defaultParentName;
        [SerializeField] protected FadeEffect _fadeEffect = null!; public FadeEffect FadeEffect => this._fadeEffect;

        // Status

        public IInputProxy InputProxy = new SimpleInput();

        public Action? OnLogShowing { get; set; }
        public Action? OnLogHidden { get; set; }

        public AssetLoadProxy AssetLoadProxy { get; protected set; } = null!;
        public string ResourcesFolderPathPart { get; protected set; } = null!;
        public string VoiceRoot => this.ResourcesFolderPathPart + "Sound/Voice/";

        public MessageWindowManager MessageWindowManager { get; protected set; } = null!;
        public AutoNext AutoNext { get; protected set; } = null!;
        public LogManager LogManager { get; protected set; } = null!;

        public Dictionary<string, Character> CharacterDictionary { get; protected set; } = null!;
        public Dictionary<string, TextureRow> BgDictionary { get; protected set; } = null!;
        public Dictionary<string, TextureRow> SpriteDictionary { get; protected set; } = null!;
        public Dictionary<string, string> BgmPathDictionary { get; protected set; } = null!;
        public Dictionary<string, string> SePathDictionary { get; protected set; } = null!;
        public Dictionary<string, string> VoicePathDictionary { get; protected set; } = null!;
        public Dictionary<string, Param> ParamDictionary { get; protected set; } = null!;

        public ScenarioExecutor? ScenarioExecutor { get; protected set; }
        public bool IsAutoInputOnce { get; set; } = false;
        // public Dictionary<string, (Camera, List<CinemachineCamera>)> CameraDictionary { get; protected set; } = new Dictionary<string, (Camera, List<CinemachineCamera>)>();
        public List<Animator> UiEffectList { get; protected set; } = null!; // NOTE: Setup() 自動検出
        public Dictionary<string, (UnityEngine.Object, Animator, SpriteRenderer)> ObjectDictionary { get; protected set; } = new Dictionary<string, (UnityEngine.Object, Animator, SpriteRenderer)>(); // NOTE: 調整中

        // Methods

        protected virtual void Awake()
        {
            this._tglAutoNext.onValueChanged.AddListener((isOn) => this.ChangeAutoMode(isOn));
            this._tglLogView.onValueChanged.AddListener((isOn) => this.ChangeLogView(isOn));
        }

        protected virtual void Update()
        {
            if (this.ScenarioExecutor == null) return;
            if (this.ScenarioExecutor.IsPauseScenario) return;

            // IsAutoNextReady制御
            var w = this.MessageWindowManager.CurrentMessageWindow;
            var isCountable = !(w != null && w.IsTypewriting) && !this.SoundManager.IsAnyVoicePlaying;
            this.AutoNext.UpdateFrame(isCountable, Time.deltaTime);

            this.ScenarioExecutor.UpdateFrame(this);
            if (!this.ScenarioExecutor.IsWaiting && this.ScenarioExecutor.IsWaitingForInput && !(w != null && w.IsTypewriting))
            {
                if (this.IsAutoInputOnce)
                {
                    this.IsAutoInputOnce = false;
                    this.Next();
                }
                if (w != null) w.EnableImgNextSymbol();
            }
            else
            {
                if (w != null) w.DisableImgNextSymbol();
            }
        }

        public virtual void Setup(string scenarioCsvText, string characterCsvText, string textureCsvText, string soundCsvText, string paramCsvText, AssetLoadProxy assetLoadProxy, string resourcesFolderPathPart,
            ScenarioCsvLoader? scenarioCsvLoader = null, string? defaultMessageWindowName = null)
        {
            this.Cleanup();
            scenarioCsvLoader ??= new ScenarioCsvLoader();
            this.SetupInner(
                assetLoadProxy,
                resourcesFolderPathPart,
                new CharacterCsvLoader(assetLoadProxy).Load(characterCsvText, resourcesFolderPathPart),
                new TextureCsvLoader(assetLoadProxy).Load(textureCsvText, resourcesFolderPathPart),
                new SoundCsvLoader().Load(soundCsvText, resourcesFolderPathPart),
                new ParamCsvLoader().Load(paramCsvText),
                new ScenarioExecutor(scenarioCsvLoader.Load(scenarioCsvText, "test")),
                defaultMessageWindowName);
        }

        public virtual void Setup(ScenarioTableSO scenarioSO, CharacterTableSO characterSO, TextureTableSO textureSO, SoundTableSO soundSO, ParamTableSO paramSO,
            AssetLoadProxy assetLoadProxy, string resourcesFolderPathPart,
            ScenarioCsvLoader? scenarioCsvLoader = null, string? defaultMessageWindowName = null)
        {
            this.Cleanup();
            scenarioCsvLoader ??= new ScenarioCsvLoader();
            this.SetupInner(
                assetLoadProxy,
                resourcesFolderPathPart,
                new CharacterCsvLoader(assetLoadProxy).Load(characterSO),
                new TextureCsvLoader(assetLoadProxy).Load(textureSO, resourcesFolderPathPart),
                new SoundCsvLoader().Load(soundSO, resourcesFolderPathPart),
                new ParamCsvLoader().Load(paramSO),
                new ScenarioExecutor(scenarioCsvLoader.Load(scenarioSO, "test")),
                defaultMessageWindowName);
        }

        protected virtual void SetupInner(
            AssetLoadProxy assetLoadProxy,
            string resourcesFolderPathPart,
            Dictionary<string, Character> characterDict,
            TextureCsvLoader.TextureDictionaries textures,
            SoundCsvLoader.SoundDictionaries sounds,
            Dictionary<string, Param> paramDict,
            ScenarioExecutor scenarioExecutor,
            string? defaultMessageWindowName)
        {
            this.AssetLoadProxy = assetLoadProxy;
            this.ResourcesFolderPathPart = resourcesFolderPathPart;

            this.MessageWindowManager = new MessageWindowManager();
            this.AutoNext = new AutoNext();
            this.LogManager = new LogManager();
            this._tglLogView.SetIsOnWithoutNotify(false);
            this.PostEffectManager.Setup(this);
            this.SoundManager.Setup(new Dictionary<string, AudioInfo>(), assetLoadProxy);

            this.CharacterDictionary = characterDict;
            this.BgDictionary = textures._bgDict;
            this.SpriteDictionary = textures._spriteDict;
            this.BgmPathDictionary = sounds._bgmPathDict;
            this.SePathDictionary = sounds._sePathDict;
            this.VoicePathDictionary = sounds._voicePathDict;
            this.ParamDictionary = paramDict;

            this.ScenarioExecutor = scenarioExecutor;
            this._tglAutoNext.SetIsOnWithoutNotify(scenarioExecutor.IsAutoNext);
            this.MessageWindowManager.Setup(this.GetComponentsInChildren<MessageWindow>(true), defaultMessageWindowName);

            foreach (var cameraPrefix in this._cameraPrefixes)
            {
                var camera = this.GetComponentsInChildrenOrderByName<Camera>(true, c => c.gameObject.name.StartsWith(cameraPrefix)).FirstOrDefault();
                if (camera != null) { }
            }
            this.UiEffectList = this.GetComponentsInChildrenOrderByName<Animator>(true, c => c.gameObject.name.StartsWith(this._uiEffectPrefix));
            this.HideUI(0);
        }

        public virtual void Cleanup()
        {
            this.ScenarioExecutor = null;
            this.MessageWindowManager?.Cleanup();
            // this.CameraDictionary.Clear();
            this.ObjectDictionary.Clear();
            this.UiEffectList?.Clear();
        }

        public virtual void ChangeAutoMode(bool isOn)
        {
            if (this.ScenarioExecutor == null) return;

            this.ScenarioExecutor.IsAutoNext = isOn;
            this._tglAutoNext.SetIsOnWithoutNotify(isOn);
        }

        public virtual void ChangeLogView(bool isOn)
        {
            var ct = this.destroyCancellationToken;
            if (isOn)
            {
                this.ChangeAutoMode(false); // NOTE: Logを表示したら自動的にAutoNextをオフにする
                this.OnLogShowing?.Invoke();
                this._logController.ShowAsync(ct, this.LogManager.LogItemList).Forget();
            }
            else
            {
                UniTask.Void(async () =>
                {
                    await this._logController.CloseAsync(ct);
                    this.OnLogHidden?.Invoke();
                });
            }

            this._tglLogView.SetIsOnWithoutNotify(isOn);
        }

        public virtual void Next(bool playsNextSound = true)
        {
            var w = this.MessageWindowManager.CurrentMessageWindow;
            if (w != null && w.IsTypewriting)
            {
                w.EndTypewriting();
                return;
            }

            if (this.ScenarioExecutor == null) return;
            this.ScenarioExecutor.IsWaitingForInput = false;
        }

        public virtual void HideUI(float fadeSeconds)
        {
            var w = this.MessageWindowManager.CurrentMessageWindow;
            if (w != null) w.Hide(fadeSeconds);
            this._tglAutoNext.gameObject.SetActive(false);
            this._tglLogView.gameObject.SetActive(false);
        }

        public virtual void ShowUI(float fadeSeconds)
        {
            var w = this.MessageWindowManager.CurrentMessageWindow;
            if (w != null) w.Show(fadeSeconds);
            this._tglAutoNext.gameObject.SetActive(true);
            this._tglLogView.gameObject.SetActive(true);
        }

        public virtual void SetText(Character? character, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            var w = this.MessageWindowManager.CurrentMessageWindow;
            var lastPageCtrl = w != null ? w.LastPageCtrl : CmdText.PageCtrlType.InputBrPageAndNoHide;
            this.LogManager.Add(lastPageCtrl, character, name, text);
            if (w != null) w.SetText(this, name, text, pageCtrl, windowType);
            this.SpriteManager.ControlCharacterGrayout(character, !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text));
        }
    }
}
