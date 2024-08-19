using Azure.Core;
using Azure.Identity;
using Graphi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Diagnostics;
using System.Runtime;

namespace Graphi
{
  internal class GraphClient
  {
    private Settings settings;
    private DeviceCodeCredential deviceCodeCredential;

    public GraphClient()
    {
      IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false)
          .AddJsonFile($"appsettings.Development.json", optional: true)
          .Build();

      this.settings = config.GetRequiredSection("Settings").Get<Settings>()!;

      var options = new DeviceCodeCredentialOptions
      {
        ClientId = settings.ClientId,
        TenantId = settings.TenantId,
        DeviceCodeCallback = (info, cancel) =>
        {
          // Display the device code message to
          // the user. This tells them
          // where to go to sign in and provides the
          // code to use.
          Debug.WriteLine(info.Message);
          return Task.FromResult(0);
        },
      };

      this.deviceCodeCredential = new DeviceCodeCredential(options);

      var userClient = new GraphServiceClient(deviceCodeCredential, this.settings.GraphUserScopes);
       
      var token = this.GetTokenAsync();

      userClient.Me.RevokeSignInSessions.PostAsRevokeSignInSessionsPostResponseAsync()
    }

    private async Task GetTokenAsync()
    {
      var context = new TokenRequestContext(this.settings.GraphUserScopes);
      var response = await this.deviceCodeCredential.GetTokenAsync(context);
      var token = response.Token;
      Debug.WriteLine(token);
    }
  }
}