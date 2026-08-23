namespace MusicEco;

public static class Extensions {
    public static T LoadTemplate<T>(this ContentView view, string templateKey) {
        return (T)((DataTemplate)view.Resources[templateKey]).CreateContent();
    }
    public static T LoadTemplate<T>(this ContentPage view, string templateKey) {
        return (T)((DataTemplate)view.Resources[templateKey]).CreateContent();
    }
    public static IEnumerable<Element> WalkChildrenRecursive(this Element element, bool includeSelf) {
        Queue<Element> q = new();
        q.Enqueue(element);
        while(q.Count > 0) {
            var current = q.Dequeue();
            if (includeSelf || current != element) {
                yield return current;
            }
            if (current is IVisualTreeElement visual) {
                foreach(var child in visual.GetVisualChildren()) {
                    if (child is Element childElement) {
                        q.Enqueue(childElement);
                    }
                }
            }
        }
    }
}
