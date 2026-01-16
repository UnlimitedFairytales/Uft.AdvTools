using System;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.Samples
{
    public class SampleMessageBox : MonoBehaviour
    {
        [SerializeField] Button _btnResume;

        public Action OnClickResume;

        void Awake()
        {
            this._btnResume.onClick.AddListener(() => this.OnClickResume());
            this.gameObject.SetActive(false);
        }

        public void Show() => this.gameObject.SetActive(true);
        public void Hide() => this.gameObject.SetActive(false);
    }
}
