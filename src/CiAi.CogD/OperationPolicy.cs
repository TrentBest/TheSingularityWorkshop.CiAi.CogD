using TheSingularityWorkshop.ProtocolAi;

namespace CiAi.CogD;

/// <summary>Resolves operation vocabulary with ProtocolAi, then applies host-owned allowlists.</summary>
public sealed class OperationPolicy
{
    private const ulong ProtocolId = 0x43494149444F4D01;
    private readonly ProtocolDefinition _protocol;
    private readonly IReadOnlyDictionary<string, ulong> _symbolIds;

    public OperationPolicy()
    {
        _protocol = new ProtocolBuilder(ProtocolId, "CiAi.OperationPolicy")
            .Define(1, "ObserveStatus", "observe.status")
            .Define(2, "HarmlessTest", "test.noop")
            .Define(3, "PublishPackage", "publish.package")
            .Build();
        _symbolIds = _protocol.Symbols.ToDictionary(s => s.Value, s => s.Id, StringComparer.Ordinal);
    }

    public PolicyDecision Evaluate(string? operationLiteral, PolicyContext? context)
    {
        if (string.IsNullOrWhiteSpace(operationLiteral))
            return Deny("operation.required", operationLiteral ?? string.Empty, context?.TargetRepository ?? string.Empty, "Operation is missing.");
        if (context is null || context.AllowedTargets is null)
            return Deny("policy-context.required", operationLiteral, context?.TargetRepository ?? string.Empty, "Trusted host policy context is required.");
        if (string.IsNullOrWhiteSpace(context.TargetRepository) || !context.AllowedTargets.Contains(context.TargetRepository))
            return Deny("target.not-allowed", operationLiteral, context.TargetRepository, "Target is not in the host-provided allowlist.");

        if (!_symbolIds.TryGetValue(operationLiteral, out var symbolId))
            return Deny("operation.unknown-literal", operationLiteral, context.TargetRepository, "Unknown literals remain data and are never promoted to executable operations.");

        if (operationLiteral == "publish.package" && !context.ExplicitReleaseApproval)
            return new PolicyDecision(false, "release-approval.required", operationLiteral, symbolId, context.TargetRepository, "Publishing requires explicit release approval from the trusted host.");

        if (operationLiteral == "publish.package")
            return new PolicyDecision(false, "operation.not-enabled", operationLiteral, symbolId, context.TargetRepository, "This policy prototype does not enable publishing, even with an approval flag.");

        return new PolicyDecision(true, "operation.allowed", operationLiteral, symbolId, context.TargetRepository, "Known operation and target passed this prototype policy. The host must still authorize and execute it.");
    }

    private static PolicyDecision Deny(string code, string operation, string target, string explanation) =>
        new(false, code, operation, null, target, explanation);
}
