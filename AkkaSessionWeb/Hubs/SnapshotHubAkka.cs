using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Akka.Actor;
using AkkaSessionWeb.Akka;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace AkkaSessionWeb.Hubs
{
    public class SnapshotHubAkka : Hub
    {
        private const string ScreenShareRole = "Screen Share";

        private string _sessioncode;
        private readonly SecurityManagerService _securityManagerService = new SecurityManagerService();

        public async Task<string> CreateSession()
        {
            var sessionCoordinator = SessionActors.GetSessionCordinator();
            var sessionDetails = await sessionCoordinator.Ask(new CreateSession {MasterId = Context.ConnectionId});

            return JsonConvert.SerializeObject(sessionDetails);
        }

        public async Task UpdateSnapshot(string sessionCode, string snapshot)
        {
            var sessionCoordinator = SessionActors.GetSessionCordinator();
            var session = await sessionCoordinator.Ask(new FindSession { SessionId = sessionCode });

            ((IActorRef)session).Tell(new Update {State = snapshot});

            Clients.Group(sessionCode).snapshotChanged(snapshot);
        }

        public async Task<string> JoinSession(string sessionCode)
        {
            var sessionCoordinator = SessionActors.GetSessionCordinator();
            var session = await sessionCoordinator.Ask(new FindSession {SessionId = sessionCode});

            ((IActorRef)session).Tell(new Join { ClientId = Context.ConnectionId });
            
            await Groups.Add(Context.ConnectionId, sessionCode);

            var userHostAddress = HttpContext.Current?.Request.UserHostAddress;
            Trace.TraceInformation($"Peer ({userHostAddress}) joined session ({sessionCode})");

            var latestState = await ((IActorRef)session).Ask(new GetState());

            return (string)latestState;
        }


        //change methods below to use actors
        //
        public override async Task OnDisconnected(bool stopCalled)
        {
            _sessioncode = _sessioncode ?? SnapshotHelper.GetSessionCode(Context.ConnectionId);

            Clients.Group(_sessioncode).masterDisconnected();
            SnapshotHelper.RemoveMasterSesssions(Context.ConnectionId);
            Trace.TraceInformation($"User {Context.Request?.User.Identity.Name} disconnected ({Context.ConnectionId})");
            var userDeletedSuccessfully = await _securityManagerService.DeleteScreenShareUserAsync(Context.ConnectionId);
            if (!userDeletedSuccessfully)
                Trace.TraceError($"An error occured deleting shared screen user with share screen id: {Context.ConnectionId}");
            await base.OnDisconnected(stopCalled);
        }

        public void EndSession(string sessionCode)
        {
            _sessioncode = sessionCode;
        }
    }

    public static class SessionActors
    {
        private static IActorRef _sessionCoordinator;

        public static IActorRef GetSessionCordinator()
        {
            if (_sessionCoordinator == null)
            {
                var system = ActorSystem.Create("SessionActorSystem");
                _sessionCoordinator = system.ActorOf<SessionCoordinator>("coordinator");
            }

            return _sessionCoordinator
;
        }
    }
}