using Microsoft.Extensions.Options;
using OAuthServer.Models;

namespace OAuthServer.Endpoints.OAuth
{
    public class SecretKeys
    {

        public readonly JWTKey JWTKey;
        public SecretKeys(IOptions<JWTKey> options)
        {
            JWTKey = options.Value;
        }
    }
}
