using System.Collections.Generic;
using Uft.AdvTools.Loader;
using Uft.UnityUtils.Asset;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Uft.AdvTools.Samples
{
    [DefaultExecutionOrder(-1)]
    public class SampleDirector : MonoBehaviour
    {
        [SerializeField] bool _isScriptableObject = false;

        [Header("CSV")]
        [SerializeField] TextAsset _scenarioFile;
        [SerializeField] TextAsset _characterFile;
        [SerializeField] TextAsset _textureFile;
        [SerializeField] TextAsset _soundFile;
        [SerializeField] TextAsset _paramFile;

        [Header("ScriptableObject")]
        [SerializeField] ScenarioTableSO _scenarioSO;
        [SerializeField] CharacterTableSO _characterSO;
        [SerializeField] TextureTableSO _textureSO;
        [SerializeField] SoundTableSO _soundSO;
        [SerializeField] ParamTableSO _paramSO;

        [SerializeField] string _resourcesFolderPathPart;
        [SerializeField] AdvRoot _advRoot;

        // NOTE: PauseScenario制御サンプル
        [SerializeField] SampleMessageBox _sampleMessageBox;

        bool _isInitialized = false;
        bool _lastPauseState = false;

        void Awake()
        {
            this._sampleMessageBox.OnClickResume = () => this._advRoot.ScenarioExecutor.ResumeScenario();
        }

        void Update()
        {
            this._advRoot.InputProxy.Update();

            // HACK: シーン配置ActiveオブジェクトStart済み保証タイミング
            if (!this._isInitialized)
            {
                this._isInitialized = true;

                if (this._advRoot != null)
                {
                    if (this._isScriptableObject)
                        this._advRoot.Setup(this._scenarioSO, this._characterSO, this._textureSO, this._soundSO, this._paramSO, new AssetLoadProxy(null), this._resourcesFolderPathPart);
                    else
                        this._advRoot.Setup(this._scenarioFile.text, this._characterFile.text, this._textureFile.text, this._soundFile.text, this._paramFile.text, new AssetLoadProxy(null), this._resourcesFolderPathPart);
                    this._advRoot.ScenarioExecutor.ResumeScenario();
                    this._lastPauseState = this._advRoot.ScenarioExecutor.IsPauseScenario;
                }
            }

            if (Input.GetKeyUp(KeyCode.Z) || Input.GetButtonUp("Submit") || (Input.GetMouseButtonUp(0) && !this.IsPointerOverUI()))
            {
                this._advRoot.Next();
            }

            // NOTE: PauseScenario制御サンプル
            if (this._lastPauseState != this._advRoot.ScenarioExecutor.IsPauseScenario)
            {
                if (this._advRoot.ScenarioExecutor.IsPauseScenario)
                {
                    this._sampleMessageBox.Show();
                }
                else
                {
                    this._sampleMessageBox.Hide();
                }
            }
            this._lastPauseState = this._advRoot.ScenarioExecutor.IsPauseScenario;
        }

        bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition,
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}
