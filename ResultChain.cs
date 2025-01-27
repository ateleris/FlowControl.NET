namespace FlowControl.NET;

public class ResultChain<T, E>(Task<ResultOrError<T, E>> initial) where E : Error?
{
    private Task<ResultOrError<List<object>, E>> results = initial.ContinueWith(t =>
            t.Result.MatchFuncAsync(
                success => ResultOrError<List<object>, E>.Success([success]),
                error => ResultOrError<List<object>, E>.Error(error)
            )
        ).Unwrap();

    public ResultChain<T, E> Then<U>(Func<Task<ResultOrError<U, E>>> next)
    {
        results = results.ContinueWith(t =>
            t.Result.MatchFuncAsync(async list =>
            {
                var nextResult = await next();
                return await nextResult.MatchFuncAsync(
                    success =>
                    {
                        list.Add(success!);
                        return ResultOrError<List<object>, E>.Success(list);
                    },
                    error => ResultOrError<List<object>, E>.Error(error)
                );
            },
            error => Task.FromResult(ResultOrError<List<object>, E>.Error(error))
            )
        ).Unwrap();
        return this;
    }

    public Task<U> FinallyAsync<U>(Func<List<object>, Task<U>> success, Func<E, Task<U>> failure) =>
        results.ContinueWith(t => t.Result.MatchFuncAsync(success, failure)).Unwrap();

    public U Finally<U>(Func<List<object>, U> success, Func<E, U> failure) =>
        results.ContinueWith(t => t.Result.MatchFuncAsync(success, failure)).Unwrap().Result;
}

public static class ResultOrErrorExtensions
{
    public static ResultChain<T, E> AsChain<T, E>(this Task<ResultOrError<T, E>> result)
        where E : Error? => new(result);
}
