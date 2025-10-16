using System;
using System.Collections.Generic;
using System.Net;

namespace WebServer
{
    public class Session
    {
        public DateTime LastConnection { get; set; }
        public bool Authorized { get; set; }
        public Dictionary<string, string> Objects { get; set; }

        public Session()
        {
            Objects = new Dictionary<string, string>();
            UpdateLastConnectionTime();
        }

        public void UpdateLastConnectionTime()
        {
            LastConnection = DateTime.Now;
        }

        public bool IsExpired(int expirationInSeconds)
        {
            return (DateTime.Now - LastConnection).TotalSeconds > expirationInSeconds;
        }
    }

    public class SessionManager
    {
        protected Dictionary<IPAddress, Session> sessionMap = new Dictionary<IPAddress, Session>();

        public SessionManager()
        {
            sessionMap = new Dictionary<IPAddress, Session>();
        }

        public Session GetSession(IPEndPoint remoteEndPoint)
        {
            if (!sessionMap.TryGetValue(remoteEndPoint.Address, out var session))
            {
                session = new Session();
                sessionMap[remoteEndPoint.Address] = session;
            }
            return session;
        }
    }
}