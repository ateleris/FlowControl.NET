namespace FlowControl.NET;

public class SuccessOrError<T> : ResultOrError<bool, T> where T : Error
{
    public SuccessOrError() : base(true)
    {
    }

    public SuccessOrError(T error) : base(error)
    {
    }

    public static SuccessOrError<T> Success()
    {
        return new SuccessOrError<T>();
    }

    public new static SuccessOrError<T> Error(T value) => new(value);

    public static implicit operator SuccessOrError<T>(T value)
    {
        return new SuccessOrError<T>(value);
    }

    public new bool Value => base.Value;

    public new T? ErrorValue => base.ErrorValue;
}
