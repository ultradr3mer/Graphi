using Azure.Core;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphiConsole48.MailKit
{
  //
  // Zusammenfassung:
  //     The OAuth2 SASL mechanism.
  //
  // Hinweise:
  //     A SASL mechanism used by Google that makes use of a short-lived OAuth 2.0 access
  //     token.
  public class EvoSaslMechanismOAuth2 : SaslMechanism
  {
    private const string AuthBearer = "auth=Bearer ";

    private const string UserEquals = "user=";

    //
    // Zusammenfassung:
    //     Get the name of the SASL mechanism.
    //
    // Wert:
    //     The name of the SASL mechanism.
    //
    // Hinweise:
    //     Gets the name of the SASL mechanism.
    public override string MechanismName => "XOAUTH2";

    //
    // Zusammenfassung:
    //     Get whether or not the mechanism supports an initial response (SASL-IR).
    //
    // Wert:
    //     true if the mechanism supports an initial response; otherwise, false.
    //
    // Hinweise:
    //     Gets whether or not the mechanism supports an initial response (SASL-IR).
    //
    //     SASL mechanisms that support sending an initial client response to the server
    //     should return true.
    public override bool SupportsInitialResponse => true;

    //
    // Zusammenfassung:
    //     Initializes a new instance of the MailKit.Security.EvoSaslMechanismOauth2 class.
    //
    //
    // Parameter:
    //   credentials:
    //     The user's credentials.
    //
    // Ausnahmen:
    //   T:System.ArgumentNullException:
    //     credentials is null.
    //
    // Hinweise:
    //     Creates a new XOAUTH2 SASL context.
    public EvoSaslMechanismOAuth2(NetworkCredential credentials)
      : base(credentials)
    {
    }

    //
    // Zusammenfassung:
    //     Initializes a new instance of the MailKit.Security.EvoSaslMechanismOauth2 class.
    //
    //
    // Parameter:
    //   userName:
    //     The user name.
    //
    //   auth_token:
    //     The auth token.
    //
    // Ausnahmen:
    //   T:System.ArgumentNullException:
    //     userName is null.
    //
    //     -or-
    //
    //     auth_token is null.
    //
    // Hinweise:
    //     Creates a new XOAUTH2 SASL context.
    public EvoSaslMechanismOAuth2(string userName, string auth_token)
      : base(userName, auth_token)
    {
    }

    //
    // Zusammenfassung:
    //     Parse the server's challenge token and return the next challenge response.
    //
    // Parameter:
    //   token:
    //     The server's challenge token.
    //
    //   startIndex:
    //     The index into the token specifying where the server's challenge begins.
    //
    //   length:
    //     The length of the server's challenge.
    //
    //   cancellationToken:
    //     The cancellation token.
    //
    // Rückgabewerte:
    //     The next challenge response.
    //
    // Ausnahmen:
    //   T:System.NotSupportedException:
    //     The SASL mechanism does not support SASL-IR.
    //
    //   T:System.OperationCanceledException:
    //     The operation was canceled via the cancellation token.
    //
    //   T:MailKit.Security.SaslException:
    //     An error has occurred while parsing the server's challenge token.
    //
    // Hinweise:
    //     Parses the server's challenge token and returns the next challenge response.
    protected override byte[] Challenge(byte[] token, int startIndex, int length, CancellationToken cancellationToken)
    {
      if (base.IsAuthenticated)
      {
        return null;
      }

      string password = base.Credentials.Password;
      string userName = base.Credentials.UserName;
      //int num = 0;
      //byte[] array = new byte["user=".Length + userName.Length + "auth=Bearer ".Length + password.Length + 3];
      //for (int i = 0; i < "user=".Length; i++)
      //{
      //  array[num++] = (byte)"user="[i];
      //}

      //for (int j = 0; j < userName.Length; j++)
      //{
      //  array[num++] = (byte)userName[j];
      //}

      //array[num++] = 1;
      //for (int k = 0; k < "auth=Bearer ".Length; k++)
      //{
      //  array[num++] = (byte)"auth=Bearer "[k];
      //}

      //for (int l = 0; l < password.Length; l++)
      //{
      //  array[num++] = (byte)password[l];
      //}

      //array[num++] = 1;
      //array[num++] = 1;

      string auth = "user=" + userName + "^Aauth=Bearer " + password + "^A^A";
      var array = Encoding.UTF8.GetBytes(auth);
      base.IsAuthenticated = true;
      return array;
    }
  }
}
