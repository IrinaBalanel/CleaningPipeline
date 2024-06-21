using CleaningPipeline.Models;
using CleaningPipeline.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CleaningPipeline.Controllers
{
    public class OwnerController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static OwnerController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
            client = new HttpClient(handler);
        }

        /// <summary>
        /// Gets the authentication cookie sent to this controller
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token
            //pass along to the WebAPI
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Owner/List
        public ActionResult List()
        {
            //curl localhost:44395/api/ownerdata/listowners

            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/listowners";

            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<OwnerDto> Owners = response.Content.ReadAsAsync<IEnumerable<OwnerDto>>().Result;

            //Views/Owner/List.cshtml
            return View(Owners);
        }

        // GET: Owner/Show/{id}
        public ActionResult Show(int id)
        {
            //curl localhost:44395/api/ownerdata/findowner/{id}
            DetailsOwner ViewModel = new DetailsOwner();
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/findowner/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            OwnerDto SelectedOwner = response.Content.ReadAsAsync<OwnerDto>().Result;
            ViewModel.SelectedOwner = SelectedOwner;

            //show all related chores to this particular owner
            url = "https://localhost:44395/api/choredata/listchoresforowner/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ChoreDto> RelatedChores = response.Content.ReadAsAsync<IEnumerable<ChoreDto>>().Result;
            ViewModel.RelatedChores = RelatedChores;

            //Views/Chore/Show.cshtml
            return View(ViewModel);
        }

        // GET: Owner/Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Owner/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Owner/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Owner owner)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/addowner";


            string jsonpayload = jss.Serialize(owner);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Owner/Edit/1
        [Authorize]
        public ActionResult Edit(int id)
        {

            //curl localhost:44395/api/ownerdata/findowner/{id}
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/findowner/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            OwnerDto selectedowner = response.Content.ReadAsAsync<OwnerDto>().Result;
            //Debug.WriteLine("owner received : ");
            //Debug.WriteLine(selectedowner.OwnerName);

            return View(selectedowner);
        }

        // POST: Owner/Update/1
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Owner owner)
        {
            GetApplicationCookie();
            try
            {
                //Debug.WriteLine("The new chore info is:");
                //Debug.WriteLine(owner.OwnerName);
                //Debug.WriteLine(owner.OwnerAvailability);

                //HttpClient client = new HttpClient();
                string url = "https://localhost:44395/api/ownerdata/updateowner/" + id;

                string jsonpayload = jss.Serialize(owner);
                //Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                //POST: api/OwnerData/UpdateOwner/{id}
                //Header : Content-Type: application/json
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                //Views/Owner/Show.cshtml
                return RedirectToAction("Show/" + id);
            }
            catch
            {
                return View();
            }
        }

        // GET: Owner/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            //curl localhost:44395/api/ownerdata/findowner/{id}
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/findowner/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            OwnerDto selectedowner = response.Content.ReadAsAsync<OwnerDto>().Result;

            //Views/Owner/Show.cshtml
            return View(selectedowner);
        }


        // POST: Owner/Delete/{id}
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            //curl -d "" localhost:44395/api/ownerdata/deleteowner
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/deleteowner/" + id;
            
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }



        }
    }
}