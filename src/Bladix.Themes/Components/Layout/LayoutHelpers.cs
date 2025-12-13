using System.Text.RegularExpressions;

namespace Bladix.Themes.Components.Layout
{
    public static class LayoutHelpers
    {
        private static readonly Regex _tokenRegex = new(@"\$([A-Za-z0-9_.-]+)(?::([^)\s,;]+))?", RegexOptions.Compiled);

        /// <summary>
        /// Resolve token syntax into CSS var(...) form.
        /// Examples:
        ///   "$colors.bg"                   => "var(--colors-bg)"
        ///   "$colors.bg:transparent"       => "var(--colors-bg, transparent)"
        ///   "linear-gradient($a, $b:transparent)" => "linear-gradient(var(--a), var(--b, transparent))"
        /// Raw CSS values (not starting with $) pass through unchanged.
        /// </summary>
        public static string? ResolveToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            value = value.Trim();

            // Replace all token occurrences with var(...) form
            var replaced = _tokenRegex.Replace(value, match =>
            {
                var token = NormalizeTokenName(match.Groups[1].Value);
                var fallback = match.Groups[2].Success ? match.Groups[2].Value : null;
                return fallback is null ? $"var(--{token})" : $"var(--{token}, {fallback})";
            });

            return replaced;
        }

        /// <summary>
        /// Normalize token names to css var style (dots/spaces -> hyphens, lowercased).
        /// </summary>
        public static string NormalizeTokenName(string tokenName)
        {
            if (string.IsNullOrWhiteSpace(tokenName)) return string.Empty;
            return tokenName.Trim().Replace('.', '-').Replace(' ', '-').ToLowerInvariant();
        }

        /// <summary>
        /// Map Display enum to CSS display string.
        /// Expects the enum 'Display' to exist in the same namespace.
        /// </summary>
        public static string ToCssDisplay(Display v) => v switch
        {
            Display.Inline => "inline",
            Display.Block => "block",
            Display.InlineBlock => "inline-block",
            Display.None => "none",
            _ => "initial"
        };
    }
}
