namespace CodeMe.ServiceErrors.Results;

public readonly struct OkResult : IEquatable<OkResult>
{
    public bool Equals(OkResult other) => true;

    public override bool Equals(object? obj) => obj is OkResult;

    public override int GetHashCode() => 0;
}