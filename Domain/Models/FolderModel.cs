namespace Domain.Models;
public interface IFolderModel: IItemModel { 
    public List<IFolderModel> ChildFolders { get; }
    public List<IFileModel> ChildFiles { get; }
}