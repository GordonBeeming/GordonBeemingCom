using System.Text.RegularExpressions;

namespace GordonBeemingCom.Editor;

public partial class RegexHelpers
{
  [GeneratedRegex("wlEmoticon wlEmoticon-(\\w+)\"[^>]*src=\"([^\\\"]+)", RegexOptions.IgnoreCase)]
  internal static partial Regex WlEmoticonPattern();
}
