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
        // GET: Room/List
        public ActionResult List()
        {
            //curl localhost:44395/api/roomdata/listrooms

            HttpClient client = new HttpClient();
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
            HttpClient client = new HttpClient();
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
        public ActionResult New()
        {
            return View();
        }

        // POST: Room/Create
        [HttpPost]
        public ActionResult Create(Room room)
        {
            Debug.WriteLine("the json payload is :");
            HttpClient client = new HttpClient();
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
        public ActionResult Edit(int id)
        {

            //curl localhost:44395/api/roomdata/findroom/{id}
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/findroom/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            RoomDto SelectedRoom = response.Content.ReadAsAsync<RoomDto>().Result;


            return View(SelectedRoom);
        }

        // POST: Room/Update/1
        [HttpPost]
        public ActionResult Update(int id, Room room)
        {
            try
            {

                HttpClient client = new HttpClient();
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
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/roomdata/findroom/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            RoomDto SelectedRoom = response.Content.ReadAsAsync<RoomDto>().Result;

            //Views/Room/Show.cshtml
            return View(SelectedRoom);
        }


        // POST: Room/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //curl -d "" localhost:44395/api/roomdata/deleteroom
            HttpClient client = new HttpClient();
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