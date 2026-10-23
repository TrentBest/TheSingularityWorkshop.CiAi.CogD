# CiAi.CogD — Deterministic Vocabulary and Policy Gate

**Mission:** use ProtocolAi's actual integer-backed vocabulary to resolve known operations, then apply a host-provided allowlist. Unknown literals remain data and cannot become commands.

## 01 — See it in a minute

Requires the .NET 8 SDK and restores TheSingularityWorkshop.ProtocolAi 0.1.0-alpha.2.

    dotnet build src/CiAi.CogD/CiAi.CogD.csproj --configuration Release
    dotnet run --project tests/CiAi.CogD.Tests/CiAi.CogD.Tests.csproj --configuration Release

Offline checks cover allowed read-only/harmless operations, unknown literals, disallowed targets, missing operations, and the publishing gate.

## 02 — Policy boundary

The prototype defines three ProtocolAi symbols: observe.status, test.noop, and publish.package. Only the first two can pass the current prototype policy, and only for a target in the caller-provided allowlist. Publishing remains disabled even when the explicit-approval flag is true; that flag only demonstrates the separate approval gate.

An Allowed decision is not execution authorization. The trusted CiAi host must enforce actual permissions and call a transport only after all applicable checks pass.

## 03 — Not yet implemented

No authentication, permissions discovery, dynamic/shared vocabulary negotiation, transport, dispatch, credentials, execution, or CiAi/CogB integration. The package reference pins the current ProtocolAi alpha version; this is not yet a cross-repository version-negotiation mechanism.

## 04 — Status

**Readiness: 20%.** Policy prototype with offline checks; not a complete authorization system. See [READINESS.md](docs/READINESS.md).
