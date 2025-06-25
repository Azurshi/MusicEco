using Domain.Exceptions;
namespace Domain.DataAccess;
public class KeyNotFoundException : BaseException {
    public KeyNotFoundException(Type type, long key) {
        this.Type = "Data error";
        this.Info = $"{type} with key: {key} not found";
    }
}