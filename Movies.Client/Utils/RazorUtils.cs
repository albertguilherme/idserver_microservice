using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Movies.Client.Utils
{
    public static class RazorUtils
    {
        public static bool IsInRoles(this ClaimsPrincipal p, params string[] roles)
        {
            var r = p.Claims.FirstOrDefault(y => y.Type == "role");
            
            if(r == null)
                return false;

            try
            {
                var d = JsonConvert.DeserializeObject<List<string>>(r.Value);
                return roles.All(x => d?.FirstOrDefault(y => y == x) != null);
            }
            catch
            {
                return roles.All(x => r.Value == x);
            }
        }
    }
}
