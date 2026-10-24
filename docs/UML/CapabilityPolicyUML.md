# CapabilityPolicy UML and Use Case

**Owner moniker:** The Singularity Workshop  
**Purpose:** make an explicit authorization decision before a capability request is dispatched.

```mermaid
classDiagram
    class CapabilityPolicyRule {
        +string RequesterIdentity
        +string TargetRepository
        +string CapabilityId
        +string CapabilityVersion
        +string ProviderRepository
        +DateTimeOffset ValidFrom
        +DateTimeOffset ValidUntil
        +bool Enabled
    }
    class CapabilityAuthorizationRequest
    class CapabilityPolicyDecision {
        +bool IsAllowed
        +string ReasonCode
    }
    class CapabilityPolicy {
        +Add(rule)
        +Evaluate(request) CapabilityPolicyDecision
        +int Count
    }
    CapabilityPolicy o-- CapabilityPolicyRule : contains
    CapabilityPolicy ..> CapabilityAuthorizationRequest : evaluates
    CapabilityPolicy ..> CapabilityPolicyDecision : returns
```

## Use case

1. Parent coordinator identifies the authenticated requester and exact target.
2. CogA resolves a declared capability; CogB constructs a bounded request.
3. CogD evaluates the exact rule and time window.
4. A denial stops dispatch. An allow result still requires provider and CI controls.
