using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinCms.Zero.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] Delimeters = { ' ', '-', '_' };

        public static string ToPascalCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '\0',
                (s, i) => new char[] { char.ToUpperInvariant(s) });
        }

        public static string ToCamelCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '\0',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return new char[] { char.ToLowerInvariant(s) };
                    }

                    return new char[] { char.ToUpperInvariant(s) };
                });
        }

        public static string ToKebabCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '-',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return new char[] { char.ToLowerInvariant(s) };
                    }

                    return new char[] { '-', char.ToLowerInvariant(s) };
                });
        }

        public static string ToSnakeCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '_',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return new char[] { char.ToLowerInvariant(s) };
                    }

                    return new char[] { '_', char.ToLowerInvariant(s) };
                });
        }

        public static string ToTrainCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '-',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return new char[] { char.ToUpperInvariant(s) };
                    }

                    return new char[] { '-', char.ToUpperInvariant(s) };
                });
        }

        private static string SymbolsPipe(
            string source,
            char mainDelimeter,
            Func<char, bool, char[]> newWordSymbolHandler)
        {
            var builder = new StringBuilder();

            bool nextSymbolStartsNewWord = true;
            bool disableFrontDelimeter = true;
            foreach (var symbol in source)
            {
                if (Delimeters.Contains(symbol))
                {
                    if (symbol == mainDelimeter)
                    {
                        builder.Append(symbol);
                        disableFrontDelimeter = true;
                    }

                    nextSymbolStartsNewWord = true;
                }
                else if (!char.IsLetterOrDigit(symbol))
                {
                    builder.Append(symbol);
                    disableFrontDelimeter = true;
                    nextSymbolStartsNewWord = true;
                }
                else
                {
                    if (nextSymbolStartsNewWord || char.IsUpper(symbol))
                    {
                        builder.Append(newWordSymbolHandler(symbol, disableFrontDelimeter));
                        disableFrontDelimeter = false;
                        nextSymbolStartsNewWord = false;
                    }
                    else
                    {
                        builder.Append(symbol);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
