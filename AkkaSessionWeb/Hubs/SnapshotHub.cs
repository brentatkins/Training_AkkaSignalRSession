using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace AkkaSessionWeb.Hubs
{
    public class SnapshotHub : Hub
    {
        private const string ScreenShareRole = "Screen Share";

        private string _sessioncode;
        private readonly SecurityManagerService _securityManagerService = new SecurityManagerService();

        public async Task<string> CreateSession()
        {
            var sessioncode = SnapshotHelper.CreateSession(Context.ConnectionId);
            Trace.TraceInformation($"User {Context.Request?.User.Identity.Name} created new session ({sessioncode})");

            var password = Guid.NewGuid().ToString().Substring(0, 5);

            var user = new RedingtonUser()
            {
                FirstName = "Share Screen",
                LastName = sessioncode,
                Email = sessioncode,
                CanDisable = true,
                CanSetPassword = false,
                Password = password,
                ConfirmPassword = password,
                SelectedRoles = new List<string>()
                {
                    ScreenShareRole
                },
                SharescreenId = Context.ConnectionId
            };

            var userCreatedSuccessfully = await _securityManagerService.CreateRedingtonUserAsync(user);

            var sessionInfo = new
            {
                Username = sessioncode,
                Password = userCreatedSuccessfully ? password : null,
                Error = userCreatedSuccessfully ? null : "Error in creating the Shared Screen session. Please try again."
            };

            return JsonConvert.SerializeObject(sessionInfo);
        }

        public void UpdateSnapshot(string sessionCode, string snapshot)
        {
            sessionCode = sessionCode.ToUpper();
            if (!SnapshotHelper.SessionExists(sessionCode)) return;

            Clients.Group(sessionCode).snapshotChanged(snapshot);
            SnapshotHelper.SaveSessionSnapshot(sessionCode, snapshot);
        }

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

        public async Task<string> JoinSession(string sessionCode)
        {
            sessionCode = sessionCode?.ToUpper();
            if (sessionCode == null || !SnapshotHelper.SessionExists(sessionCode)) return null;

            await Groups.Add(Context.ConnectionId, sessionCode);

            var userHostAddress = HttpContext.Current?.Request.UserHostAddress;
            Trace.TraceInformation($"Peer ({userHostAddress}) joined session ({sessionCode})");

            return SnapshotHelper.GetSessionSnapshot(sessionCode);
        }
    }
}