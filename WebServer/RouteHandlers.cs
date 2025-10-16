using System;
using System.Collections.Generic;

namespace WebServer
{
    public abstract class RouteHandler
    {
        protected Func<Session, Dictionary<string, string>, ResponsePacket> handler;

        public RouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
        {
            this.handler = handler;
        }

        public abstract ResponsePacket Handle(Session session, Dictionary<string, string> parms);
    }

    public class AnonymousRouteHandler : RouteHandler
    {
        public AnonymousRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
            : base(handler)
        {
        }

        public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
        {
            return handler(session, parms);
        }
    }

    public class AuthenticatedRouteHandler : RouteHandler
    {
        public AuthenticatedRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
            : base(handler)
        {
        }

        public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
        {
            return session.Authorized ? handler(session, parms) : Server.Redirect(Server.onError(ServerError.NotAuthorized));
        }
    }

    public class AuthenticatedExpirableRouteHandler : AuthenticatedRouteHandler
    {
        public AuthenticatedExpirableRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
            : base(handler)
        {
        }

        public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
        {
            if (session.IsExpired(Server.expirationTimeSeconds))
            {
                session.Authorized = false;
                return Server.Redirect(Server.onError(ServerError.ExpiredSession));
            }
            return base.Handle(session, parms);
        }
    }
}