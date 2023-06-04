namespace GordonBeemingCom.Shared.Services;

public sealed class DeploymentInfo
{
  public DeploymentInfo()
  {
    AspNetVersion = Environment.GetEnvironmentVariable("ASPNET_VERSION") ?? Environment.Version.ToString();
    var aspnetVersionSegments = AspNetVersion.Split(".");
    DotNetLink = $"https://dotnet.microsoft.com/en-us/download/dotnet/{aspnetVersionSegments[0]}.{aspnetVersionSegments[1]}";
    CommitHash = Environment.GetEnvironmentVariable("COMMIT_HASH") ?? "local";
    if (CommitHash.Length > 6)
    {
      CommitHashText = CommitHash.Substring(0, 6);
    }
    else
    {
      CommitHashText = CommitHash;
    }
    BranchName = Environment.GetEnvironmentVariable("BRANCH_NAME") ?? "local";
  }

  public string AspNetVersion { get; }
  public string DotNetLink { get; }
  public string CommitHash { get; }
  public string CommitHashText { get; }
  public string BranchName { get; }
}
