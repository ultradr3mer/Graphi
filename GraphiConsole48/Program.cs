using Graphi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphiConsole48
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var config = AppConfigGenerator.Generate();

      var client = new GraphClient();
      client.CreateMessage().GetAwaiter().GetResult();
    }
  }
}
