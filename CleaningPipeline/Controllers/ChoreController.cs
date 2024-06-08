using CleaningPipeline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using System.Web.Script.Serialization;
using CleaningPipeline.Models.ViewModels;

namespace CleaningPipeline.Controllers
{
    public class ChoreController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        
        // GET: Chore/List
        public ActionResult List()
        {
            //curl localhost:44395/api/choredata/listchores
            
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/listchores";
            
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ChoreDto>Chores = response.Content.ReadAsAsync<IEnumerable<ChoreDto>>().Result;

            //Views/Chore/List.cshtml
            return View(Chores);
        }

        // GET: Chore/Show/{id}
        public ActionResult Show(int id)
        {
            DetailsChore ViewModel = new DetailsChore();
            //curl localhost:44395/api/choredata/findchore/{id}
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/findchore/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            ChoreDto SelectedChore = response.Content.ReadAsAsync<ChoreDto>().Result;
            ViewModel.SelectedChore = SelectedChore;


            url = "https://localhost:44395/api/roomdata/listroomsforchore/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<RoomDto> AssignedRooms = response.Content.ReadAsAsync<IEnumerable<RoomDto>>().Result;
            ViewModel.AssignedRooms = AssignedRooms;
            
            url = "https://localhost:44395/api/roomdata/listroomsnotforchore/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<RoomDto> NotAssignedRooms = response.Content.ReadAsAsync<IEnumerable<RoomDto>>().Result;
            ViewModel.NotAssignedRooms = NotAssignedRooms;


            //Views/Chore/Show.cshtml
            return View(ViewModel);
        }


        ///ASSIGN CHORE TO A ROOM

        //POST: Chore/Assign/{ChoreId}/{RoomId}
        [HttpPost]
        public ActionResult Assign(int ChoreId, int RoomId)
        {
         
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/assignchoretoroom/" + ChoreId + "/" + RoomId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Show/" + ChoreId);
        }

        //GET: Chore/Unassign/{choreid}?RoomId={roomid}
        [HttpPost]
        public ActionResult Unassign(int ChoreId, int RoomId)
        {

            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/unassignchorefromroom/" + ChoreId + "/" + RoomId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Show/" + ChoreId);
        }



        // GET: Chore/Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Chore/New
        public ActionResult New()
        {
            //GET api/ownerdata/listowners
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/ownerdata/listowners";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<OwnerDto> OwnerOptions = response.Content.ReadAsAsync<IEnumerable<OwnerDto>>().Result;

            return View(OwnerOptions);
        }

        // POST: Chore/Create
        [HttpPost]
        public ActionResult Create(Chore chore)
        {
            Debug.WriteLine("the json payload is :");
            HttpClient client = new HttpClient();
            //curl -H "Content-Type:application/json" -d @chore.json localhost:44395/api/choredata/addchore
            string url = "https://localhost:44395/api/choredata/addchore";


            string jsonpayload = jss.Serialize(chore);
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

        // GET: Chore/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateChore ViewModel = new UpdateChore();
            //curl localhost:44395/api/choredata/findchore/{id}
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/findchore/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            
            ChoreDto SelectedChore = response.Content.ReadAsAsync<ChoreDto>().Result;
            ViewModel.SelectedChore = SelectedChore;
            //Debug.WriteLine("chore received : ");
            //Debug.WriteLine(selectedchore.ChoreName);

            //all owners to choose from when updating this given chore
            url = "https://localhost:44395/api/ownerdata/listowners";
            response = client .GetAsync(url).Result;
            IEnumerable<OwnerDto> OwnerOptions = response.Content.ReadAsAsync<IEnumerable<OwnerDto>>().Result;
            ViewModel.OwnerOptions = OwnerOptions;

            return View(ViewModel);
        }

        // POST: Chore/Update/5
        [HttpPost]
        public ActionResult Update(int id, Chore chore)
        {
            try
            {
                Debug.WriteLine("The new chore info is:");
                Debug.WriteLine(chore.ChoreName);
                Debug.WriteLine(chore.ChoreDescription);
                Debug.WriteLine(chore.ChoreFrequency);
                Debug.WriteLine(chore.OwnerId);

                HttpClient client = new HttpClient();
                string url = "https://localhost:44395/api/choredata/updatechore/" + id;


                string jsonpayload = jss.Serialize(chore);
                Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                //POST: api/ChoreData/UpdateChore/{id}
                //Header : Content-Type: application/json
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                //Views/Chore/Show.cshtml
                return RedirectToAction("Show/" + id);
            }
            catch
            {
                return View();
            }
        }



        // GET: Chore/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            //curl localhost:44395/api/choredata/findchore/{id}
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/findchore/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;
            ChoreDto selectedchore = response.Content.ReadAsAsync<ChoreDto>().Result;

            //Views/Chore/Show.cshtml
            return View(selectedchore);
        }


        // POST: Chore/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            
            //curl -d "" localhost:44395/api/choredata/deletechore
                
            HttpClient client = new HttpClient();
            string url = "https://localhost:44395/api/choredata/deletechore/" + id;
            HttpContent content = new StringContent("");
            HttpResponseMessage response=client.PostAsync(url, content).Result;

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