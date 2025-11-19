#nullable enable

namespace Uft.AdvTools.View
{
    public enum WindowState
    {
        Closed,
        Showing,
        Shown,
        Closing
    }

    public enum OperationResultStatus
    {
        Accepted,
        Canceled,
        RejectedDueToDuplicate,
        RejectedDueToCooldown,
    }

    public readonly struct OperationResult<T>
    {
        public readonly T Value;
        public readonly OperationResultStatus Status;

        public OperationResult(T value, OperationResultStatus status)
        {
            this.Value = value;
            this.Status = status;
        }
    }
}
