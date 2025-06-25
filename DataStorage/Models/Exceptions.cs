namespace DataStorage.Models;
using Domain.Exceptions;

public class DataException : BaseException {
    public DataException() {
        this.Type = "Data";
    }
}
