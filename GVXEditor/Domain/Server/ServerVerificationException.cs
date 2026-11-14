namespace GVXEditor.Domain.Server;

/// <summary>
/// Overenie kodom servera sa nedalo vykonat (most chyba, nespustil sa, nedokoncil sa alebo nevratil vysledok). Pricina je
/// v logu programu.
/// </summary>
internal sealed class ServerVerificationException : Exception
{
    public ServerVerificationException()
    {
    }

    public ServerVerificationException(string message) : base(message)
    {
    }

    public ServerVerificationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
