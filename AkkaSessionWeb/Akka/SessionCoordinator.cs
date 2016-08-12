using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;

namespace AkkaSessionWeb.Akka
{
    public class CreateSession
    {
        public string MasterId { get; set; }
    }

    public class SessionCreated
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class FindSession
    {
        public string SessionId { get; set; }
    }

    public class SessionCoordinator : ReceiveActor
    {
        private readonly Dictionary<string, IActorRef> OriginalSenders = new Dictionary<string, IActorRef>();

        public SessionCoordinator()
        {
            Receive<CreateSession>(x =>
            {
                OriginalSenders.Add(x.MasterId, Sender);

                //create new session user
                var userCreator = Context.ActorOf<SessionUserCreator>();
                userCreator.Tell(new CreateSessionUser {MasterId = x.MasterId});
            });

            Receive<UserSessionCreated>(x =>
            {
                Context.Stop(Sender);

                var sessionActor = Context.ActorOf<Session>(x.Username);
                sessionActor.Tell(new StartSession { MasterId = x.MasterId, SessionId = x.Username, Password = x.Password });
            });

            Receive<SessionStarted>(x =>
            {
                var originalSender = OriginalSenders[x.MasterId];
                originalSender.Tell(new SessionCreated { Username = x.SessionId, Password = x.Password });
            });

            Receive<FindSession>(x =>
            {
                Context.ActorSelection(x.SessionId).ResolveOne(TimeSpan.FromSeconds(1)).PipeTo(Sender);
            });
        }
    }
}