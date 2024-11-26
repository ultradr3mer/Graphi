using Azure.Core;
using Azure.Identity;
using Graphi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Graphi
{
  internal class GraphClient
  {
    protected Settings settings;
    private TokenCredential credential;
    private GraphServiceClient userClient;

    public GraphClient(AccessToken token = default)
    {
      IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false)
          .AddJsonFile($"appsettings.Development.json", optional: true)
          .Build();

      this.settings = config.GetRequiredSection("Settings").Get<Settings>();


      if (this.settings.TenantId == "common")
      {
        var options = new InteractiveBrowserCredentialOptions
        {
          ClientId = this.settings.ClientId,
          TenantId = "common",
          AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
          RedirectUri = new Uri("http://localhost"),
        };

        this.credential = new InteractiveBrowserCredential(options);

        this.userClient = new GraphServiceClient(this.credential, this.settings.GraphUserScopes);
      }
      else
      {
        var options = new ClientSecretCredentialOptions
        {
          AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        this.credential = new ClientSecretCredential(this.settings.TenantId, this.settings.ClientId, this.settings.ClientSecret, options);
        this.userClient = new GraphServiceClient(this.credential, this.settings.GraphUserScopes);
      }
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
      await this.userClient.Users[this.settings.SharedFolder].SendMail.PostAsync(req);
    }
  }
}