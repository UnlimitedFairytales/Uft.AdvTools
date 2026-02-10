#nullable enable

using DG.Tweening;
using System;
using TMPro;
using Uft.AdvTools.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class MessageWindow : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected bool _isTypewriterEnabled = true;
        [SerializeField] protected float _typewriterInterval_sec = 0.05f;
        [SerializeField] protected bool _isNameAreaHiddenOnNonCharacterText = false;

        [SerializeField] protected CanvasGroup? _canvasGroup;
        [SerializeField] protected GameObject? _goNameAreaRoot;
        [SerializeField] protected TMP_Text? _txtName;
        [SerializeField] protected TMP_Text? _txtText;
        [SerializeField] protected Image? _imgNextSymbol;
        [SerializeField] protected Vector2 _offsetImgNext = new(48, 18);

        // Status

        protected RectTransform? _parentRectTransform;
        protected Animator? _animNextSymbol;
        protected float _typewriterIntervalCounter_sec = 0;

        public bool IsDisplayed => this.gameObject.activeSelf;

        public bool IsTypewriting => this._txtText != null && this._txtText.textInfo.characterCount != this._txtText.maxVisibleCharacters;
        public CmdText.PageCtrlType LastPageCtrl { get; protected set; } = CmdText.PageCtrlType.InputBrPageAndNoHide;

        // Unity events

        protected virtual void Awake()
        {
            if (this._goNameAreaRoot == null || this._txtName == null || this._txtText == null || this._imgNextSymbol == null)
                throw new InvalidOperationException($"{nameof(this._goNameAreaRoot)}, {nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");

            this._parentRectTransform = this._txtText.rectTransform.parent as RectTransform;
            this._animNextSymbol = this._imgNextSymbol.GetComponent<Animator>();
            this._txtName.text = "";
            this._txtText.text = "";
            if (this._isNameAreaHiddenOnNonCharacterText)
            {
                this._goNameAreaRoot.SetActive(false);
            }
        }

        protected virtual void Update()
        {
            if (this._txtName == null || this._txtText == null || this._imgNextSymbol == null) throw new InvalidOperationException($"{nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");
            if (!this.IsTypewriting) return;

            this._typewriterIntervalCounter_sec += Time.deltaTime;
            while (this.IsTypewriting && this._typewriterInterval_sec <= this._typewriterIntervalCounter_sec)
            {
                this._typewriterIntervalCounter_sec -= this._typewriterInterval_sec;
                this._txtText.maxVisibleCharacters++;
            }
        }

        // Methods

        public virtual void SetText(AdvRoot advRoot, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            if (this._goNameAreaRoot == null || this._txtName == null || this._txtText == null || this._imgNextSymbol == null)
                throw new InvalidOperationException($"{nameof(this._goNameAreaRoot)}, {nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");

            this.DisableImgNextSymbol();
            advRoot.ShowUI(0);

            this._txtName.text = name;
            if (string.IsNullOrEmpty(this._txtName.text) && this._isNameAreaHiddenOnNonCharacterText)
            {
                this._goNameAreaRoot.SetActive(false);
            }
            else
            {
                this._goNameAreaRoot.SetActive(true);
            }
            switch (this.LastPageCtrl)
            {
                // NOTE: defaultはInputBrPage
                default:
                case CmdText.PageCtrlType.InputBrPageAndNoHide:
                case CmdText.PageCtrlType.InputBrPage:
                    this._txtText.text = text;
                    if (this._isTypewriterEnabled)
                    {
                        this._typewriterIntervalCounter_sec = 0;
                        this._txtText.maxVisibleCharacters = 0;
                    }
                    break;
                case CmdText.PageCtrlType.InputBr:
                    this._txtText.text += "\n" + text;
                    break;
                case CmdText.PageCtrlType.Input:
                case CmdText.PageCtrlType.Next:
                    this._txtText.text += text;
                    break;
            }
            this.LastPageCtrl = pageCtrl;
            // NOTE: windowType は 対応予定なし

            this._txtText.ForceMeshUpdate();
        }

        public virtual void FixLastPageCtrl()
        {
            if (this.LastPageCtrl == CmdText.PageCtrlType.InputBrPage)
            {
                this.LastPageCtrl = CmdText.PageCtrlType.InputBrPageAndNoHide;
            }
        }

        public virtual void ForceSetLastPageCtrl(CmdText.PageCtrlType pageCtrlType)
        {
            this.LastPageCtrl = pageCtrlType;
        }

        public virtual void EndTypewriting()
        {
            if (this._txtName == null || this._txtText == null || this._imgNextSymbol == null) throw new InvalidOperationException($"{nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");

            this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
        }

        public virtual void DisableImgNextSymbol()
        {
            if (this._txtName == null || this._txtText == null || this._imgNextSymbol == null) throw new InvalidOperationException($"{nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");

            this._imgNextSymbol.enabled = false;
        }

        public virtual void EnableImgNextSymbol()
        {
            if (this._txtName == null || this._txtText == null || this._imgNextSymbol == null) throw new InvalidOperationException($"{nameof(this._txtName)}, {nameof(this._txtText)}, {nameof(this._imgNextSymbol)} are required.");
            if (this._animNextSymbol == null) throw new InvalidOperationException($"{nameof(this._animNextSymbol)} is required.");

            if (this._imgNextSymbol.enabled) return;

            this._txtText.ForceMeshUpdate();
            this._imgNextSymbol.enabled = true;
            this._animNextSymbol.Play(this._animNextSymbol.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            var Count = this._txtText.textInfo.characterCount;
            Vector3 localPos;
            if (0 < Count)
            {
                localPos = this._txtText.textInfo.characterInfo[Count - 1].bottomRight;
            }
            else
            {
                var firstLine = this._txtText.textInfo.lineInfo[0];
                localPos = new Vector3(firstLine.lineExtents.min.x, firstLine.lineExtents.min.y, 0);
            }
            Vector3 worldPos = this._txtText.transform.TransformPoint(localPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this._parentRectTransform,
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                null,
                out Vector2 anchoredPos);
            anchoredPos += this._offsetImgNext;
            this._imgNextSymbol.rectTransform.anchoredPosition = anchoredPos;
        }

        public virtual void Show(float fadeSeconds)
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");

            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
                this._canvasGroup.DOKill();
                if (fadeSeconds <= 0)
                {
                    this._canvasGroup.alpha = 1.0f;

                }
                else
                {
                    this._canvasGroup.DOFade(1, fadeSeconds);
                }
            }
        }

        public virtual void Hide(float fadeSeconds)
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");

            if (this.gameObject.activeSelf)
            {
                this._canvasGroup.DOKill();
                if (fadeSeconds <= 0)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                    this._canvasGroup
                        .DOFade(0, fadeSeconds)
                        .OnComplete(() => this.gameObject.SetActive(false));
                }
            }
        }
    }
}
