namespace Domain.Models;
public interface IFileModel: IItemModel {
    public long Size { get; set; }
    public string Type { get; set; }
    public string Sha256 { get; set; }
    public string Extension { get; set; }
}