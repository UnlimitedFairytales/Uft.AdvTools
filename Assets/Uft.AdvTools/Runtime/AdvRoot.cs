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

        [SerializeField] protected Bg _bg; public Bg Bg => this._bg;

        [SerializeField] protected SpriteManager _spriteManager; public SpriteManager SpriteManager => this._spriteManager;
        [SerializeField] protected SoundManager _soundManager; public SoundManager SoundManager => this._soundManager;
        [SerializeField] protected PostEffectManager _postEffectManager; public PostEffectManager PostEffectManager => this._postEffectManager;
        [SerializeField] protected SelectionListView _selectionListView; public SelectionListView SelectionListView => this._selectionListView;

        [SerializeField] protected LogController _logController; public bool LogControllerIsVisible => this._logController.gameObject.activeSelf;

        [SerializeField] protected Toggle _tglAutoNext;
        [SerializeField] protected Toggle _tglLogView;

        [SerializeField] protected string[] _cameraPrefixes = new[] { "WideCamera", "ACamera", "BCamera" };
        [SerializeField] protected string _uiEffectPrefix = "UIEffect_";
        [SerializeField] protected string _defaultParentName = "Stage"; public string DefaultParentName => this._defaultParentName;
        [SerializeField] protected FadeEffect _fadeEffect; public FadeEffect FadeEffect => this._fadeEffect;

        // Status

        public IInputProxy InputProxy = new SimpleInput();

        public Action OnLogShowing { get; set; }
        public Action OnLogHidden { get; set; }

        public AssetLoadProxy AssetLoadProxy { get; protected set; }
        public string ResourcesFolderPathPart { get; protected set; }
        public string VoiceRoot => this.ResourcesFolderPathPart + "Sound/Voice/";

        public MessageWindowManager MessageWindowManager { get; protected set; }
        public AutoNext AutoNext { get; protected set; }
        public LogManager LogManager { get; protected set; }

        public Dictionary<string, Character> CharacterDictionary { get; protected set; }
        public Dictionary<string, TextureRow> BgDictionary { get; protected set; }
        public Dictionary<string, TextureRow> SpriteDictionary { get; protected set; }

        public Dictionary<string, AudioClip> BgmDictionary { get; protected set; }
        public Dictionary<string, AudioClip> SeDictionary { get; protected set; }
        public Dictionary<string, AudioClip> VoiceDictionary { get; protected set; }

        public Dictionary<string, Param> ParamDictionary { get; protected set; }

        public ScenarioExecutor ScenarioExecutor { get; protected set; }
        public bool IsAutoInputOnce { get; set; } = false;
        // public Dictionary<string, (Camera, List<CinemachineCamera>)> CameraDictionary { get; protected set; } = new Dictionary<string, (Camera, List<CinemachineCamera>)>();
        public List<Animator> UiEffectList { get; protected set; } = new List<Animator>(); // NOTE: Setup() 自動検出
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
            var isCountable = !this.MessageWindowManager.CurrentMessageWindow.IsTypewriting && !this.SoundManager.IsAnyVoicePlaying;
            this.AutoNext.UpdateFrame(isCountable, Time.deltaTime);

            this.ScenarioExecutor.UpdateFrame(this);
            if (!this.ScenarioExecutor.IsWaiting && this.ScenarioExecutor.IsWaitingForInput && !this.MessageWindowManager.CurrentMessageWindow.IsTypewriting)
            {
                if (this.IsAutoInputOnce)
                {
                    this.IsAutoInputOnce = false;
                    this.Next();
                }
                this.MessageWindowManager.CurrentMessageWindow.EnableImgNextSymbol();
            }
            else
            {
                this.MessageWindowManager.CurrentMessageWindow.DisableImgNextSymbol();
            }
        }

        public virtual void Setup(string scenarioCsvText, string characterCsvText, string textureCsvText, string soundCsvText, string paramCsvText, AssetLoadProxy assetLoadProxy, string resourcesFolderPathPart,
            ScenarioCsvLoader scenarioCsvLoader = null, string defaultMessageWindowName = null)
        {
            this.Cleanup();

            this.AssetLoadProxy = assetLoadProxy;
            this.ResourcesFolderPathPart = resourcesFolderPathPart;
            scenarioCsvLoader ??= new ScenarioCsvLoader(assetLoadProxy);

            this.MessageWindowManager = new MessageWindowManager();
            this.AutoNext = new AutoNext();
            this.LogManager = new LogManager();
            this._tglLogView.SetIsOnWithoutNotify(false);

            this.PostEffectManager.Setup(this);

            this.CharacterDictionary = new CharacterCsvLoader(assetLoadProxy).Load(characterCsvText, resourcesFolderPathPart);

            var textures = new TextureCsvLoader(assetLoadProxy).Load(textureCsvText, resourcesFolderPathPart);
            this.BgDictionary = textures._bgDict;
            this.SpriteDictionary = textures._spriteDict;

            var sounds = new SoundCsvLoader(assetLoadProxy).Load(soundCsvText, resourcesFolderPathPart);
            this.BgmDictionary = sounds._bgmDict;
            this.SeDictionary = sounds._seDict;
            this.VoiceDictionary = sounds._voiceDict;

            this.ParamDictionary = new ParamCsvLoader().Load(paramCsvText);

            this.ScenarioExecutor = new ScenarioExecutor(scenarioCsvLoader.Load(scenarioCsvText, "test"));
            this._tglAutoNext.SetIsOnWithoutNotify(this.ScenarioExecutor.IsAutoNext);
            this.MessageWindowManager.Setup(this.GetComponentsInChildren<MessageWindow>(true), defaultMessageWindowName);
            foreach (var cameraPrefix in this._cameraPrefixes)
            {
                var camera =
                    this.GetComponentsInChildrenOrderByName<Camera>(true, component => component.gameObject.name.StartsWith(cameraPrefix))
                    .FirstOrDefault();
                if (camera != null)
                {
                    // var vCameraList = this.GetComponentsInChildrenOrderByName<CinemachineCamera>(component => component.gameObject.name.StartsWith(cameraPrefix));
                    // this.CameraDictionary.Add(camera.gameObject.name, (camera, vCameraList));
                }
            }
            this.UiEffectList = this.GetComponentsInChildrenOrderByName<Animator>(true, component => component.gameObject.name.StartsWith(this._uiEffectPrefix));
            this.HideUI(0);
        }
        public virtual void Cleanup()
        {
            this.ScenarioExecutor = null;
            this.MessageWindowManager?.Cleanup();
            // this.CameraDictionary.Clear();
            this.ObjectDictionary.Clear();
            this.UiEffectList.Clear();
        }

        public virtual void ChangeAutoMode(bool isOn)
        {
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
            if (this.MessageWindowManager.CurrentMessageWindow.IsTypewriting)
            {
                this.MessageWindowManager.CurrentMessageWindow.EndTypewriting();
                return;
            }
            this.ScenarioExecutor.IsWaitingForInput = false;
        }

        public virtual void HideUI(float fadeSeconds)
        {
            this.MessageWindowManager.CurrentMessageWindow.Hide(fadeSeconds);
            this._tglAutoNext.gameObject.SetActive(false);
            this._tglLogView.gameObject.SetActive(false);
        }

        public virtual void ShowUI(float fadeSeconds)
        {
            this.MessageWindowManager.CurrentMessageWindow.Show(fadeSeconds);
            this._tglAutoNext.gameObject.SetActive(true);
            this._tglLogView.gameObject.SetActive(true);
        }

        public virtual void SetText(Character character, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            var lastPageCtrl = this.MessageWindowManager.CurrentMessageWindow.LastPageCtrl;
            this.LogManager.Add(lastPageCtrl, character, name, text);
            this.MessageWindowManager.CurrentMessageWindow.SetText(this, name, text, pageCtrl, windowType);
            this.SpriteManager.ControlCharacterGrayout(character, !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text));
        }
    }
}
