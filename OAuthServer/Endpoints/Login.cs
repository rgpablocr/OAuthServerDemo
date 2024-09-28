using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Web;

namespace OAuthServer.Endpoints
{
    public class Login
    {
        //se envia vista de html, al dar submit viajamos al post del login
        public static async Task<IResult> LoginHandler(string returnUrl, HttpContext context)
        {
            await context.SignInAsync(
                "cookie",
                new System.Security.Claims.ClaimsPrincipal
                (
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                        },
                        CookieAuthenticationDefaults.AuthenticationScheme
                )));

            return Results.Redirect(returnUrl); //se retorna result al ser iresult
        }
    }
}
