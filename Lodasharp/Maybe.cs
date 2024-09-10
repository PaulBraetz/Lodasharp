namespace Lodasharp;

public static class Maybe
{
    public static Maybe<T> Some<T>(T value) => Maybe<T>.Some(value);
    public static Maybe<T> None<T>(String? error = null) => Maybe<T>.None(error);
}

public readonly struct Maybe<T>
{
    private Maybe(T value)
    {
        _value = value;
        _hasValue = true;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Maybe(String? error) => _error = error;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private readonly Boolean _hasValue;
    private readonly T _value;
    private readonly String? _error;

    public static Maybe<T> Some(T value) => new(value);
    public static Maybe<T> None(String? error = null) => new(error);
    public Maybe<T> None() => new();
    public Maybe<TResult> Bind<TResult>(Func<T, TResult> onSome) =>
        _hasValue
        ? new(onSome.Invoke(_value))
        : default;
    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> onSome) =>
        _hasValue
        ? onSome.Invoke(_value)
        : default;
    public T Unwrap(Func<T>? onNone = null) =>
        _hasValue
        ? _value
        : onNone != null
            ? onNone.Invoke()
            : throw ( new InvalidOperationException(_error ?? $"Unable to unwrap Maybe<{typeof(T).Name}> instance. No value was contained in the instance.") );
    public Maybe<T> Or(Func<Maybe<T>> onNone) =>
        _hasValue
        ? this
        : onNone.Invoke();
    public Maybe<T> Or(Func<T> onNone) =>
        _hasValue
        ? this
        : new(onNone.Invoke());
    public Maybe<T> Or(T onNone) =>
        _hasValue
        ? this
        : new(onNone);
    public Maybe<T> Or(String error) =>
        _hasValue
        ? this
        : new(error);
}