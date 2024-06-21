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
    public class RoomController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static RoomController()
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



        // GET: Room/List
        public ActionResult List()
        {
            //curl localhost:44395/api/roomdata/listrooms

            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/listrooms";

            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<RoomDto> Rooms = response.Content.ReadAsAsync<IEnumerable<RoomDto>>().Result;

            //Views/Room/List.cshtml
            return View(Rooms);
        }

        // GET: Room/Show/{id}
        public ActionResult Show(int id)
        {
            //curl localhost:44395/api/roomdata/findroom/{id}
            DetailsRoom ViewModel = new DetailsRoom();
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/findroom/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            RoomDto SelectedRoom = response.Content.ReadAsAsync<RoomDto>().Result;
           
            ViewModel.SelectedRoom = SelectedRoom;

            //show all related chores to this particular room
            url = "https://localhost:44395/api/choredata/listchoresforroom/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ChoreDto> RelatedChores = response.Content.ReadAsAsync<IEnumerable<ChoreDto>>().Result;
            ViewModel.RelatedChores = RelatedChores;

            //Views/Chore/Show.cshtml
            return View(ViewModel);
        }


        // GET: Room/Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Room/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Room/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Room room)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/addroom";


            string jsonpayload = jss.Serialize(room);
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

        // GET: Room/Edit/1
        [Authorize]
        public ActionResult Edit(int id)
        {

            //curl localhost:44395/api/roomdata/findroom/{id}
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/findroom/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            RoomDto SelectedRoom = response.Content.ReadAsAsync<RoomDto>().Result;


            return View(SelectedRoom);
        }

        // POST: Room/Update/1
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Room room)
        {
            GetApplicationCookie();
            try
            {

                //HttpClient client = new HttpClient();
                string url = "https://localhost:44395/api/roomdata/updateroom/" + id;

                string jsonpayload = jss.Serialize(room);
                //Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                //POST: api/RoomData/UpdateRoom/{id}
                //Header : Content-Type: application/json
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                //Views/Room/Show.cshtml
                return RedirectToAction("Show/" + id);
            }
            catch
            {
                return View();
            }
        }

        // GET: Room/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            //curl localhost:44395/api/roomdata/findroom/{id}
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/findroom/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            RoomDto SelectedRoom = response.Content.ReadAsAsync<RoomDto>().Result;

            //Views/Room/Show.cshtml
            return View(SelectedRoom);
        }


        // POST: Room/Delete/{id}
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            //curl -d "" localhost:44395/api/roomdata/deleteroom
            //HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/deleteroom/" + id;

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