using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CsQuery;

namespace UrbanClassBooker.Controllers {
    [RoutePrefix("classes")]
    public class ClassesController : ApiController {
        private static readonly string classEndpoint = "https://urbanclimb.com.au/uc-services/member-class/login.aspx?classId={0}&url=https://urbanclimb.com.au/uc-services/member-class/enrol.aspx?classId={0}";

        [Route("{classId}/book/{memberId}")]
        [HttpGet]
        public async Task<IHttpActionResult> Book(string classId, string memberId) {
            using(var client = new HttpClient()) {
                var url = String.Format(classEndpoint, classId);

                var formContent = await client.GetStringAsync(url);
                var formCq = new CQ(formContent);
                var viewState = formCq.Select("input[name=__VIEWSTATE]").Val();
                var eventValidation = formCq.Select("input[name=__EVENTVALIDATION]").Val();
                var barcodeName = formCq.Select("#cphBody_txtBarcode").Attr("name");
                var submitName = "ctl00$cphBody$btnSubmit";// formCq.Select("#cphBody_btnSubmit").Attr("name");

                var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"__EVENTVALIDATION", eventValidation},
                    {"__VIEWSTATE", viewState},
                    {submitName, "LOGIN"},
                    {barcodeName, memberId}
                });

                using(var response = await client.PostAsync(url, content)) {
                    response.EnsureSuccessStatusCode();

                    var body = await response.Content.ReadAsStringAsync();

                    if(body.Contains("Welcome back")) {
                        return this.Ok("Everything is awesome");
                    } else {
                        return this.BadRequest($"Something is not right: \n{body}");
                    }                    
                }
            }
        }
    }
}
