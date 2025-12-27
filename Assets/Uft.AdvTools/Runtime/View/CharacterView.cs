#nullable enable

using DG.Tweening;
using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    /// <summary>2DPrefabの場合は、LateUpdate（=Animator適用後）にCanvasGroupのalpha値をして上書きします。</summary>
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup? _canvasGroup;
        [SerializeField] protected Image? _image;

        public GameObject? Instantiated { get; protected set; }
        public SpriteRenderer[]? SpriteRenderers { get; protected set; }

        /// <summary>この値がある値が、現在の最新の表示箇所</summary>
        public Character? Character { get; protected set; }
        public bool IsOn => this.Character != null;
        public Image? Image => this._image!;

        /// <summary>spriteとinstantiatedはどちらしか渡せない</summary>
        public void Set(bool isAlreadyDisplayed, Character character, Sprite? sprite, GameObject? instantiated, Vector2 toPos, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var img = this._image; if (img == null) return;
            var cg = this._canvasGroup; if (cg == null) return;
            if (sprite != null && instantiated != null) throw new ArgumentException($"[{nameof(SpriteRenderer)}] You can only give {nameof(sprite)} or {nameof(instantiated)}");

            // 1. 既存は完了
            cg.DOComplete();

            if (this.Character == character && this.Instantiated != instantiated)
            {
                DevLog.LogWarning($"{nameof(CharacterView)} It's the same character, but a different instantiated.");
            }

            // 2. キャラクターの上書き切り替えの場合、以前のキャラクターは即座に消す
            if (this.Character != character)
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

            // 3. すでに別のCharacterViewで表示済みなら、フェードインなし
            if (isAlreadyDisplayed)
            {
                cg.alpha = 1;
            }
            else
            {
                cg.alpha = 0;
                cg.DOFade(1, fadeTime_sec);
            }

            // 4. currentの設定
            this.Character = character;
            img.sprite = sprite;
            this.Instantiated = instantiated;

            // 5. currentの詳細設定
            if (sprite != null)
            {
                img.SetNativeSize();
                var rt = img.rectTransform;
                rt.pivot = pivot.GetPivot();
                rt.anchoredPosition = toPos;
                rt.localScale = new Vector3(scale, scale, scale);
            }
            if (instantiated != null)
            {
                img.rectTransform.localScale = Vector3.zero;
                instantiated.transform.SetParent(cg.transform, false);
                instantiated.SetActive(true);
                this.SpriteRenderers = instantiated.GetComponentsInChildren<SpriteRenderer>();

                var t = instantiated.transform;
                t.localPosition = (Vector3)toPos;
                if (this.SpriteRenderers != null && 0 < this.SpriteRenderers.Length)
                {
                    var ppu = this.SpriteRenderers[0].sprite.pixelsPerUnit;
                    var fixedScale = ppu * scale;
                    this.SpriteRenderers[0].transform.localScale = new Vector3(fixedScale, fixedScale, fixedScale);
                }
                else
                {
                    t.localScale = new Vector3(scale, scale, scale);
                }
            }
        }

        public void Off(float fadeTime_sec)
        {
            var img = this._image; if (img == null) return;
            var cg = this._canvasGroup; if (cg == null) return;

            this.Character = null;
            if (0 < fadeTime_sec)
            {
                cg.DOComplete();
                cg.DOFade(0, fadeTime_sec);
            }
            else
            {
                cg.alpha = 0;
            }
        }

        public void ToMain()
        {
            var img = this._image; if (img == null) return;

            img.DOColor(Color.white, 0.2f);
            if (this.SpriteRenderers != null)
            {
                foreach (var renderer in this.SpriteRenderers)
                {
                    renderer.DOColor(Color.white, 0.2f);
                }
            }
        }

        public void ToSub(Color grayoutColor)
        {
            var img = this._image; if (img == null) return;

            img.DOColor(grayoutColor, 0.2f);
            if (this.SpriteRenderers != null)
            {
                foreach (var renderer in this.SpriteRenderers)
                {
                    renderer.DOColor(grayoutColor, 0.2f);
                }
            }
        }

        public void LateUpdate()
        {
            if (this.SpriteRenderers == null || this._canvasGroup == null) return;
            if (this.Character == null) return;

            var canvasGroupAlpha = this._canvasGroup.alpha;
            foreach (var renderer in this.SpriteRenderers)
            {
                if (!renderer.transform.IsChildOf(this._canvasGroup.transform)) continue;
                var c = renderer.color;
                if (!Mathf.Approximately(c.a, canvasGroupAlpha))
                {
                    c.a = canvasGroupAlpha;
                    renderer.color = c;
                }
            }
        }
    }
}
