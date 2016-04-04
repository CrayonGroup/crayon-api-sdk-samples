using System.Collections.Generic;
using System.Security.Claims;

namespace MVC6.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static void AddClaim(this ClaimsIdentity to, ClaimsPrincipal incoming, string key)
        {
            var name = incoming.FindFirst(key);
            if (name != null)
            {
                to.AddClaim(name);
            }
        }

        public static void AddClaimsFromIdentity(this ClaimsIdentity to, ClaimsPrincipal incoming, List<string> keys)
        {
            foreach (var key in keys)
            {
                to.AddClaim(incoming, key);
            }
        }
    }
}