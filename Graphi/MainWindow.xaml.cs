using System.Windows;

namespace Graphi
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      _ = this.Test();
    }

    private async Task Test()
    {
      var client = new GraphClient();
      var token = await client.GetTokenAsync();

      var client2 = new GraphClient(token);
      await client2.CreateMessage();
    }
  }
}