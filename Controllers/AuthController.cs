using CoreSecuirtyToekn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreSecuirtyToekn.Controllers
{
    public class AuthController
    {

        [Route("api/[controller]")]

        [HttpPost]
        public string Post(UserLogin userLogin)
        {
            return "token";
        }

    }
}