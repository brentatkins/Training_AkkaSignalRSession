using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkkaSessionWeb
{
    public class SnapshotSession
    {
        public string Code { get; set; }
        public string MasterId { get; set; }
        public string Snapshot { get; set; }
    }

    public static class SnapshotHelper
    {
        private static readonly List<SnapshotSession> _snapshotSessions = new List<SnapshotSession>();

        private static string GenerateSessionCode()
        {
            const int codeMin = 1000;
            const int codeMax = 9999;

            var rnd = new Random();
            var code = rnd.Next(codeMin, codeMax);

            while (_snapshotSessions.Any(snp => snp.Code == $"RED{code}")) code++;

            return $"RED{code}";
        }

        public static string CreateSession(string masterId)
        {
            var sessionCode = GenerateSessionCode();

            _snapshotSessions.Add(new SnapshotSession()
            {
                Code = sessionCode,
                MasterId = masterId
            });

            return sessionCode;
        }

        public static string GetMasterSessionCode(string masterId)
        {
            return _snapshotSessions.Where(snp => snp.MasterId == masterId).Select(snp => snp.Code).FirstOrDefault();
        }

        public static void RemoveMasterSesssions(string masterId)
        {
            _snapshotSessions.RemoveAll(x => x.MasterId == masterId);
        }

        public static bool SessionExists(string sessionCode)
        {
            return _snapshotSessions.Any(snp => snp.Code == sessionCode);
        }

        public static string GetMasterId(string sessionCode)
        {
            return _snapshotSessions.Where(snp => snp.Code == sessionCode).Select(snp => snp.MasterId).FirstOrDefault();
        }

        public static string GetSessionCode(string masterId)
        {
            return _snapshotSessions.Where(snp => snp.MasterId == masterId).Select(x => x.Code).FirstOrDefault();
        }

        public static string GetSessionSnapshot(string sessionCode)
        {
            return _snapshotSessions.Where(x => x.Code == sessionCode).Select(x => x.Snapshot).FirstOrDefault();
        }

        public static void SaveSessionSnapshot(string sessionCode, string snapshot)
        {
            var sessionItem = _snapshotSessions.FirstOrDefault(x => x.Code == sessionCode);

            if (sessionItem != null)
            {
                sessionItem.Snapshot = snapshot;
            }
        }
    }
}