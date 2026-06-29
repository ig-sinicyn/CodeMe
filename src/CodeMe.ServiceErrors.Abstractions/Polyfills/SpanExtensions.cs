namespace CodeMe.ServiceErrors.Polyfills;

internal static class SpanExtensions
{
    public static SpanSplitEnumerator SplitToSpans(this ReadOnlySpan<char> span, char separator)
    {
        return new SpanSplitEnumerator(span, separator);
    }

    internal ref struct SpanSplitEnumerator
    {
        private readonly char _separator;
        private ReadOnlySpan<char> _current;
        private ReadOnlySpan<char> _remaining;
        private bool _isFinished;

        public SpanSplitEnumerator(ReadOnlySpan<char> span, char separator)
        {
            _separator = separator;
            _current = default;
            _remaining = span;
            _isFinished = false;
        }

        public readonly SpanSplitEnumerator GetEnumerator()
        {
            return this;
        }

        public readonly ReadOnlySpan<char> Current => _current;

        public bool MoveNext()
        {
            if (_isFinished)
                return false;

            var index = _remaining.IndexOf(_separator);

            if (index < 0)
            {
                _current = _remaining;
                _isFinished = true;
                return true;
            }

            _current = _remaining[..index];
            _remaining = _remaining[(index + 1)..];
            return true;
        }
    }
}