using System.Web;

namespace OAuthServer.Endpoints
{
    public class GetLogin
    {
        //se envia vista de html, al dar submit viajamos al post del login
        public static async Task GetLoginHandler(string returnUrl, HttpResponse response)
        {
            response.Headers.ContentType = new string[] { "text/html" };

            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "Template\\Login.html");
            string htmlFile = File.ReadAllText(path);
            string htmlResponse = String.Empty;

            if (htmlFile != null)
            {

                htmlResponse =  htmlFile.Replace("[ReturnUrl]", HttpUtility.UrlEncode(returnUrl));
            }

            await response.WriteAsync(htmlResponse);
            //await response.WriteAsync(
            //        $"<html> <body><form action=\"/login?returnUrl={HttpUtility.UrlEncode(returnUrl)}\" method=\"post\" ><input value=\"Submit\" type=\"submit\" ></body> <html> "
            //    );
        }
    }
}
