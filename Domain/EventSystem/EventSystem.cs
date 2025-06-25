using System.Diagnostics;

namespace Domain.EventSystem;
/// <summary>
/// <see cref="StaticInitializerAttribute"/> priority: 1000
/// </summary>
public static class EventSystem {
    private static readonly Dictionary<Type, WeakEventHandler> _eventManager = [];
#if DEBUG
    public static readonly List<Type> PrintBlockedEvents = [];
#endif
    [StaticInitializer(1000)]
    public static void Initialize() {
        Debug.WriteLine("~~~ Mediator finished");
    }
    public static void Connect<TEventArgs>(Action<object?, TEventArgs> action) where TEventArgs : EventArgs {
        Type type = typeof(TEventArgs);
        Debug.WriteLine($"-C- {type.Name}");
        if (_eventManager.TryGetValue(type, out WeakEventHandler? evenHandler)) {
            if (evenHandler is WeakEventHandler<TEventArgs> parameterEventHandler) {
                parameterEventHandler.Connect(action);
            }
            else {
                throw new ArgumentException($"Mismatch event handler arg {type} and {evenHandler.GetType()}");
            }
        }
        else {
            WeakEventHandler<TEventArgs> newHandler = new();
            newHandler.Connect(action);
            _eventManager[type] = newHandler;
        }
    }
    public static void Connect(Action<object?, EventArgs> action) {
        Connect<EventArgs>(action);
    }
    public static void Publish<TEventArgs>(object? sender, TEventArgs e) where TEventArgs : EventArgs {
        Type type = typeof(TEventArgs);
#if DEBUG
        if (!PrintBlockedEvents.Contains(type)) {
            Debug.WriteLine($"-P- {type.Name}");
        }
#endif
        if (_eventManager.TryGetValue(type, out WeakEventHandler? evenHandler)) {
            evenHandler.Invoke(sender, e);
        }
    }
    public static void Publish(object? sender) {
        Publish<EventArgs>(sender, EventArgs.Empty);
    }
}