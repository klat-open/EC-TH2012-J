using EC_TH2012_J.App_Start;
using EC_TH2012_J.Controllers;
using EC_TH2012_J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace System.Web.Http
{
    class IdentityAuthenticationAttribute : AuthorizeAttribute
    {
         public bool Active = false;

    public IdentityAuthenticationAttribute()
    { }
    public IdentityAuthenticationAttribute(bool active)
    {
        Active = active;
    }
        
    public override void OnAuthorization(HttpActionContext actionContext)
    {
        if (Active)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "bearer")
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == null)
                {
                    Challenge(actionContext);
                    return;
                }


                if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
                {
                    Challenge(actionContext);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                Thread.CurrentPrincipal = principal;

                // inside of ASP.NET this is required
                //if (HttpContext.Current != null)
                //    HttpContext.Current.User = principal;

                //base.OnAuthorization(actionContext);
            }
        }
    }
    protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
    {
        AccountController cc =new AccountController();
        if (cc.Kiemtrataikhoan(username, password))
            return true;
        else
            return false;
    }
    protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
    {
        string authHeader = null;
        var auth = actionContext.Request.Headers.Authorization;
        if (auth != null && auth.Scheme == "Basic")
            authHeader = auth.Parameter;

        if (string.IsNullOrEmpty(authHeader))
            return null;

        authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

        var tokens = authHeader.Split(':');
        if (tokens.Length < 2)
            return null;

        return new BasicAuthenticationIdentity(tokens[0],tokens[1]);
    }

    void Challenge(HttpActionContext actionContext)
    {
        var host = actionContext.Request.RequestUri.DnsSafeHost;
        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
    }
    }
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            this.Password = password;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string Password { get; set; }
    }
}
