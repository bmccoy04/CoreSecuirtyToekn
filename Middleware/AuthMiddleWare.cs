
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreSecuirtyToekn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CoreSecuirtyToekn.Middleware
{
    public class AuthMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;

        public AuthMiddleWare(RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var url = httpContext.Connection;
            var l = httpContext.Request.Path;
            if (httpContext.Request.Path.HasValue && httpContext.Request.Path == "/Token")
            {
                await DoTokenStuff(httpContext);
                //await httpContext.Response.WriteAsync("{Name:'Something', Text:'somethingelse'}");

            }
            else
            {

                Console.WriteLine($"Request for {httpContext.Request.Path} received ({httpContext.Request.ContentLength ?? 0} bytes)");

                // Call the next middleware delegate in the pipeline 
                await _next.Invoke(httpContext);
            }
        }

        private async Task DoTokenStuff(HttpContext context)
        {
            var now = DateTime.UtcNow;
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "username"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.Integer64)
            };

            var claimsIdentity = new ClaimsIdentity(claims);
            
            // Create the JWT and write it to a string
            var jwt = new JwtSecurityTokenHandler();

            var toke = jwt.CreateEncodedJwt(
                issuer: _options.Issuer,
                audience: _options.Audience,
                subject: claimsIdentity,
                notBefore: DateTime.Now,
                expires: now.Add(_options.Expiration),
                issuedAt: DateTime.Now,
                signingCredentials: _options.SigningCredentials
            );
                

            var response = new
            {
                access_token = toke,// encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds,
                user = "Bryan"
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }


    }
}