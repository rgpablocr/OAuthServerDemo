using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OAuthServer.Models;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace OAuthServer.Endpoints.OAuth
{
    public class Token
    {
     

        public static async Task<IResult> TokenHandler(HttpRequest request, SecretKeys keys, IDataProtectionProvider dataProtectionProvider)
        {

            var bytes = await request.BodyReader.ReadAsync();
            var content = Encoding.UTF8.GetString(bytes.Buffer); //se lee contenido de http request y se convierte a string

            //se obtiene request hacia el token endpoint
            var tokenRequest = HttpUtility.ParseQueryString(content);

            //se crea jwt con IdentityModel.JsonWebTokens, tambien se puede crear con JwtBearerDefaults.AuthenticationScheme pero eso cambia auth options del server
            var tokenHandler = new JsonWebTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(keys.JWTKey.SecretKey));

            //data protection api to unprotect code challenge
            var protector = dataProtectionProvider.CreateProtector("oauth");
            //unprotect code challenge
            var codeChallengeString = protector.Unprotect(tokenRequest["code"]);
            var authCode = JsonSerializer.Deserialize<AuthCodeGrant>(codeChallengeString);

            if (!validateCodeVerififer(authCode, tokenRequest["code_verifier"]))
            {
                return Results.BadRequest("Code Challenge does not match");
            }

            return Results.Ok(
                new
                {
                    access_token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
                    {
                        Claims = new Dictionary<string, object>() {
                            //LISTA DE CLAIMS
                            [JwtRegisteredClaimNames.Sub] = Guid.NewGuid().ToString(),
                            ["custom"] = "foo"
                        },
                        Expires = DateTime.UtcNow.AddMinutes(15),
                        TokenType= "Bearer",
                        SigningCredentials  = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)

                    })
                }
              );

        }

        private static bool validateCodeVerififer(AuthCodeGrant authCode, string codeVerifier)
        {
            //we verify the code challenge => BASE64URLENCODE (SHA256(ASCII(CODE VERIFIER)))
            using var sha256 = SHA256.Create();

            var codeChallenge = Base64UrlEncoder.Encode(sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)));

            return authCode.CodeChallenge == codeChallenge;
        }
    }
}
