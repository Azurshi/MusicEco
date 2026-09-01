namespace MusicEco.Core;

public interface IErrorHandler {
    public void HandleError(Exception ex);
}
public static class TaskExtensions {
    public static async void FireAndForgetAsync(this Task task, IErrorHandler? handler = null) {
        try {
            await task;
        }
        catch (Exception ex) {
            handler?.HandleError(ex);
        }
    }
}