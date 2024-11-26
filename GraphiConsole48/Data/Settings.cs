namespace Graphi.Data
{
  public class Settings
  {
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public string[] GraphUserScopes { get; set; }
    public string[] GraphAppScopes { get; set; }
    public string ClientSecret { get; set; }
    public string Recipient { get; set; }
    public string Sender { get; set; }
    public string SharedFolder { get; set; }
  }
}
