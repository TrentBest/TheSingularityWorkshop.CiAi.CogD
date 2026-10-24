namespace TheSingularityWorkshop.CiAi.CogD;

/// <summary>Exact allow rule; no wildcards or implicit inheritance are supported.</summary>
public sealed record CapabilityPolicyRule
{
    public CapabilityPolicyRule(string requesterIdentity, string targetRepository, string capabilityId,
        string capabilityVersion, string providerRepository, DateTimeOffset validFrom, DateTimeOffset validUntil, bool enabled = true)
    {
        RequesterIdentity = Token(requesterIdentity, nameof(requesterIdentity));
        TargetRepository = Repository(targetRepository, nameof(targetRepository));
        CapabilityId = Token(capabilityId, nameof(capabilityId), dots: true);
        CapabilityVersion = Token(capabilityVersion, nameof(capabilityVersion), dots: true);
        ProviderRepository = Repository(providerRepository, nameof(providerRepository));
        if (validUntil <= validFrom) throw new ArgumentException("Policy expiry must be later than its start.", nameof(validUntil));
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        Enabled = enabled;
    }
    public string RequesterIdentity { get; }
    public string TargetRepository { get; }
    public string CapabilityId { get; }
    public string CapabilityVersion { get; }
    public string ProviderRepository { get; }
    public DateTimeOffset ValidFrom { get; }
    public DateTimeOffset ValidUntil { get; }
    public bool Enabled { get; }
    internal string Key => string.Join("\n", RequesterIdentity, TargetRepository, CapabilityId, CapabilityVersion, ProviderRepository);

    private static string Token(string value, string parameter, bool dots = false)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 128 || value.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '-' or '_' || (dots && c == '.'))))
            throw new ArgumentException("Value must be a bounded identifier.", parameter);
        return value;
    }
    private static string Repository(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Repository is required.", parameter);
        var p = value.Split('/');
        if (p.Length != 2 || p.Any(x => x.Length is < 1 or > 100 || x.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.'))))
            throw new ArgumentException("Repository must be owner/repository.", parameter);
        return value;
    }
}

/// <summary>Exact request scope evaluated by CogD.</summary>
public sealed record CapabilityAuthorizationRequest(string RequesterIdentity, string TargetRepository,
    string CapabilityId, string CapabilityVersion, string ProviderRepository, DateTimeOffset RequestedAt);

/// <summary>Explicit policy outcome; denial is the default.</summary>
public sealed record CapabilityPolicyDecision(bool IsAllowed, string ReasonCode);

/// <summary>Exact-match allow-list. It never performs network calls or dispatches work.</summary>
public sealed class CapabilityPolicy
{
    private readonly object _gate = new();
    private readonly Dictionary<string, CapabilityPolicyRule> _rules = new(StringComparer.Ordinal);

    /// <summary>Registers one exact rule; duplicate scopes are rejected rather than silently replaced.</summary>
    public void Add(CapabilityPolicyRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        lock (_gate)
        {
            if (!_rules.TryAdd(rule.Key, rule)) throw new InvalidOperationException("An exact policy rule already exists.");
        }
    }

    /// <summary>Evaluates exact identity and time scope; every unmatched request is denied.</summary>
    public CapabilityPolicyDecision Evaluate(CapabilityAuthorizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var key = string.Join("\n", request.RequesterIdentity, request.TargetRepository, request.CapabilityId, request.CapabilityVersion, request.ProviderRepository);
        lock (_gate)
        {
            if (!_rules.TryGetValue(key, out var rule)) return new(false, "policy.no-matching-rule");
            if (!rule.Enabled) return new(false, "policy.rule-disabled");
            if (request.RequestedAt < rule.ValidFrom || request.RequestedAt >= rule.ValidUntil) return new(false, "policy.outside-validity-window");
            return new(true, "policy.allowed");
        }
    }

    /// <summary>Returns the number of explicitly registered rules.</summary>
    public int Count { get { lock (_gate) return _rules.Count; } }
}
