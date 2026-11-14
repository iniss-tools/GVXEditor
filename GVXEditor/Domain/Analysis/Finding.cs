namespace GVXEditor.Domain.Analysis;

/// <summary>
/// Zistenie kontroly instalacie.
/// </summary>
/// <param name="Severity">zavaznost</param>
/// <param name="Group">poradie grafikonu; -1 = cela instalacia</param>
/// <param name="Train">poradie vlaku v GV.XML; -1 = nie je vlak</param>
/// <param name="Place">kde zistenie je (grafikon, stanica, vlak)</param>
/// <param name="Message">popis</param>
internal sealed record Finding(FindingSeverity Severity, int Group, int Train, string Place, string Message);
