using TheSingularityWorkshop.CiAi.CogD;
var passed = 0;
var policy = new CapabilityPolicy();
var now = DateTimeOffset.Parse("2026-10-24T00:30:00Z");
var rule = new CapabilityPolicyRule("client-ci", "TrentBest/Gui", "uml.generate", "1.0.0", "TrentBest/UmlProvider", now.AddHours(-1), now.AddHours(1));
Check("default deny before any rule", () => Assert(!policy.Evaluate(Request()).IsAllowed, "policy did not default deny"));
policy.Add(rule);
Check("exact in-window scope is allowed", () => { var d = policy.Evaluate(Request()); Assert(d.IsAllowed && d.ReasonCode == "policy.allowed", "matching rule denied"); });
Check("different target or provider is denied", () => { Assert(!policy.Evaluate(Request(target: "TrentBest/Other")).IsAllowed, "wrong target allowed"); Assert(!policy.Evaluate(Request(provider: "TrentBest/Other")).IsAllowed, "wrong provider allowed"); });
Check("expired and disabled rules are denied", () => { Assert(policy.Evaluate(Request(at: now.AddHours(2))).ReasonCode == "policy.outside-validity-window", "expired request allowed"); var disabled = new CapabilityPolicy(); disabled.Add(new CapabilityPolicyRule("client-ci", "TrentBest/Gui", "uml.generate", "1.0.0", "TrentBest/UmlProvider", now.AddHours(-1), now.AddHours(1), false)); Assert(disabled.Evaluate(Request()).ReasonCode == "policy.rule-disabled", "disabled rule allowed"); });
Check("duplicate exact rules and invalid validity windows are rejected", () => { Throws<InvalidOperationException>(() => policy.Add(rule)); Throws<ArgumentException>(() => new CapabilityPolicyRule("client-ci", "TrentBest/Gui", "uml.generate", "1.0.0", "TrentBest/UmlProvider", now, now)); });
Console.WriteLine($"PASS: {passed} CogD contract checks.");
void Check(string name, Action action) { action(); passed++; Console.WriteLine($"PASS {passed}: {name}"); }
CapabilityAuthorizationRequest Request(string target="TrentBest/Gui", string provider="TrentBest/UmlProvider", DateTimeOffset? at=null) => new("client-ci", target, "uml.generate", "1.0.0", provider, at ?? now);
static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static void Throws<TException>(Action action) where TException : Exception { try { action(); } catch (TException) { return; } throw new InvalidOperationException($"Expected {typeof(TException).Name}."); }