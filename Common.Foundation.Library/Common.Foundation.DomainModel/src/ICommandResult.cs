namespace Common.Foundation.DomainModel
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }
        bool IsFailure { get; }
        object Result { get; set; }
    }
}
