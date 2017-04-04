using CoreSecuirtyToekn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreSecuirtyToekn.Controllers
{
    public class AuthController
    {

        [Route("api/[controller]")]

        [HttpGet]
        [Authorize]
        public string Get(){
            return "Get token secure;";
        }

        [HttpPost]
        public string Post(UserLogin userLogin)
        {
            return "token";
        }

    }
}