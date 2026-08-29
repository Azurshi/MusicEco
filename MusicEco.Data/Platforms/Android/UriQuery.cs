using Android;
using Android.Provider;
using AndroidF = Android;
using MusicEco.Core.Data;
using Uri = Android.Net.Uri;

namespace MusicEco.Data.Platforms.Android;
public abstract class ItemInfo {
    public Uri Uri;
    public ItemInfo(Uri uri) {
        this.Uri = uri;
    }
}
public class FileInfo: ItemInfo {
    public string Name;
    public string Path;
    public DateTime LastWriteTimeUtc;
    public long Length;
    public FileInfo(Uri uri, string name, string path, DateTime lastWriteTimeUtc, long length): base(uri) {
        this.Name = name;
        this.Path = path;
        this.LastWriteTimeUtc = lastWriteTimeUtc;
        this.Length = length;
    }
}
public class FolderInfo: ItemInfo {
    public FolderInfo(Uri uri) : base(uri) {
    }
}
public static class UriQuery {
    private const string FolderMimeType = "vnd.android.document/directory";
    public static List<ItemInfo> GetItemsInfo(Uri folder) {
        var treeUri = folder;
        List<ItemInfo> items = [];
        var context = AndroidF.App.Application.Context;
        var resolver = context.ContentResolver!;
        string documentId;
        if (DocumentsContract.IsDocumentUri(context, treeUri)) {
            documentId = DocumentsContract.GetDocumentId(treeUri)!;
        }
        else {
            documentId = DocumentsContract.GetTreeDocumentId(treeUri)!;
        }
        var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree(treeUri, documentId)!;
        string[] projection = [
            DocumentsContract.Document.ColumnDocumentId,
            DocumentsContract.Document.ColumnMimeType,
            DocumentsContract.Document.ColumnDisplayName,
            DocumentsContract.Document.ColumnLastModified,
            DocumentsContract.Document.ColumnSize
            ];
        var cursor = resolver.Query(
            childrenUri,
            projection,
            null,
            null,
            null);
        if (cursor == null) {
            return items;
        }
        try {
            int idIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnDocumentId);
            int mimeIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnMimeType);
            int nameIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnDisplayName);
            int modifiedIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnLastModified);
            int sizeIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnSize);
            while (cursor.MoveToNext()) {
                string childDocumentId = cursor.GetString(idIndex)!;
                string mimeType = cursor.GetString(mimeIndex)!;
                string name = cursor.GetString(nameIndex)!;
                bool isFolder = mimeType == FolderMimeType;
                var childUri = DocumentsContract.BuildDocumentUriUsingTree(treeUri, childDocumentId);
                if (isFolder) {
                    FolderInfo info = new(childUri!);
                    items.Add(info);
                }
                else {
                    DateTime? modifiedTime = null;
                    if (!cursor.IsNull(modifiedIndex)) {
                        var ms = cursor.GetLong(modifiedIndex);
                        modifiedTime = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
                    }
                    long size = 0;
                    if (!cursor.IsNull(sizeIndex)) {
                        size = cursor.GetLong(sizeIndex);
                    }
                    FileInfo info = new(
                        childUri!,
                        name,
                        childUri!.ToString()!,
                        modifiedTime ?? DateTime.MinValue,
                        size);
                    items.Add(info);
                }
            }
            return items;
        }
        finally {
            cursor.Close();
            cursor.Dispose();
        }
    }
    public static List<FileInfo> GetFilesInfo(Uri folder) {
        var treeUri = folder;
        List<FileInfo> items = [];
        var context = AndroidF.App.Application.Context;
        var resolver = context.ContentResolver!;
        string documentId;
        if (DocumentsContract.IsDocumentUri(context, treeUri)) {
            documentId = DocumentsContract.GetDocumentId(treeUri)!;
        }
        else {
            documentId = DocumentsContract.GetTreeDocumentId(treeUri)!;
        }
        var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree(treeUri, documentId)!;
        string[] projection = [
            DocumentsContract.Document.ColumnDocumentId,
            DocumentsContract.Document.ColumnMimeType,
            DocumentsContract.Document.ColumnDisplayName,
            DocumentsContract.Document.ColumnLastModified,
            DocumentsContract.Document.ColumnSize
            ];
        var cursor = resolver.Query(
            childrenUri,
            projection,
            null,
            null,
            null);
        if (cursor == null) {
            return items;
        }
        try {
            int idIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnDocumentId);
            int mimeIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnMimeType);
            int nameIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnDisplayName);
            int modifiedIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnLastModified);
            int sizeIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnSize);
            while (cursor.MoveToNext()) {
                string childDocumentId = cursor.GetString(idIndex)!;
                string mimeType = cursor.GetString(mimeIndex)!;
                string name = cursor.GetString(nameIndex)!;
                bool isFolder = mimeType == FolderMimeType;
                if (isFolder) {
                    continue;
                }
                DateTime? modifiedTime = null;
                if (!cursor.IsNull(modifiedIndex)) {
                    var ms = cursor.GetLong(modifiedIndex);
                    modifiedTime = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
                }
                long size = 0;
                if (!cursor.IsNull(sizeIndex)) {
                    size = cursor.GetLong(sizeIndex);
                }
                var childUri = DocumentsContract.BuildDocumentUriUsingTree(treeUri, childDocumentId);
                items.Add(new(
                    childUri!,
                    name,
                    childUri!.ToString()!,
                    modifiedTime ?? DateTime.MinValue,
                    size));
            }
            return items;
        }
        finally {
            cursor.Close();
            cursor.Dispose();
        }
    }
}
