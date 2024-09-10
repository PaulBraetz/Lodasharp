namespace Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Lodasharp.Maybe;
using Lodasharp;

public class MaybeTests
{
    [Fact]
    public void Bind_sets_value_if_some()
    {
        var value = new Object();
        var unwrapped = Some(value).Bind<Object[]>(o => [o]).Unwrap();
        Assert.Equal(value, unwrapped.Single());
    }
    [Fact]
    public void Bind_returns_value_from_maybe_factory_if_some()
    {
        var value = new Object();
        var unwrapped = Some(value).Bind(o => Some<Object[]>([o])).Unwrap();
        Assert.Equal(value, unwrapped.Single());
    }
    [Fact]
    public void Bind_does_not_set_value_if_none()
    {
        var instance = None<Object>().Bind<Object[]>(o => [o]);
        _ = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
    }
    [Fact]
    public void Bind_does_not_return_value_from_maybe_factory_if_some()
    {
        var instance = None<Object>().Bind(o => Some<Object[]>([o]));
        _ = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
    }
    [Fact]
    public void Unwrap_throws_ioe_if_none()
    {
        var instance = None<Object>();
        _ = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
    }
    [Fact]
    public void Unwrap_returns_value_if_some()
    {
        var value = new Object();
        var instance = Some(value);
        var unwrapped = instance.Unwrap();
        Assert.Equal(value, unwrapped);
    }
    [Fact]
    public void Unwrap_throws_ioe_with_error_if_none()
    {
        const String error = "Custom error message.";
        var instance = None<Object>(error);
        var ex = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
        Assert.Equal(error, ex.Message);
    }
    [Fact]
    public void Or_sets_error_provided()
    {
        const String error = "Custom error message.";
        var instance = None<Object>(error).Or(error);
        var ex = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
        Assert.Equal(error, ex.Message);
    }
    [Fact]
    public void Or_sets_value_provided()
    {
        var value = new Object();
        var instance = None<Object>().Or(value);
        var unwrapped = instance.Unwrap();
        Assert.Equal(value, unwrapped);
    }
    [Fact]
    public void Or_sets_value_from_factory_provided()
    {
        var value = new Object();
        var instance = None<Object>().Or(() => value);
        var unwrapped = instance.Unwrap();
        Assert.Equal(value, unwrapped);
    }
    [Fact]
    public void Or_sets_value_from_maybe_factory_provided()
    {
        var value = new Object();
        var instance = None<Object>().Or(() => Some(value));
        var unwrapped = instance.Unwrap();
        Assert.Equal(value, unwrapped);
    }
    [Fact]
    public void Or_sets_error_from_maybe_factory_provided()
    {
        const String error = "Custom error message.";
        var instance = None<Object>().Or(() => None<Object>(error));
        var ex = Assert.Throws<InvalidOperationException>(() => instance.Unwrap());
        Assert.Equal(error, ex.Message);
    }
}
