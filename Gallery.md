# CogD Gallery

CogD is a small, dependency-free CiAi control-plane component. It makes one responsibility explicit and testable instead of hiding policy, identity, correlation, or lifecycle assumptions in coordinator code.

## See it in a minute

```bash
dotnet run --project tests/TheSingularityWorkshop.CiAi.CogD.ContractTests --configuration Release
```

## Recipe

- [CapabilityPolicy UML and use case](docs/UML/CapabilityPolicyUML.md)
- [CapabilityPolicy class companion](src/TheSingularityWorkshop.CiAi.CogD/CapabilityPolicy.md)
- [CI contract verification](https://github.com/TrentBest/TheSingularityWorkshop.CiAi.CogD/actions)

This is a contract demonstration, not a claim of end-to-end CiAi orchestration.
