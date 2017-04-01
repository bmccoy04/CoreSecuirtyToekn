
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CoreSecuirtyToekn.Middleware
{
    public class AuthMiddleWare
    {
        private readonly RequestDelegate _next;

        public AuthMiddleWare(RequestDelegate next)
        {
            _next = next;
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
            var secretKey = "mykeyisaverylargesecretssssssss";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "username"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.Integer64)
            };
            var experation = TimeSpan.FromMinutes(60);
            var expires = now.Add(experation);
            var claimsIdentity = new ClaimsIdentity(claims);
            
            // Create the JWT and write it to a string
            var jwt = new JwtSecurityTokenHandler();

            var toke = jwt.CreateEncodedJwt(
                issuer: "bryan",
                audience: "myapp",
                subject: claimsIdentity,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                issuedAt: DateTime.Now,
                signingCredentials: credentials
            );
                

            var response = new
            {
                access_token = toke,// encodedJwt,
                expires_in = (int)experation.TotalSeconds,
                user = "Bryan"
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }


    }
}