using Domain.Models;

namespace Domain.DataAccess;
public interface IDataGetter {
    /// <summary>
    /// Get available Image, return MissingImage if not available, return DefaultImage if image not exists
    /// </summary>
    /// <param name="song"></param>
    /// <returns></returns>
    public ImageSource? Image(ISongModel song);
    /// <summary>
    /// Get available Icon, return MissingIcon if not available, return DefaultIcon if image not exists
    /// </summary>
    /// <param name="song"></param>
    /// <returns></returns>
    public ImageSource Icon(ISongModel song);
    /// <summary>
    /// Return MissingImage if not available
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public ImageSource Image(IFileModel file);
    /// <summary>
    /// Return MissingIcon if not available
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public ImageSource Icon(IFileModel file);
    /// <summary>
    /// Return DefaultImage if image not exists
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public ImageSource Image(string filePath);
    /// <summary>
    /// Return DefaultIcon if icon not exists
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public ImageSource Icon(string filePath);
    public List<string> AlbumNames();
}