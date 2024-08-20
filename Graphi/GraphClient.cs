using Azure.Core;
using Azure.Identity;
using Graphi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Diagnostics;

namespace Graphi
{
  internal class GraphClient
  {
    private Settings settings;
    private TokenCredential credential;
    private GraphServiceClient userClient;

    public GraphClient(AccessToken token = default)
    {
      IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false)
          .AddJsonFile($"appsettings.Development.json", optional: true)
          .Build();

      this.settings = config.GetRequiredSection("Settings").Get<Settings>()!;

      var options = new InteractiveBrowserCredentialOptions
      {
        ClientId = settings.ClientId,
        TenantId = settings.TenantId,
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        // MUST be http://localhost or http://localhost:PORT
        // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/System-Browser-on-.Net-Core
        RedirectUri = new Uri("http://localhost"),
      };

      if (token.Token != default(AccessToken).Token)
      {
        credential = DelegatedTokenCredential.Create((context, cancel) => token);
        this.userClient = new GraphServiceClient(credential, this.settings.GraphUserScopes);
      }
      else
      {
        credential = new InteractiveBrowserCredential(options);
      }

      this.userClient = new GraphServiceClient(credential, this.settings.GraphUserScopes);
    }

    public async Task<AccessToken> GetTokenAsync()
    {
      var context = new TokenRequestContext(this.settings.GraphUserScopes);
      return await this.credential.GetTokenAsync(context, new CancellationToken());
    }

    public async Task CreateMessage()
    {
      var requestBody = new Message
      {
        Subject = "Did you see last night's game?",
        Importance = Importance.Low,
        Body = new ItemBody
        {
          ContentType = BodyType.Html,
          Content = "They were <b>awesome</b>!",
        },
        ToRecipients = new List<Recipient>
        {
          new Recipient
          {
            EmailAddress = new EmailAddress
            {
              Address = "schulz-theissen@eevolution.de",
            },
          },
        },
      };

      var test = await this.userClient.Me.Messages.PostAsync(requestBody);
      System.Diagnostics.Process.Start(new ProcessStartInfo(test.WebLink) { UseShellExecute = true });
    }
  }
}