namespace ChallengeGenerator.Exceptions;

public class MalformedArgumentException : Exception
{
    public MalformedArgumentException(string _MalformedArg) : base(DebugMessage(_MalformedArg)) {
    }

    public MalformedArgumentException(string _MalformedArg, int _ArgComponentCount) : base(DebugMessage(_MalformedArg,
        _ArgComponentCount)) {
        
    }

    static private string DebugMessage(string _MalformedArg)
        => $"Arguments must contain either [-] or [--] and [=]. This arg {_MalformedArg} doesn't.";

    static private string DebugMessage(string _MalformedArg, int _ArgComponentCount)
        => $"Arguments must be composed of a switch (--food) and values (cheese) separated by a [=]. " +
           $"Argument [{_MalformedArg}] is composed of {_ArgComponentCount} components.";
}
