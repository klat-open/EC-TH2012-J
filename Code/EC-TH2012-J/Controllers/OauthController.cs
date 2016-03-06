using EC_TH2012_J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EC_TH2012_J.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OauthController : ApiController
    {
        private static Entities db = new Entities();
        #region Helper
        public HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T data)
        {
            return Request.CreateResponse(statusCode, data);
        }

        public HttpResponseMessage CreateResponse(HttpStatusCode httpStatusCode)
        {
            return Request.CreateResponse(httpStatusCode);
        }
        #endregion
        [HttpPost]
        [Route("api/oauth/request_token")]
        public HttpResponseMessage request_tokenfunction([FromBody]Request_token param )
        {

                var tam = db.Oauths.Where(m => m.Consumer_key == param.consumer_key).FirstOrDefault();
                if(tam == null)
                {
                    return CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    string request_token = "";
                    Random rand = new Random();
                    for(int i = 0 ; i < 20 ; i++)
                    {
                        request_token += rand.Next() % 10;
                    }
                    var t = new
                    {
                        request_token = request_token
                    };

                    tam.Callback = param.callback;
                    tam.Request_token = request_token;
                    db.SaveChanges();

                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
                    httpResponseMessage.Headers.Location = new Uri(Url.Link("oauthApi", new
                    {
                        controller = "Oauth",
                        action = "authenticate",
                        id = request_token
                    }));
                    return httpResponseMessage;
                }
            
        }
        [HttpGet]
        public HttpResponseMessage authenticate(string id)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
            httpResponseMessage.Headers.Location = new Uri(Url.Link("Default", new
            {
                controller = "Xacthuc",
                action = "Kiemtra",
                id = id
            }));
            return httpResponseMessage;
        }
        [HttpPost]
        [Route("api/oauth/access_token")]
        public async Task<HttpResponseMessage> accessToken([FromBody]Access_token param)
        {
            try
            {
                var temp = db.Oauths.Where(m => m.Consumer_key == param.consumer_key && m.Request_token == param.request_token && m.Verifier_token == param.verifier_token).FirstOrDefault();
                if(temp == null)
                {
                    return CreateResponse(HttpStatusCode.NotFound, "Giá trị tham số bạn nhập sai");
                }
                else
                {
                    var request = HttpContext.Current.Request;
                    var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
                    using (var client = new HttpClient())
                    {
                        var requestParams = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("grant_type", "password"),
                            new KeyValuePair<string, string>("username","admin2"),
                            new KeyValuePair<string, string>("password","")
                        };
                        var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                        var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                        var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                        var responseCode = tokenServiceResponse.StatusCode;
                        var responseMsg = new HttpResponseMessage(responseCode)
                        {
                            Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                        };
                        return responseMsg;
                    }
                }
            }
            catch (Exception e)
            {
                return CreateResponse(HttpStatusCode.NotFound, "Giá trị tham số bạn nhập sai");
            }
        }
    }
}
