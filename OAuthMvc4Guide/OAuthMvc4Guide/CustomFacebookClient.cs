using DotNetOpenAuth.AspNet.Clients;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace OAuthMvc4Guide
{
    public class CustomFacebookClient : OAuth2Client
    {
        private const string AuthorizationEP = "https://www.facebook.com/dialog/oauth";
        private const string TokenEP = "https://graph.facebook.com/oauth/access_token";
        private readonly string _appId;
        private readonly string _appSecret;

        public CustomFacebookClient(string appId, string appSecret)
            : base("Facebook")
        {
            this._appId = appId;
            this._appSecret = appSecret;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            return new Uri(
                        AuthorizationEP
                        + "?client_id=" + this._appId
                        + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
                        + "&scope=email,user_about_me,user_birthday,user_location"
                        + "&display=page"
                    );
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            WebClient client = new WebClient();
            string content = client.DownloadString(
                "https://graph.facebook.com/me?access_token=" + accessToken
            );
            dynamic data = Json.Decode(content);

            Dictionary<string, string> dataDictionary = new Dictionary<string, string> {
                {
                    "id",
                    data.id
                },
                {
                    "username",
                    data.username
                },
                {
                    "name",
                    data.name
                },
                {
                    "link",
                    data.link
                },
                {
                    "gender",
                    data.gender
                },
                {
                    "birthday",
                    data.birthday
                },
                {
                    "email",
                    data.email
                },
                {
                    "photo",
                    "https://graph.facebook.com/" + data.id + "/picture"
                }
            };

            dataDictionary.Add("verified", data.verified == true ? bool.TrueString : bool.FalseString);

            return dataDictionary;

        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            WebClient client = new WebClient();
            string content = client.DownloadString(
                TokenEP
                + "?client_id=" + this._appId
                + "&client_secret=" + this._appSecret
                + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
                + "&code=" + authorizationCode
            );

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(content);
            if (nameValueCollection != null)
            {
                string result = nameValueCollection["access_token"];
                return result;
            }
            return null;
        }
    }
}