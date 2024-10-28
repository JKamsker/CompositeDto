using System.Linq;
using System.Text;

namespace CompositeDto.Generator.Helpers;

internal class StringHelper
{
    private static readonly char[] _illegalFilenameChars = new[] { '<', '>', ',', ':' };

    public static string EscapeFileName(string fileName) => _illegalFilenameChars
        .Aggregate(new StringBuilder(fileName), (s, c) => s.Replace(c, '_'))
        .ToString();
}