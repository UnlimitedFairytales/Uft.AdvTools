using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Uft.AdvTools.Samples
{
    public class SampleDirector : MonoBehaviour
    {
        [SerializeField] TextAsset _scenarioFile;
        [SerializeField] TextAsset _characterFile;
        [SerializeField] TextAsset _textureFile;
        [SerializeField] TextAsset _soundFile;
        [SerializeField] TextAsset _paramFile;
        [SerializeField] string _resourcesFolderPathPart;
        [SerializeField] AdvRoot _advRoot;

        void Start()
        {
            if (this._advRoot != null)
            {
                this._advRoot.Setup(
                    this._scenarioFile.text,
                    this._characterFile.text,
                    this._textureFile.text,
                    this._soundFile.text,
                    this._paramFile.text,
                    this._resourcesFolderPathPart);
                this._advRoot.ResumeScenario();
            }
        }

        void Update()
        {
            if (Input.GetButtonUp("Submit") || (Input.GetMouseButtonUp(0) && !this.IsPointerOverUI()))
            {
                this._advRoot.Next();
            }
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
