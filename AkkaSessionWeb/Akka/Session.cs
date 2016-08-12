using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;

namespace AkkaSessionWeb.Akka
{
    public class StartSession
    {
        public string SessionId { get; set; }

        public string MasterId { get; set; }
        public string Password { get; set; }
    }

    public class SessionStarted
    {
        public string SessionId { get; set; }

        public string Password { get; set; }

        public string MasterId { get; set; }
    }

    public class Update
    {
        public string State { get; set; }
    }

    public class Join
    {
        public string ClientId { get; set; }
    }

    public class GetState { }

    public class Session : ReceiveActor
    {
        private string _sessionId;
        private string _masterId;
        private string _state;

        public Session()
        {
            Ready();
        }

        void Ready()
        {
            Receive<StartSession>(x =>
            {
                _masterId = x.MasterId;
                _sessionId = x.SessionId;
                Become(Started);

                Sender.Tell(new SessionStarted { MasterId = x.MasterId, SessionId = x.SessionId });
            });
        }

        void Started()
        {
            Receive<Update>(x =>
            {
                var test = 1;
                _state = x.State;
            });

            Receive<Join>(x => { });

            Receive<GetState>(x =>
            {
                var test = 1;
                Sender.Tell(_state);
            });
        }
    }
}