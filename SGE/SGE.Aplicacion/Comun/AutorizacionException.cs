public class AutorizacionException : Exception{
    public AutorizacionException(){}
    public AutorizacionException(string? messaje) : base(messaje) {}
    public AutorizacionException(string? messaje, Exception? innerException) : base(messaje,innerException){}
}