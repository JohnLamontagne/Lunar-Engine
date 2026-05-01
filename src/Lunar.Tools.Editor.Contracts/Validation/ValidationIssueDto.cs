namespace Lunar.Tools.Editor.Contracts.Validation;

/// <summary>
/// A single diagnostic issue, modeled after Roslyn's diagnostic format.
/// Severity values: "error", "warning", "info"
/// </summary>
public record ValidationIssueDto(
    string FileName,
    string? FilePath,
    int Line,
    int Column,
    string DiagnosticId,
    string Message,
    string Severity
);
