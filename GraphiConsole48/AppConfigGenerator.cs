using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphiConsole48
{
  internal class AppConfigGenerator
  {
    public static string Generate()
    {
      var asms = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.dll")
                          .Select(f => Assembly.LoadFrom(f)).ToList();

      var sb = new StringBuilder();

      foreach (var a in asms)
      {
        var n = a.GetName();
        sb.AppendLine($@"    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
      <dependentAssembly>
        <assemblyIdentity name=""{n.Name}"" publicKeyToken=""{ByteArrayToString(n.GetPublicKeyToken())}"" culture=""neutral"" />
        <bindingRedirect oldVersion=""0.0.0.0-65535.65535.65535.65535"" newVersion=""{n.Version}"" />
      </dependentAssembly>
    </assemblyBinding>");
      }

      return sb.ToString();
    }

    public static string ByteArrayToString(byte[] ba)
    {
      StringBuilder hex = new StringBuilder(ba.Length * 2);
      foreach (byte b in ba)
        hex.AppendFormat("{0:x2}", b);
      return hex.ToString();
    }
  }
}
