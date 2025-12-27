#nullable enable

using DG.Tweening;
using System;
using Uft.UnityUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    /// <summary>2DPrefabの場合は、LateUpdate（=Animator適用後）にCanvasGroupのalpha値を上書きします。</summary>
    [RequireComponent(typeof(RectTransform))]
    public class OffsettableImage : MonoBehaviour
    {
        const string NAME = "[" + nameof(OffsettableImage) + "]";

        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => this._canvasGroup!; // NOTE: null-forgiving（警告抑制のみ）。他のメンバで必須チェック済み。
        [SerializeField] protected Image? _image; public Image Image => this._image!; // NOTE: null-forgiving（警告抑制のみ）。他のメンバで必須チェック済み。

        public RectTransform RootRectTransform => (RectTransform)this.transform;

        // Status

        public bool IsOn { get; set; } = false;
        public object? KeyObject { get; protected set; }
        public T? CastedKey<T>() where T : class => this.KeyObject as T;
        public GameObject? Instantiated { get; protected set; }
        public SpriteRenderer[]? SpriteRenderers { get; protected set; }

        // Unity events

        void Reset()
        {
            this._canvasGroup = this.GetComponentInChildren<CanvasGroup>(true);
            this._image = this.GetComponentInChildren<Image>(true);
        }

        public virtual void LateUpdate()
        {
            if (this.SpriteRenderers == null || this._canvasGroup == null) return;

            var canvasGroupAlpha = this._canvasGroup.alpha;
            foreach (var renderer in this.SpriteRenderers)
            {
                if (renderer == null) continue;

                if (!renderer.transform.IsChildOf(this._canvasGroup.transform)) continue;
                var c = renderer.color;
                if (!Mathf.Approximately(c.a, canvasGroupAlpha))
                {
                    c.a = canvasGroupAlpha;
                    renderer.color = c;
                }
            }
        }

        // Methods

        // NOTE: 同一keyObjectの場合、SpriteとGameObjectは混在しない想定
        public virtual void Set<T>(bool isOn, T? keyObject, Sprite? sprite,
            Vector2 pivot, float scale, Vector2 position, Vector2 offset, float fromAlpha, float toAlpha, float fadeTime_sec)
            where T : class
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this.IsOn = isOn;
            var img = this._image;
            var cg = this._canvasGroup;

            cg.DOComplete();

            if (!Equals(keyObject, this.KeyObject))
            {
                cg.alpha = 0;
                if (this.Instantiated != null && this.Instantiated.transform.IsChildOf(cg.transform))
                {
                    this.Instantiated.SetActive(false);
                }
                this.Instantiated = null;
                this.SpriteRenderers = null;
                img.sprite = null;
            }
            this.KeyObject = keyObject;

            img.sprite = sprite;
            img.rectTransform.pivot = pivot;
            if (sprite != null) img.SetNativeSize();
            img.rectTransform.localScale = new Vector3(scale, scale, scale);
            this.RootRectTransform.anchoredPosition = position;
            img.rectTransform.anchoredPosition = offset;

            if (0 < fadeTime_sec)
            {
                cg.alpha = fromAlpha;
                cg.DOFade(toAlpha, fadeTime_sec);
            }
            else
            {
                cg.alpha = toAlpha;
            }
        }

        // NOTE: 同一keyObjectの場合、SpriteとGameObjectは混在しない想定
        public virtual void Set<T>(bool isOn, T? keyObject, GameObject instantiated,
            Vector2 pivot, float scale, Vector2 position, Vector2 offset, float fromAlpha, float toAlpha, float fadeTime_sec)
            where T : class
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this.IsOn = isOn;
            var img = this._image;
            var cg = this._canvasGroup;

            cg.DOComplete();

            if (Equals(keyObject, this.KeyObject) && this.Instantiated != instantiated)
            {
                DevLog.LogWarning($"{NAME} It's the same {nameof(keyObject)}, but a different {nameof(instantiated)}.");
            }

            if (!Equals(keyObject, this.KeyObject))
            {
                cg.alpha = 0;
                if (this.Instantiated != null && this.Instantiated.transform.IsChildOf(cg.transform))
                {
                    this.Instantiated.SetActive(false);
                }
                this.Instantiated = null;
                this.SpriteRenderers = null;
                img.sprite = null;
            }

            this.KeyObject = keyObject;
            this.Instantiated = instantiated;
            img.rectTransform.localScale = new Vector3(0, 0, 0); // NOTE: Image も表示されてしまうため、とりあえず見えないようにする

            instantiated.transform.SetParent(cg.transform, false);
            instantiated.SetActive(true);
            this.SpriteRenderers = instantiated.GetComponentsInChildren<SpriteRenderer>(true);
            var t = instantiated.transform;
            t.localPosition = (Vector3)position;
            if (this.SpriteRenderers != null && 0 < this.SpriteRenderers.Length)
            {
                var ppu = this.SpriteRenderers[0].sprite.pixelsPerUnit;
                var fixedScale = ppu * scale;
                this.SpriteRenderers[0].transform.localScale = new Vector3(fixedScale, fixedScale, fixedScale);
            }
            else
            {
                t.localScale = new Vector3(scale, scale, scale); // TODO: 後でちゃんとする
            }

            if (0 < fadeTime_sec)
            {
                cg.alpha = fromAlpha;
                cg.DOFade(toAlpha, fadeTime_sec);
            }
            else
            {
                cg.alpha = toAlpha;
            }
        }

        public virtual void Off(float fadeTime_sec)
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this.IsOn = false;
            this.KeyObject = null;
            var img = this._image;
            var cg = this._canvasGroup;
            var sprite = img.sprite;
            cg.DOComplete();
            if (0 < fadeTime_sec)
            {
                cg.DOFade(0, fadeTime_sec).OnComplete(() =>
                {
                    if (img.sprite == sprite)
                    {
                        img.sprite = null;
                    }
                });
            }
            else
            {
                cg.alpha = 0;
                img.sprite = null;
            }
        }

        public void ToMain() => this.FadeColor(Color.white);
        public void ToSub(Color grayoutColor) => this.FadeColor(grayoutColor);
        void FadeColor(Color fadeColor)
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this._image.DOColor(fadeColor, 0.2f);
            if (this.SpriteRenderers != null)
            {
                foreach (var renderer in this.SpriteRenderers)
                {
                    if (renderer == null) continue;
                    renderer.DOColor(fadeColor, 0.2f);
                }
            }
        }
    }
}
