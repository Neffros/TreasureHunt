namespace TreasureHunt.Exceptions;

[Serializable]
public class InitializationMapException : Exception
{
    public InitializationMapException() { }

    public InitializationMapException(string message)
        : base(message) { }

    public InitializationMapException(string message, Exception inner)
        : base(message, inner) { }
}