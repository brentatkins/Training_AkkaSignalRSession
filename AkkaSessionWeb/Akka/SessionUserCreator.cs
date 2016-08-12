using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;

namespace AkkaSessionWeb.Akka
{
    public class CreateSessionUser
    {
        public string MasterId { get; set; }
    }

    public class UserSessionCreated
    {
        public string MasterId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SessionUserCreator : ReceiveActor
    {
        public SessionUserCreator()
        {
            Receive<CreateSessionUser>(x =>
            {
                //asyncronous task that creates user in security db
                Sender.Tell(new UserSessionCreated { MasterId = x.MasterId, Username = GenerateSessionCode(), Password = "password" });
            });
        }

        private static string GenerateSessionCode()
        {
            const int codeMin = 1000;
            const int codeMax = 9999;

            var rnd = new Random();
            var code = rnd.Next(codeMin, codeMax);

            return $"RED{code}";
        }
    }
}