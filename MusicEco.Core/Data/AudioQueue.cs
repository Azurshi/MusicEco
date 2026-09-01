using MusicEco.Core.Utility;

namespace MusicEco.Core.Data;

public class AudioQueue {
    public DateTime CreationTime { get; init; }
    public string Name { get; init; }
    public DateTime ModifiedTime { get; init; }
    public DateTime LastPlayTime { get; init; }
    public AudioEntry? Current { get; init; }
    public IReadOnlyList<AudioEntry> Audios { get; init; }
    public AudioQueue(DateTime creationTime, string name, DateTime modifiedTime, DateTime lastPlayTime, AudioEntry? current, IReadOnlyList<AudioEntry> audios) {
        this.CreationTime = creationTime;
        this.Name = name;
        this.ModifiedTime = modifiedTime;
        this.LastPlayTime = lastPlayTime;
        this.Current = current;
        this.Audios = audios;
    }
    public AudioQueue WithName(string name) {
        return new(CreationTime, name, ModifiedTime, LastPlayTime, Current, Audios);
    }
    public AudioQueue WithCurrent(AudioEntry? current) {
        return new(CreationTime, Name, ModifiedTime, LastPlayTime, current, Audios);
    }
    public AudioQueue WithModifyNow() {
        return new(CreationTime, Name, DateTime.Now, LastPlayTime, Current, Audios);
    }
    public AudioQueue WithPlayNow() {
        return new(CreationTime, Name, ModifiedTime, DateTime.Now, Current, Audios);
    }
    public AudioQueue WithAudios(AudioEntry? current, IReadOnlyList<AudioEntry> audios) {
        return new(CreationTime, Name, ModifiedTime, LastPlayTime, current, audios);
    }
    public AudioQueue Shuffle() {
        return new(CreationTime, Name, ModifiedTime, LastPlayTime, Current, Shuffler.Shuffle(Audios));
    }
    public AudioQueue Previous() {
        int index = Audios.Count;
        if (Current != null) {
            for (int i = 0; i < Audios.Count; i++) {
                if (Audios[i].Hash == Current.Hash) {
                    index = i;
                    break;
                }
            }
        }
        index = index - 1;
        if (index < 0) {
            index = Audios.Count - 1;
        }
        AudioEntry? previousCurrent = null;
        if (index < Audios.Count) {
            previousCurrent = Audios[index];
        }
        return new(CreationTime, Name, ModifiedTime, LastPlayTime, previousCurrent, Audios);
    }
    public AudioQueue Next() {
        int index = -1;
        if (Current != null) {
            for (int i = 0; i < Audios.Count; i++) {
                if (Audios[i].Hash == Current.Hash) {
                    index = i;
                    break;
                }
            }
        }
        index = index + 1;
        if (index >= Audios.Count) {
            index = 0;
        }
        AudioEntry? nextCurrent = null;
        if (index < Audios.Count) {
            nextCurrent = Audios[index];
        }
        return new(CreationTime, Name, ModifiedTime, LastPlayTime, nextCurrent, Audios);
    }
}
