#nullable enable

using System.Collections.Generic;
using Uft.AdvTools.View;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;

namespace Uft.AdvTools
{
    /// <summary>
    /// 1. 非表示Windowへフォーカスする場合、直前のWindowは非表示される<br/>
    /// 2. 表示Window間へフォーカスする場合、直前のWindowはそのまま
    /// </summary>
    public class MessageWindowManager
    {
        static readonly MessageWindow[] EMPTY_LIST =  new MessageWindow[0];

        public string DefaultMessageWindowName { get; set; } = "MessageWindow";
        public IReadOnlyList<MessageWindow> MessageWindowList { get; protected set; } = EMPTY_LIST;
        public MessageWindow? CurrentMessageWindow { get; protected set; }

        public void Setup(IReadOnlyList<MessageWindow> messageWindowList)
        {
            ThrowIf.NullOrEmpty(messageWindowList, nameof(messageWindowList));

            this.CurrentMessageWindow = null;
            this.MessageWindowList = messageWindowList;

            // 初期ウィンドウの決定
            for (int i = 0; i < this.MessageWindowList.Count; i++)
            {
                if (this.MessageWindowList[i].name == this.DefaultMessageWindowName)
                {
                    this.CurrentMessageWindow = this.MessageWindowList[i];
                    break;
                }
            }
            if (this.CurrentMessageWindow == null)
            {
                DevLog.LogWarning($"[{nameof(MessageWindowManager)}] {nameof(this.DefaultMessageWindowName)} was not found. Use index 0.");
                this.CurrentMessageWindow = messageWindowList[0];
            }

            // 初期ウィンドウ以外を非表示に
            for (int i = 0; i < this.MessageWindowList.Count; i++)
            {
                if (this.MessageWindowList[i].name != this.DefaultMessageWindowName)
                {
                    this.MessageWindowList[i].gameObject.SetActive(false);
                    break;
                }
            }
        }

        public void Cleanup()
        {
            this.CurrentMessageWindow = null;
            this.MessageWindowList = EMPTY_LIST;
        }

        /// <summary>nextのShowはしない</summary>
        public void ChangeMessageWindow(string messageWindowName)
        {
            ThrowIf.NullOrEmpty(messageWindowName, nameof(messageWindowName));
            ThrowIf.NullOrEmpty(this.MessageWindowList, nameof(this.MessageWindowList));

            for (int i = 0; i < this.MessageWindowList.Count; i++)
            {
                if (this.MessageWindowList[i].name == messageWindowName)
                {
                    var next = this.MessageWindowList[i];
                    if (!next.IsDisplayed &&
                        this.CurrentMessageWindow != null &&
                        this.CurrentMessageWindow != next)
                    {
                        this.CurrentMessageWindow.Hide();
                    }
                    this.CurrentMessageWindow = next;
                    return;
                }
            }

            DevLog.LogWarning($"[{nameof(MessageWindowManager)}] {messageWindowName} is not found.");
        }
    }
}
