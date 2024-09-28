namespace OAuthServer.Models
{
    public class AuthCodeGrant
    {
        public string ClientId { get; set; }
        public string CodeChallenge { get; set; }
        public string CodeChallengeMethod { get; set; }
        public string RedirectUri { get; set; }
        public DateTime Expiry { get; set; }
    }
}
