namespace FlowControl.NET;

public class ResultOrError<T, E> where E : Error?
{
    private readonly T? tVal;
    private readonly E? error;
    private readonly bool isError;

    public ResultOrError(T value)
    {
        tVal = value;
        isError = false;
    }

    public ResultOrError(E value)
    {
        error = value;
        isError = true;
    }

    public static implicit operator ResultOrError<T, E>(T value)
    {
        return new ResultOrError<T, E>(value);
    }

    public static implicit operator ResultOrError<T, E>(E value)
    {
        return new ResultOrError<T, E>(value);
    }

    public Task<U> MatchFuncAsync<U>(Func<T, U> f1, Func<E, U> f2)
        => Task.FromResult(!isError ? f1(tVal!) : f2(error!));

    public async Task<U> MatchFuncAsync<U>(Func<T, Task<U>> f1, Func<E, U> f2)
        => !isError ? await f1(tVal!) : f2(error!);

    public async Task<U> MatchFuncAsync<U>(Func<T, U> f1, Func<E, Task<U>> f2)
        => !isError ? f1(tVal!) : await f2(error!);

    public async Task<U> MatchFuncAsync<U>(Func<T, Task<U>> f1, Func<E, Task<U>> f2)
        => !isError ? await f1(tVal!) : await f2(error!);

    public void MatchAction(Action<T> f1, Action<E> f2)
    {
        if (!isError)
        {
            f1(tVal!);
        }
        else
        {
            f2(error!);
        }
    }

    public Task MatchActionAsync(Action<T> f1, Action<E> f2)
    {
        if (isError) f2(error!);
        else f1(tVal!);
        return Task.CompletedTask;
    }

    public Task MatchActionAsync(Func<T, Task> f1, Func<E, Task> f2)
        => !isError ? f1(tVal!) : f2(error!);

    public Task MatchActionAsync(Func<T, Task> f1, Action<E> f2)
    {
        if (isError) return f1(tVal!);
        f2(error!);
        return Task.CompletedTask;
    }

    public Task MatchActionAsync(Action<T> f1, Func<E, Task> f2)
    {
        if (isError)
        {
            f1(tVal!);
            return Task.CompletedTask;
        }
        return f2(error!);
    }

    public static ResultOrError<T, E> Success(T value)
    {
        return new ResultOrError<T, E>(value);
    }

    public static ResultOrError<T, E> Error(E value)
    {
        return new ResultOrError<T, E>(value);
    }

    public bool IsSuccess => !isError;

    public bool IsError => isError;

    public T Value => !isError ? tVal! : throw new InvalidOperationException("Cannot access Value on an Error result.");

    public E ErrorValue => isError ? error! : throw new InvalidOperationException("Cannot access ErrorValue on a Success result.");
}
