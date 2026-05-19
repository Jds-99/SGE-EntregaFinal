public class RepositorioException : Exception
{
        public RepositorioException(){}
        public RepositorioException(string? messaje) : base(messaje) {}
        public RepositorioException(string? messaje, Exception? innerException) : base(messaje,innerException){}
}