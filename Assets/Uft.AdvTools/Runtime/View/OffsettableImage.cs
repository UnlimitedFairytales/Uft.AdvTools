// #nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    [RequireComponent(typeof(RectTransform))]
    public class OffsettableImage : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected Image _image; public Image Image => this._image;

        public RectTransform RootRectTransform => (RectTransform)this.transform;
        public bool IsOn { get; set; } = false;

        // Unity events

        void Reset()
        {
            this._image = this.GetComponentInChildren<Image>();
        }
    }
}
