using System.Text.RegularExpressions;

namespace Bladix.Themes.Layout
{
    public static class LayoutHelpers
    {
        private static readonly Regex _tokenRegex = new(@"\$([A-Za-z0-9_.-]+)(?::([^)\s,;]+))?", RegexOptions.Compiled);

        /// <summary>
        /// Convert "$token.name" -> "var(--token-name)". Raw CSS values pass through.
        /// Supports multiple tokens inside a string and an inline fallback using ":":
        ///   "$colors.bg" => "var(--colors-bg)"
        ///   "$colors.bg:transparent" => "var(--colors-bg, transparent)"
        /// </summary>
        public static string? ResolveToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            value = value.Trim();

            if (value.StartsWith("$") && _tokenRegex.IsMatch(value.Substring(1)) && !_tokenRegex.Matches(value).Count.Equals(0))
            {
                // Fall through to the general replacer below to handle fallback
            }

            // Replace all $token occurrences in the value with corresponding CSS var(...) expressions
            var replaced = _tokenRegex.Replace(value, match =>
            {
                var token = match.Groups[1].Value;
                var fallbackGroup = match.Groups[2];
                var fallback = fallbackGroup.Success ? fallbackGroup.Value : null;
                return ToCssVar(token, fallback);
            });

            return replaced;
        }

        /// <summary>
        /// Convert a token name and optional fallback into a CSS variable reference.
        /// Example: ("colors.bg", "white") => "var(--colors-bg, white)"
        /// </summary>
        public static string ToCssVar(string tokenName, string? fallback = null)
        {
            var normalized = NormalizeTokenName(tokenName);
            if (string.IsNullOrEmpty(normalized)) return string.Empty;
            return fallback is null ? $"var(--{normalized})" : $"var(--{normalized}, {fallback})";
        }

        /// <summary>
        /// Normalize token names to CSS variable style: replace dots/spaces with hyphens and lowercase.
        /// "colors.primary.100" -> "colors-primary-100"
        /// </summary>
        public static string NormalizeTokenName(string tokenName)
        {
            if (string.IsNullOrWhiteSpace(tokenName)) return string.Empty;
            return tokenName.Trim().Replace('.', '-').Replace(' ', '-').ToLowerInvariant();
        }

        public static string ToCssDisplay(Display v) => v switch
        {
            Display.Inline => "inline",
            Display.Block => "block",
            Display.InlineBlock => "inline-block",
            Display.Flex => "flex",
            Display.Grid => "grid",
            Display.Table => "table",
            Display.TableRow => "table-row",
            Display.TableCell => "table-cell",
            Display.Contents => "contents",
            Display.Initial => "initial",
            Display.Inherit => "inherit",
            Display.Revert => "revert",
            Display.Unset => "unset",
            _ => "initial"
        };
    }
}
