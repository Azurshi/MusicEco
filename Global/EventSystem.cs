using MusicEco.Common;
using MusicEco.Global.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco.Global;
public static class EventSystem {
    private static readonly Dictionary<Enum, WeakEventHandler> _eventManager = [];
    public static void Initialize() {
        StaticInitialize();
        Debug.WriteLine("~~~ Mediator finished");
    }
    private static void StaticInitialize() {
        Dictionary<int, List<MethodInfo>> methodsPriority = [];
        List<MethodInfo> initilizerList = StaticInitializerAttribute.InitializerList;
        foreach (var initializer in initilizerList) {
            var priority = (initializer.GetCustomAttributes()
                .Where(a => a.GetType() == typeof(StaticInitializerAttribute))
                .ToList()[0] as StaticInitializerAttribute)!.Priority;
            if (methodsPriority.ContainsKey(priority)) {
                methodsPriority[priority].Add(initializer);
            }
            else {
                methodsPriority.Add(priority, [initializer]);
            }
        }
        List<int> priorities = methodsPriority.Keys.ToList();
        for (var i = priorities.Count - 1; i >= 0; i--) {
            Debug.WriteLine($"Priority {i}");
            var priority = priorities[i];
            var methods = methodsPriority[priority];
            foreach (var method in methods) {
                Debug.WriteLine($"---Calling {method.DeclaringType?.Name} {method.Name}");
                method.Invoke(null, null);
            }
        }
        StaticInitializerAttribute.Dispose();
    }
    public static void Connect<TEventArgs>(Common.Events.Signal command, Action<object?, TEventArgs> action) where TEventArgs : EventArgs {
        if (Common.Value.System.EventSystemBlockConnect) return;
        Debug.WriteLine($"-C- {command}");
        if (_eventManager.TryGetValue(command, out WeakEventHandler? evenHandler)) {
            if (evenHandler is WeakEventHandler<TEventArgs> parameterEventHandler) {
                parameterEventHandler.Connect(action);
            }
            else {
                throw new ArgumentException($"Mismatch event handler type {typeof(TEventArgs)} and {evenHandler.GetType()}");
            }
        }
        else {
            WeakEventHandler<TEventArgs> newHandler = new();
            newHandler.Connect(action);
            _eventManager[command] = newHandler;
        }
    }
    public static void Connect(Common.Events.Signal command, Action<object?, EventArgs> action) {
        Connect<EventArgs>(command, action);
    }
    public static void Publish<TEventArgs>(Common.Events.Signal command, object? sender, TEventArgs e) where TEventArgs : EventArgs {
        if (Common.Value.System.EventSystemBlockPublish) return;
        if (command != Common.Events.Signal.Player_Progress_Changed) {
            Debug.WriteLine($"-P- {command}");
        }
        if (_eventManager.TryGetValue(command, out WeakEventHandler? evenHandler)) {
            evenHandler.Invoke(sender, e);
        }
    }
    public static void Publish(Common.Events.Signal command, object? sender) {
        Publish<EventArgs>(command, sender, EventArgs.Empty);
    }
}
