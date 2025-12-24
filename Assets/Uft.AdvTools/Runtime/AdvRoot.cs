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
        public SimplePostEffectConfig WideCameraRuleFadePostEffect
        {
            get
            {
                var index =  3;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }

        [SerializeField] protected Bg _bg; public Bg Bg => this._bg;

        [SerializeField] protected SpriteManager _spriteManager; public SpriteManager SpriteManager => this._spriteManager;
        [SerializeField] protected SoundManager _soundManager; public SoundManager SoundManager => this._soundManager;
        [SerializeField] protected PostEffectManager _postEffectManager; public PostEffectManager PostEffectManager => this._postEffectManager;
        [SerializeField] protected SelectionList _selectionList; public SelectionList SelectionList => this._selectionList;

        [SerializeField] protected LogController _logController; public bool LogControllerIsVisible => this._logController.gameObject.activeSelf;

        [SerializeField] protected Toggle _tglAutoNext;
        [SerializeField] protected Toggle _tglLogView;

        [SerializeField] protected string[] _cameraPrefixes = new[] { "WideCamera", "ACamera", "BCamera" };
        [SerializeField] protected string _uiEffectPrefix = "UIEffect_";
        [SerializeField] protected string _defaultParentName = "Stage"; public string DefaultParentName => this._defaultParentName;
        [SerializeField] protected FadeEffect _fadeEffect; public FadeEffect FadeEffect => this._fadeEffect;

        // Status

        public MessageWindowManager MessageWindowManager { get; protected set; }
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

        public virtual void Setup(string scenarioCsvText, string characterCsvText, string textureCsvText, string soundCsvText, string paramCsvText, string resourcesFolderPathPart,
            ScenarioCsvLoader scenarioCsvLoader = null)
        {
            scenarioCsvLoader ??= new ScenarioCsvLoader();

            this.Cleanup();

            this.MessageWindowManager = new MessageWindowManager();
            this.AutoNext = new AutoNext();
            this.LogManager = new LogManager();
            this._tglLogView.SetIsOnWithoutNotify(false);

            this.PostEffectManager.Setup(this);

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
            this.MessageWindowManager.Setup(this.GetComponentsInChildren<MessageWindow>(true));
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
            this.UiEffectList = this.GetComponentsInChildrenOrderByName<Animator>(true,component => component.gameObject.name.StartsWith(this._uiEffectPrefix));
        }
        public virtual void Cleanup()
        {
            this.ScenarioExecutor = null;
            this.IsPausingScenario = false;
            this.MessageWindowManager?.Cleanup();
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
            if (this.MessageWindowManager.CurrentMessageWindow.IsTypewriting)
            {
                this.MessageWindowManager.CurrentMessageWindow.EndTypewriting();
                return;
            }
            this.ScenarioExecutor.IsWaitingForInput = false;
        }

        public virtual void HideUI()
        {
            this.MessageWindowManager.CurrentMessageWindow.Hide();
            this._tglAutoNext.gameObject.SetActive(false);
            this._tglLogView.gameObject.SetActive(false);
        }

        public virtual void ShowUI()
        {
            this.MessageWindowManager.CurrentMessageWindow.Show();
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
