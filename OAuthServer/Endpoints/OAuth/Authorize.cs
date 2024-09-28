using Microsoft.AspNetCore.DataProtection;
using OAuthServer.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace OAuthServer.Endpoints.OAuth
{
    public class Authorize
    {

        public static  IResult AuthorizeHandler(
            HttpRequest request,
            IDataProtectionProvider dataProtectionProvider)
        {

            //obtenemos valores de la request desde client
            
            request.Query.TryGetValue("response_type", out var responseType); //intentamos obtener value y lo asignamos a una variable
            request.Query.TryGetValue("client_id", out var clientId);
            request.Query.TryGetValue("code_challenge", out var codeChallenge);
            request.Query.TryGetValue("code_challenge_method", out var codeChallengeMethod);
            request.Query.TryGetValue("redirect_uri", out var redirectUri);
            request.Query.TryGetValue("scope", out var scope);
            request.Query.TryGetValue("state", out var state);

            //se usa libreria para generar un auth code grant con values protegiods,se crea model 
            var protector = dataProtectionProvider.CreateProtector("oauth");
            var authGrantCode = new AuthCodeGrant()
            {
                ClientId = clientId,
                CodeChallenge = codeChallenge,
                CodeChallengeMethod = codeChallengeMethod,
                RedirectUri = redirectUri,
                Expiry = DateTime.Now.AddMinutes(10)

            };
            
            var authCodeGrantJson = protector.Protect(JsonSerializer.Serialize(authGrantCode));
            //https://localhost:7037/oauth/custom-cb
            var redirectUriIss = redirectUri.ToString().Split("/", 4);
            var issList = redirectUriIss.Where((x, index) => index > 3).ToArray();
            string issuer = string.Join("/", issList);
            //segun doc de oauth retornamos redirect con values
            return Results.Redirect($"{redirectUri}?code={authCodeGrantJson}&state={state}&iss={HttpUtility.UrlEncode("")}");
        }
    }
}
