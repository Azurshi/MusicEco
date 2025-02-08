using System;
using System.Reflection;
namespace MusicEco.Common;
public readonly struct Subscription(WeakReference? subcriber, MethodInfo handler) : IEquatable<Subscription> {
    public readonly WeakReference? SubscriberWeakReference = subcriber;
    public readonly MethodInfo Handler = handler;
    public bool Equals(Subscription other) => SubscriberWeakReference == other.SubscriberWeakReference && Handler == other.Handler;
    public override bool Equals(object? obj) => obj is Subscription other && Equals(other);
    public override int GetHashCode() => SubscriberWeakReference?.GetHashCode() ?? 0 ^ Handler.GetHashCode();
}
public sealed class WeakEventHandler<TEventArgs>: WeakEventHandler where TEventArgs : EventArgs {
    public static WeakEventHandler<TEventArgs> operator +(WeakEventHandler<TEventArgs> source, Action<object?, TEventArgs> handler) {
        source.Connect(handler);
        return source;
    }
    public static WeakEventHandler<TEventArgs> operator -(WeakEventHandler<TEventArgs> source, Action<object?, TEventArgs> handler) {
        source.Disconnect(handler);
        return source;
    }
    public void Connect(Action<object?, TEventArgs> handler) {
        if (handler.Target == null) {
            _eventHandlers.Add(new(null, handler.GetMethodInfo()));
        }
        else {
            _eventHandlers.Add(new(new(handler.Target), handler.GetMethodInfo()));
        }
    }
    public void Disconnect(Action<object?, TEventArgs> handler) {
        object? handlerTarget = handler.Target;
        string methodName = handler.GetMethodInfo().Name;
        for (int n = _eventHandlers.Count - 1; n >= 0; n--) {
            Subscription current = _eventHandlers[n];
            if (current.SubscriberWeakReference != null && !current.SubscriberWeakReference.IsAlive) {
                _eventHandlers.RemoveAt(n);
                continue;
            }
            if (current.SubscriberWeakReference?.Target == handlerTarget && current.Handler.Name == methodName) {
                _eventHandlers.RemoveAt(n);
                break;
            }
        }
    }
    public void Invoke(object? sender, TEventArgs? args) {
        List<(object? subscriber, MethodInfo handler)> toRaise = [];
        List<int> toRemove = [];
        for (int i = 0; i < _eventHandlers.Count; i++) {
            Subscription subscription = _eventHandlers[i];
            // Null subcriber weak reference
            bool isStatic = subscription.SubscriberWeakReference == null;
            if (isStatic) {
                toRaise.Add((null, subscription.Handler));
            }
            // Null subcriber target
            object? subscriber = subscription.SubscriberWeakReference?.Target;
            if (subscriber == null) {
                toRemove.Add(i);
            }
            else {
                toRaise.Add((subscriber, subscription.Handler));
            }
        }
        for (int n=toRemove.Count-1; n>=0; n--) {
            int subscriptionIndex = toRemove[n];
            _eventHandlers.RemoveAt(subscriptionIndex);
        }
        for (int i = 0; i < toRaise.Count; i++) {
            (object? subcriber, MethodInfo handler) = toRaise[i];
            handler.Invoke(subcriber, [sender, args]);
        }
    }
}
public class WeakEventHandler {
    protected readonly List<Subscription> _eventHandlers = [];
    public int Count => _eventHandlers.Count;
    public static WeakEventHandler operator +(WeakEventHandler source, Action<object?, EventArgs> handler) {
        source.Connect(handler);
        return source;
    }
    public static WeakEventHandler operator -(WeakEventHandler source, Action<object?, EventArgs> handler) {
        source.Disconnect(handler);
        return source;
    }
    public void Connect(Action<object?, EventArgs> handler) {
        if (handler.Target == null) {
            _eventHandlers.Add(new(null, handler.GetMethodInfo()));
        }
        else {
            _eventHandlers.Add(new(new(handler.Target), handler.GetMethodInfo()));
        }
    }
    public void Disconnect(Action<object?, EventArgs> handler) {
        object? handlerTarget = handler.Target;
        string methodName = handler.GetMethodInfo().Name;
        for (int n = _eventHandlers.Count - 1; n >= 0; n--) {
            Subscription current = _eventHandlers[n];
            if (current.SubscriberWeakReference != null && !current.SubscriberWeakReference.IsAlive) {
                _eventHandlers.RemoveAt(n);
                continue;
            }
            if (current.SubscriberWeakReference?.Target == handlerTarget && current.Handler.Name == methodName) {
                _eventHandlers.RemoveAt(n);
                break;
            }
        }
    }
    public void Invoke(object? sender, EventArgs? args) {
        List<(object? subscriber, MethodInfo handler)> toRaise = [];
        List<int> toRemove = [];
        for (int i = 0; i < _eventHandlers.Count; i++) {
            Subscription subscription = _eventHandlers[i];
            bool isStatic = subscription.SubscriberWeakReference == null;
            if (isStatic) {
                toRaise.Add((null, subscription.Handler));
            }
            object? subscriber = subscription.SubscriberWeakReference?.Target;
            if (subscriber == null) {
                toRemove.Add(i);
            }
            else {
                toRaise.Add((subscriber, subscription.Handler));
            }
        }
        for (int n = toRemove.Count - 1; n >= 0; n--) {
            int subscriptionIndex = toRemove[n];
            _eventHandlers.RemoveAt(subscriptionIndex);
        }
        for (int i = 0; i < toRaise.Count; i++) {
            (object? subcriber, MethodInfo handler) = toRaise[i];
            handler.Invoke(subcriber, [sender, args]);
        }
    }
}
public class WeakEventHandlerDict {
    private readonly Dictionary<string, List<Subscription>> _eventHandlers = new(StringComparer.OrdinalIgnoreCase);
    public void Connect(string eventName, Delegate handler) {
        if (!_eventHandlers.TryGetValue(eventName, out List<Subscription>? subscriptions)) {
            subscriptions = [];
            _eventHandlers.Add(eventName, subscriptions);
        }
        if (handler.Target == null) {
            subscriptions.Add(new(null, handler.GetMethodInfo()));
        } else {
            subscriptions.Add(new(new(handler.Target), handler.GetMethodInfo()));
        }
    }
    public void Remove(string eventName, Delegate handler) {
        if (_eventHandlers.TryGetValue(eventName, out List<Subscription>? subscriptions)) {
            for (int n = subscriptions.Count - 1; n >= 0; n--) {
                Subscription current = subscriptions[n];
                if (current.SubscriberWeakReference != null && !current.SubscriberWeakReference.IsAlive) {
                    subscriptions.RemoveAt(n);
                    continue;
                }
                if (current.SubscriberWeakReference?.Target == handler.Target && current.Handler.Name == handler.GetMethodInfo().Name) {
                    subscriptions.RemoveAt(n);
                    break;
                }
            }
        }
    }
    public void Invoke(object? sender, object? args, string eventName) {
        List<(object? subscriber, MethodInfo handler)> toRaise = [];
        List<Subscription> toRemove = [];
        if (_eventHandlers.TryGetValue(eventName, out List<Subscription>? target)) {
            for (int i = 0; i < target.Count; i++) {
                Subscription subscription = target[i];
                bool isStatic = subscription.SubscriberWeakReference == null;
                if (isStatic) {
                    toRaise.Add((null, subscription.Handler));
                }
                object? subcriber = subscription.SubscriberWeakReference?.Target;
                if (subcriber == null) {
                    toRemove.Add(subscription);
                } else {
                    toRaise.Add((subcriber, subscription.Handler));
                }
            }
            for (int i = 0; i < toRemove.Count; i++) {
                Subscription subscription = toRemove[i];
                target.Remove(subscription);
            }
        }
        for (int i = 0; i < toRaise.Count; i++) {
            (object? subcriber, MethodInfo handler) = toRaise[i];
            handler.Invoke(subcriber, [sender, args]);
        }
    }
}
