using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace UrbanClassBooker.Controllers {
    [RoutePrefix("classes")]
    public class ClassesController : ApiController {
        private static readonly string classEndpoint = "https://urbanclimb.com.au/uc-services/member-class/login.aspx?classId={0}&url=https%3a%2f%2furbanclimb.com.au%2fuc-services%2fmember-class%2fenrol.aspx%3fclassId%3d{0}";

        [Route("{classId}/book/{memberId}")]
        [HttpGet]
        public async Task<IHttpActionResult> Book(string classId, string memberId) {
            using(var client = new HttpClient()) {
                var url = String.Format(classEndpoint, classId);
                var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"__EVENTVALIDATION", "/wEdAAOj72lFH/ptfO4/n1Kx87lAj1h8Oz8w/lVuOhG12xvNE/gEIT9boQjD2miQ/22VIUV1MXOiuTdbCuQKcAWOoJBp+4XPoT4xBoQO/Hizg2qE2w=="},
                    {"__VIEWSTATE", "/wEPDwUJNTEzNDM4NzM2D2QWAmYPDxYCHgtfUGFnZUJyYW5jaAUkNjkwMzI2ZjktOThjZS00MjQ5LWJkOTEtNTNhMDY3NmExMzdiZBYCAgEPZBYCAgEPZBYGAgEPFgIeBFRleHQFB0ZpdG5lc3NkAgMPFgIfAQUhV2VkbmVzZGF5LCAyMSBEZWNlbWJlciAyMDE2IDA2OjMwZAIFDw8WAh4HVmlzaWJsZWhkFgJmDxYCHwEFIVdlZG5lc2RheSwgMjEgRGVjZW1iZXIgMjAxNiAwNjozMGRkNdwJCFcfl2IR/SPmT95qZGfDbBLNzCOQWHoCTTAJBZo="},
                    {"ctl00$cphBody$btnSubmit", "LOGIN"},
                    {"ctl00$cphBody$txtBarcode", memberId}
                });

                using(var response = await client.PostAsync(url, content)) {
                    response.EnsureSuccessStatusCode();

                    var body = await response.Content.ReadAsStringAsync();

                    if(body.Contains("Welcome back")) {
                        return this.Ok("Everything is awesome");
                    } else {
                        return this.BadRequest("Something is not right");
                    }                    
                }
            }
        }
    }
}
