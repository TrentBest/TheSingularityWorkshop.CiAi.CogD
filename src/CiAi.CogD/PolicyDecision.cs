namespace CiAi.CogD;

public sealed record PolicyDecision(
    bool Allowed,
    string DecisionCode,
    string OperationLiteral,
    ulong? ProtocolSymbolId,
    string TargetRepository,
    string Explanation);

public sealed record PolicyContext(
    string TargetRepository,
    IReadOnlySet<string> AllowedTargets,
    bool ExplicitReleaseApproval = false);
