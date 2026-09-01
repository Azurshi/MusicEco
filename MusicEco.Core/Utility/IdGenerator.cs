namespace MusicEco.Core.Utility;

public class IdGenerator {
    private readonly object _lock = new();
    private long _lastTimeStamp = -1;
    private long _sequence = 0;
    private const long MaxSequence = 999;
    private long GetId() {
        lock (_lock) {
            long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeStamp == _lastTimeStamp) {
                _sequence++;
                if (_sequence > MaxSequence) {
                    timeStamp = WaitNextMs(_lastTimeStamp);
                    _sequence = 0;
                }
            }
            else {
                _sequence = 0;
            }
            _lastTimeStamp = timeStamp;
            return timeStamp * 1000 + _sequence;
        }
    }
    private static long WaitNextMs(long lastTimeStamp) {
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        while (timeStamp <= lastTimeStamp) {
            Thread.Sleep(1);
            timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        return timeStamp;
    }
    private static readonly Dictionary<Type, IdGenerator> _generators = [];
    public static long GetId(Type type) {
        if (!_generators.TryGetValue(type, out IdGenerator? generator)) {
            generator = new IdGenerator();
            _generators[type] = generator;
        } 
        return generator.GetId();

    }
    public static long GetId<T>() where T: class {
        return GetId(typeof(T));
    }
}