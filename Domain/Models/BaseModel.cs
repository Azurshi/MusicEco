namespace Domain.Models;

public class DataLoadedEventArgs: EventArgs {

}
public class DataSavedEventArgs: EventArgs {

}

public interface IBaseModel {
    public long Id { get; set; }
    public void Save();
    public void Delete();
    public void AssignId();

}