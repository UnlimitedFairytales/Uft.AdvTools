#nullable enable

using Uft.AdvTools.View;

namespace Uft.AdvTools
{
    public class MessageWindowManager
    {
        public MessageArea? MessageArea { get; protected set; } // NOTE: Setup() 自動検出

        public void Setup(MessageArea[] messageAreaList)
        {
            this.MessageArea = messageAreaList[0];
        }

        public void Cleanup()
        {
            this.MessageArea = null;
        }
    }
}
