namespace Mx.Blazor.DApp.Client.Application.Exceptions
{
    public class InitException : Exception
    {
        public InitException() : base("Initialization error occured") { }
    }
}
