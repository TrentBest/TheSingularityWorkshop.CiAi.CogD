using CiAi.CogD;

var policy = new OperationPolicy();
var allowed = new PolicyContext("TrentBest/Target", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TrentBest/Target" });
Require(policy.Evaluate("observe.status", allowed).Allowed, "known read-only operation should be allowed by prototype policy");
var caseSensitiveAllowlist = new PolicyContext("trentbest/target", new HashSet<string> { "TrentBest/Target" });
Require(policy.Evaluate("observe.status", caseSensitiveAllowlist).Allowed, "repository allowlist matching must be case-insensitive regardless of collection comparer");
Require(policy.Evaluate("test.noop", allowed).Allowed, "known harmless test operation should be allowed by prototype policy");
Require(!policy.Evaluate("delete.repository", allowed).Allowed, "unknown literal must not execute");
Require(policy.Evaluate("delete.repository", allowed).DecisionCode == "operation.unknown-literal", "unknown literal should have explicit denial reason");
Require(policy.Evaluate("observe.status", allowed with { TargetRepository = "TrentBest/Other" }).DecisionCode == "target.not-allowed", "unknown target must be denied");
Require(policy.Evaluate("publish.package", allowed).DecisionCode == "release-approval.required", "publish requires explicit release approval");
Require(!policy.Evaluate("publish.package", allowed with { ExplicitReleaseApproval = true }).Allowed, "prototype must not enable publishing");
Require(policy.Evaluate(null, allowed).DecisionCode == "operation.required", "missing operation must be denied");
Console.WriteLine("PASS: CogD policy checks");
return;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
