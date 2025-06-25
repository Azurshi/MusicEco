namespace MusicEco;
public delegate void VoidEventHandler();
public interface IErrorHandler {
    public void HandleError(Exception ex);
}
public static class TaskUtilities {
    public static async void FireAndForgetAsync(this Task task, IErrorHandler? handler = null) {
        try {
            await task;
        }
        catch (Exception ex) {
            handler?.HandleError(ex);
        }
    }
}
