﻿using System;
using System.Web;
using System.Web.Security;

namespace CABOPMANAGEMENT.Tools
{
    
    /// <summary>
    /// SingleSessionPreparation is used to help ensure
    /// users may only have one session active
    /// </summary>
    internal static class SingleSessionPreparation
    {
        /// <summary>
        /// Called during LoggedIn event. Need to pass username
        /// as login process not fully completed
        /// </summary>
        internal static void CreateAndStoreSessionToken(string userName)
        {
            MembershipUser currentUser;
            // Will be using the response object several times
            HttpResponse pageResponse = HttpContext.Current.Response;

            // 'session' token
            Guid sessionToken = System.Guid.NewGuid();

            System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);

            // Get authentication cookie and ticket
            HttpCookie authenticationCookie = pageResponse.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt(authenticationCookie.Value);

            // Create a new ticket based on the existing one that includes the 'session' token in the userData
            FormsAuthenticationTicket newAuthenticationTicket =
                new FormsAuthenticationTicket(
                authenticationTicket.Version,
                authenticationTicket.Name,
                authenticationTicket.IssueDate,
                authenticationTicket.Expiration,
                authenticationTicket.IsPersistent,
                sessionToken.ToString(),
                authenticationTicket.CookiePath);

            // Store session token in Membership comment
            // You may want to store other information in the comment
            // field, if so, you may have to implement some dilimited
            // structure within it, perhaps xml
            currentUser = Membership.GetUser(userName);
            currentUser.Comment = sessionToken.ToString();
            Membership.UpdateUser(currentUser);

            // Replace the authentication cookie
            pageResponse.Cookies.Remove(FormsAuthentication.FormsCookieName);

            HttpCookie newAuthenticationCookie = new HttpCookie(
                FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(newAuthenticationTicket));

            newAuthenticationCookie.HttpOnly = authenticationCookie.HttpOnly;
            newAuthenticationCookie.Path = authenticationCookie.Path;
            newAuthenticationCookie.Secure = authenticationCookie.Secure;
            newAuthenticationCookie.Domain = authenticationCookie.Domain;
            newAuthenticationCookie.Expires = authenticationCookie.Expires;

            pageResponse.Cookies.Add(newAuthenticationCookie);
        }
    }
}

