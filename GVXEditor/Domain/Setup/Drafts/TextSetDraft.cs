using System.ComponentModel;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>
/// Sada textov tabul (TTTrasy.xml, TTStanice.xml): nazov, priznak tyfloservisu a udaje predloh, v ktorych sa texty prejavia.
/// </summary>
internal abstract class TextSetDraft
{
    /// <summary>Nazov sady.</summary>
    public string Name { get; set; } = "";

    /// <summary>Text je pre tyfloservis.</summary>
    public bool Fonet { get; set; }

    /// <summary>Udaje predloh tabul.</summary>
    public BindingList<TargetDraft> Targets { get; } = [];
}
