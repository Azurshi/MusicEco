namespace Domain.Exceptions;
public class BaseException : Exception {
    public string Type { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
    public string Affect { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string DebugInfo { get; set; } = string.Empty;
    public BaseException() { }
}