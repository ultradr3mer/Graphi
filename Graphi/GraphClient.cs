using Azure.Core;
using Azure.Identity;
using Graphi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
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

      var options = new ClientSecretCredentialOptions
      {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
      };

      if (token.Token != default(AccessToken).Token)
      {
        credential = DelegatedTokenCredential.Create((context, cancel) => token);
        this.userClient = new GraphServiceClient(credential, this.settings.GraphUserScopes);
      }
      else
      {
        credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret, options);
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
        Subject = "Dada?",
        Importance = Importance.Low,
        Body = new ItemBody
        {
          ContentType = BodyType.Html,
          Content = "Nabla!",
        },
        ToRecipients = new List<Recipient>
        {
          new Recipient
          {
            EmailAddress = new EmailAddress
            {
              Address = this.settings.Recipient,
            },
          },
        },
      };

      //var test = await this.userClient.Users[this.settings.Sender].Messages.PostAsync(requestBody);
      //System.Diagnostics.Process.Start(new ProcessStartInfo(test.WebLink) { UseShellExecute = true });

      var req = new SendMailPostRequestBody() { Message = requestBody, SaveToSentItems = true };
      await this.userClient.Users[this.settings.Sender].SendMail.PostAsync(req);
    }
  }
}