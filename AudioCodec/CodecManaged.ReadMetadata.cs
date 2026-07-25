using AudioCodec.Managed;
using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec;

public static partial class CodecManaged {
    public static unsafe Dictionary<string, string> ReadMetadata(Stream stream) {
        int bufferSize = 64 * 1024;
        using (var managedFormat = new FormatFromStream(stream, bufferSize)) {
            AVFormatContext* format = managedFormat.Context;
            Dictionary<string, string> metadata = new(StringComparer.OrdinalIgnoreCase);
            AVDictionaryEntry* previous = null;
            while (true) {
                AVDictionaryEntry* entry = FFmpeg.Util.av_dict_iterate(format->Metadata, previous);
                if (entry == null) {
                    break;
                }
                string key = Marshal.PtrToStringUTF8((nint)entry->Key) ?? string.Empty;
                string value = Marshal.PtrToStringUTF8((nint)entry->Value) ?? string.Empty;
                previous = entry;
                metadata[key] = value;
            }
            return metadata;
        }
    }
}
