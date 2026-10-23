# CogD Readiness

**Readiness: 20% — ProtocolAi-backed policy prototype, not a host authorization system.**

## Implemented on development
- Uses the published TheSingularityWorkshop.ProtocolAi 0.1.0-alpha.3 API to define a deterministic operation vocabulary.
- Allows only the known prototype operations observe.status and test.noop when the target is in a host-provided allowlist.
- Unknown operation strings remain literals and are denied, never translated into executable operations.
- Publishing is denied unless explicit release approval is represented; this prototype still does not enable publishing even when the flag is true.
- Offline tests cover allowed operations, unknown operation, disallowed target, missing operation, and publishing gate.

## Not implemented
- Authentication or permission discovery.
- Dynamic vocabulary loading, vocabulary version negotiation, or integer IDs shared with CiAi.
- Transport, dispatch, credential handling, or execution.
- Policy composition with CiAi/CogB and authoritative evidence from CogC.

## Run
    dotnet run --project tests/CiAi.CogD.Tests/CiAi.CogD.Tests.csproj --configuration Release

This is a policy prototype. An Allowed decision is not authorization by itself; the trusted host must enforce its own permissions before execution.
