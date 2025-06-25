namespace Domain.Models;
public enum ItemSource {
    Windows = 1,
    Androids = 2,
    GoogleDrive = 4,
    Unknown = 8,
}
public interface IItemModel: IBaseModel {
    public string Path { get; set; } 
    public string Name { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public bool Available { get;}
    public ItemSource Source { get; set; }
 
    public IFolderModel? Parent { get; set; }
    public string DynamicPath { get; }
    /// <summary>
    /// Re-evaluate <see cref="Available"/>
    /// </summary>
    public void Refresh();
}