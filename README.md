# The Singularity Workshop CiAi.CogD

**CogD is the default-deny policy gate for capability requests.** It evaluates exact caller, target, capability, provider, and time scope before execution can be considered.

## 01 — See it in a minute

Requires .NET 8:

```bash
dotnet run --project tests/TheSingularityWorkshop.CiAi.CogD.ContractTests --configuration Release
```

## 02 — Responsibility

CogD uses explicit exact-match allow rules. Unmatched requests, disabled rules, and requests outside the rule validity window are denied with stable reason codes. No wildcards or implicit inheritance are supported.

## 03 — Trust boundary

- Default deny.
- Repository and capability scope are exact.
- Policy permission is not evidence that provider execution succeeded.
- No network access, persistence, or execution.
- Durable policy sources, signed rules, and coordinator integration remain future work.

## Gallery

See [Gallery.md](Gallery.md) for the quick tour and verification recipe.

## 04 — UML and use case

See [CapabilityPolicy UML and use case](docs/UML/CapabilityPolicyUML.md).

See [CapabilityPolicy class companion](src/TheSingularityWorkshop.CiAi.CogD/CapabilityPolicy.md).

## 05 — Release status

**Implementation readiness: 25%. Package preparation: 25%.** Typed exact-match policy and offline denial tests are committed; authenticated identity binding, durable policy loading, review workflows, and parent-coordinator integration remain outstanding. Not a publication approval.