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

        [SerializeField] protected bool _isTypewritingEnabled = true;
        [SerializeField] protected float _typewriterInterval_sec = 0.05f;

        [SerializeField] protected TMP_Text _txtName;
        [SerializeField] protected TMP_Text _txtText;
        [SerializeField] protected Image _imgNextSymbol;
        [SerializeField] protected Vector2 _offsetImgNext = new(32, 12);

        // Status

        [NonSerialized] protected RectTransform _parentRectTransform;
        [NonSerialized] protected Animator _animNextSymbol;
        [NonSerialized] protected float _typewriterIntervalCounter_sec = 0;

        public bool IsDisplayed => this.gameObject.activeSelf;

        public bool IsTypewriting => this._txtText != null && this._txtText.textInfo.characterCount != this._txtText.maxVisibleCharacters;
        public CmdText.PageCtrlType LastPageCtrl { get; protected set; } = CmdText.PageCtrlType.InputBrPageAndNoHide;

        // Methods

        protected virtual void Awake()
        {
            this._parentRectTransform = this._txtText.rectTransform.parent as RectTransform;
            this._animNextSymbol = this._imgNextSymbol.GetComponent<Animator>();
            this._txtName.text = "";
            this._txtText.text = "";
        }

        protected virtual void Update()
        {
            if (!this.IsTypewriting) return;

            this._typewriterIntervalCounter_sec += Time.deltaTime;
            while (this.IsTypewriting && this._typewriterInterval_sec <= this._typewriterIntervalCounter_sec)
            {
                this._typewriterIntervalCounter_sec -= this._typewriterInterval_sec;
                this._txtText.maxVisibleCharacters++;
            }
        }

        public virtual void SetText(AdvRoot advRoot, string name, string text, CmdText.PageCtrlType pageCtrl, string windowType)
        {
            this.DisableImgNextSymbol();
            advRoot.ShowUI();

            this._txtName.text = name;
            switch (this.LastPageCtrl)
            {
                // NOTE: defaultはInputBrPage
                default:
                case CmdText.PageCtrlType.InputBrPageAndNoHide:
                case CmdText.PageCtrlType.InputBrPage:
                    this._txtText.text = text;
                    if (this._isTypewritingEnabled)
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

        public virtual void EndTypewriting()
        {
            this._txtText.maxVisibleCharacters = this._txtText.textInfo.characterCount;
        }

        public virtual void DisableImgNextSymbol()
        {
            this._imgNextSymbol.enabled = false;
        }

        public virtual void EnableImgNextSymbol()
        {
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

        public virtual void Show()
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
        }

        public virtual void Hide()
        {
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
