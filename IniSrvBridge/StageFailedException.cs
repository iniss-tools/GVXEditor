using System;

namespace IniSrvBridge;

/// <summary>
/// Krok nacitania zlyhal - server by sa nespustil. Chyba je uz zapisana vo vysledku.
/// </summary>
internal sealed class StageFailedException : Exception
{
    public StageFailedException()
    {
    }

    public StageFailedException(string message) : base(message)
    {
    }

    public StageFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
