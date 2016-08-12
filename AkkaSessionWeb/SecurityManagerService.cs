using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AkkaSessionWeb
{
    public class SecurityManagerService
    {
        public  Task<bool> CreateRedingtonUserAsync(RedingtonUser user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteScreenShareUserAsync(string sharescreenId)
        {
            return Task.FromResult(true);
        }
    }
}