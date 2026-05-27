#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using Uft.AdvTools.Commands;
using Uft.UnityUtils.UI;
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
        [SerializeField] protected GameObject? _goNameAreaRoot; // NOTE: nullでも動くように
        [SerializeField] protected TMP_Text? _txtName; // NOTE: nullでも動くように
        [SerializeField] protected TMP_Text? _txtText;
        [SerializeField] protected Image? _imgNextSymbol; // NOTE: nullでも動くように
        [SerializeField] protected Vector2 _offsetImgNext = new(48, 18);

        // Status

        protected RectTransform? _parentRectTransform;
        protected Animator? _animNextSymbol; // NOTE: nullでも動くように
        protected CancellationTokenSource? _lastCts;

        public bool IsDisplayed => this.gameObject.activeSelf;

        public bool IsTypewriting => this._txtText != null && this._txtText.maxVisibleCharacters < this._txtText.textInfo.characterCount;
        public CmdText.PageCtrlType LastPageCtrl { get; protected set; } = CmdText.PageCtrlType.InputBrPageAndNoHide;

        // Unity events

        protected virtual void Awake()
        {
            if (this._txtText == null) throw new InvalidOperationException($"{nameof(this._txtText)} is required.");

            this._parentRectTransform = this._txtText.rectTransform.parent as RectTransform;

            if (this._imgNextSymbol != null)
            {
                this._animNextSymbol = this._imgNextSymbol.GetComponent<Animator>();
            }
            if (this._txtName != null)
            {
                this._txtName.text = "";
            }
            this._txtText.text = "";
            if (this._isNameAreaHiddenOnNonCharacterText && this._goNameAreaRoot != null)
            {
                this._goNameAreaRoot.SetActive(false);
            }
        }

        protected virtual void OnDestroy()
        {
            this._lastCts?.Cancel();
            this._lastCts?.Dispose();
            this._lastCts = null;
        }

        // Methods

        public virtual void SetText(AdvRoot advRoot, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            if (this._txtText == null) throw new InvalidOperationException($"{nameof(this._txtText)} is required.");

            if (this._lastCts != null)
            {
                this._lastCts.Cancel();
                this._lastCts.Dispose();
                this._lastCts = null;
            }
            this._lastCts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken);
            this.DisableImgNextSymbol();
            advRoot.ShowUI(0);

            if (this._txtName != null && this._goNameAreaRoot != null)
            {
                this._txtName.text = name;
                if (string.IsNullOrEmpty(this._txtName.text) && this._isNameAreaHiddenOnNonCharacterText)
                {
                    this._goNameAreaRoot.SetActive(false);
                }
                else
                {
                    this._goNameAreaRoot.SetActive(true);
                }
            }
            switch (this.LastPageCtrl)
            {
                // NOTE: defaultはInputBrPage
                default:
                case CmdText.PageCtrlType.InputBrPageAndNoHide:
                case CmdText.PageCtrlType.InputBrPage:
                    if (this._isTypewriterEnabled)
                    {
                        this._txtText.TypeWriterEffectAsync(this._lastCts.Token, text, false, this._typewriterInterval_sec).Forget();
                    }
                    else
                    {
                        this._txtText.text = text;
                        this._txtText.ForceMeshUpdate();
                        this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
                    }
                    break;
                case CmdText.PageCtrlType.InputBr:
                    var t1 = "\n" + text;
                    if (this._isTypewriterEnabled)
                    {
                        this._txtText.TypeWriterEffectAsync(this._lastCts.Token, t1, true, this._typewriterInterval_sec).Forget();
                    }
                    else
                    {
                        this._txtText.text += t1;
                        this._txtText.ForceMeshUpdate();
                        this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
                    }
                    break;
                case CmdText.PageCtrlType.Input:
                case CmdText.PageCtrlType.Next:
                    if (this._isTypewriterEnabled)
                    {
                        this._txtText.TypeWriterEffectAsync(this._lastCts.Token, text, true, this._typewriterInterval_sec).Forget();
                    }
                    else
                    {
                        this._txtText.text += text;
                        this._txtText.ForceMeshUpdate();
                        this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
                    }
                    break;
            }
            this.LastPageCtrl = pageCtrl;
            // NOTE: windowType は 対応予定なし
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
            if (this._txtText == null) throw new InvalidOperationException($"{nameof(this._txtText)} is required.");

            if (this._lastCts != null)
            {
                this._lastCts.Cancel();
                this._lastCts.Dispose();
                this._lastCts = null;
            }
            this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
        }

        public virtual void DisableImgNextSymbol()
        {
            if (this._imgNextSymbol != null)
            {
                this._imgNextSymbol.enabled = false;
            }
        }

        public virtual void EnableImgNextSymbol()
        {
            if (this._txtText == null) throw new InvalidOperationException($"{nameof(this._txtText)} is required.");
            if (this._imgNextSymbol == null) return;
            if (this._imgNextSymbol.enabled) return;

            this._txtText.ForceMeshUpdate();
            this._imgNextSymbol.enabled = true;
            if (this._animNextSymbol != null)
            {
                this._animNextSymbol.Play(this._animNextSymbol.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
            }
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
            if (this._txtText == null) throw new InvalidOperationException($"{nameof(this._txtText)} is required.");
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");

            if (!this.gameObject.activeSelf)
            {
                if (this._txtName != null && this._goNameAreaRoot != null)
                {
                    if (this._isNameAreaHiddenOnNonCharacterText)
                    {
                        this._goNameAreaRoot.SetActive(false);
                    }
                    this._txtName.text = "";
                }
                this._txtText.text = "";
                this._canvasGroup.alpha = 0;
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
