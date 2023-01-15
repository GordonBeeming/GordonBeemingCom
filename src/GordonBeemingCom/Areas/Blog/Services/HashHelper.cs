using System.Security.Cryptography;

namespace GordonBeemingCom.Areas.Blog.Services;

public sealed class HashHelper
{
  public enum Algorithms
  {
    MD5 = 1,
    SHA1 = 2,
    SHA256 = 3,
    SHA384 = 4,
    SHA512 = 5,
  }

  public string GetHashFromFile(string fileName, Algorithms algorithm, int bufferSizeInMb = 100)
  {
    return GetHashFromStream(File.OpenRead(fileName), algorithm, bufferSizeInMb);
  }

  public string GetHashOfString(string value, Algorithms algorithm, int bufferSizeInMb = 100)
  {
    return GetHashFromBytes(GetBytes(value), algorithm, bufferSizeInMb);
  }

  public string GetHashFromBytes(byte[] bytes, Algorithms algorithm, int bufferSizeInMb = 100)
  {
    using (var ms = new MemoryStream(bytes))
    {
      return GetHashFromStream(ms, algorithm, bufferSizeInMb);
    }
  }

  public string GetHashFromStream(Stream ms, Algorithms algorithm, int bufferSizeInMb = 100)
  {
    using (var stream = new BufferedStream(ms, 1024 * 1024 * bufferSizeInMb))
    {
      return BitConverter.ToString(GetHashAlgorithm(algorithm).ComputeHash(stream)).Replace("-", string.Empty);
    }
  }

  private HashAlgorithm GetHashAlgorithm(Algorithms algorithm)
  {
    switch (algorithm)
    {
#pragma warning disable SYSLIB0021 // Type or member is obsolete
      case Algorithms.MD5:
        return new MD5CryptoServiceProvider();
      case Algorithms.SHA1:
        return new SHA1Managed();
      case Algorithms.SHA256:
        return new SHA256Managed();
      case Algorithms.SHA384:
        return new SHA384Managed();
      case Algorithms.SHA512:
        return new SHA512Managed();
#pragma warning restore SYSLIB0021 // Type or member is obsolete
    }
    throw new NotImplementedException();
  }

  private byte[] GetBytes(string value)
  {
    byte[] bytes = new byte[value.Length * sizeof(char)];
    System.Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
    return bytes;
  }
}
