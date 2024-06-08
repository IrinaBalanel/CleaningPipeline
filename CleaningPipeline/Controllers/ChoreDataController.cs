using CleaningPipeline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.Description;
using System.Diagnostics;

namespace CleaningPipeline.Controllers
{
    public class ChoreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>
        ///Lists the chores in the db along with assigned owners and returns 200(OK)
        ///</summary>
        ///<returns>An array of chores objects</returns>
        ///<example>
        ///GET: api/ChoreData/ListChores -> 
        ///[{"ChoreId":1, "ChoreName":"Mop the floor", "ChoreDescription":"Use liquid detergent for mopping", "ChoreFrequency":"Weekly", "OwnerId":2, "OwnerName":"Andrew"},
        ///{"ChoreId":2, "ChoreName":"Make a bed", "ChoreDescription":"Cover the bed with decorative blanket", "ChoreFrequency":"Daily", "OwnerId":1, "OwnerName":"Irina"}]
        ///</example>

        [ResponseType(typeof(ChoreDto))]
        [HttpGet]
        [Route("api/ChoreData/ListChores")]
        public IHttpActionResult ListChores()
        {
            List<Chore> Chores = db.Chores.ToList();
            List<ChoreDto> ChoreDtos = new List<ChoreDto>();

            Chores.ForEach(c => ChoreDtos.Add(new ChoreDto()
            {
                ChoreId = c.ChoreId,
                ChoreName = c.ChoreName,
                ChoreDescription = c.ChoreDescription,
                ChoreFrequency = c.ChoreFrequency,
                OwnerName = c.Owner.OwnerName
            }));

            return Ok(ChoreDtos);
        }

        ///<summary>
        /// Finds a chore in db by chore id and output information associated with this specific chore + 200(OK)
        ///</summary>
        ///<returns>An object of a chore with diven id</returns>
        ///<param name="id">The primary key of the chore</param>
        ///<example>
        ///GET: api/ChoreData/FindChore/8 -> 
        ///{"ChoreId":9, "ChoreName":"Mop the floor", "ChoreDescription":"Use liquid detergent for mopping", "ChoreFrequency":"Weekly", "OwnerId":2, "OwnerName":"Andrew"}
        ///</example>

        [ResponseType(typeof(Chore))]
        [HttpGet]
        [Route("api/ChoreData/FindChore/{id}")]
        public IHttpActionResult FindChore(int id)
        {
            Chore Chore = db.Chores.Find(id);
            ChoreDto ChoreDto = new ChoreDto()
            {
                ChoreId = Chore.ChoreId,
                ChoreName = Chore.ChoreName,
                ChoreDescription = Chore.ChoreDescription,
                ChoreFrequency = Chore.ChoreFrequency,
                OwnerName = Chore.Owner.OwnerName
            };

            if (Chore == null)
            {
                return NotFound();
            }

            return Ok(ChoreDto);
        }

        ///<summary>
        /// Adds a chore to the db
        ///</summary>
        ///<returns>chore Id and chore data with status 201(created) or 400(Bad request)</returns>
        ///<param name="chore">JSON form data of a chore</param>
        ///<example>
        ///POST: api/ChoreData/AddChore 
        /// FORM DATA: Chore JSON data: {"ChoreId":9, "ChoreName":"Mop the floor", "ChoreDescription":"Use liquid detergent for mopping", "ChoreFrequency":"Weekly", "OwnerId":2, "OwnerName":"Andrew"}
        ///</example>

        [ResponseType(typeof(Chore))]
        [HttpPost]
        [Route("api/ChoreData/AddChore")]

        public IHttpActionResult AddChore(Chore chore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Chores.Add(chore);
            db.SaveChanges();

            return Ok();

            /*return CreatedAtRoute("DefaultApi", new { id = chore.ChoreId }, chore);*/
        }

        ///<summary>
        /// Deletes a chore from the db by it's ID
        ///</summary>
        ///<returns>status 200(Ok) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the chore</param>
        ///<example>
        ///POST: api/ChoreData/DeleteChore/8
        ///</example>

        [ResponseType(typeof(Chore))]
        [HttpPost]
        [Route("api/ChoreData/DeleteChore/{id}")]

        public IHttpActionResult DeleteChore(int id)
        {
            Chore chore = db.Chores.Find(id);
            if (chore == null)
            {
                return NotFound();
            }

            db.Chores.Remove(chore);
            db.SaveChanges();

            return Ok();
        }


        ///<summary>
        /// Updates a particular chore by it's ID in the db with JSON post data
        ///</summary>
        ///<returns>status 204(Success) or 400(Bad request) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the chore</param>
        ///<param name="chore">JSON post data of a chore to update</param>
        ///<example>
        ///POST: api/ChoreData/UpdateChore/8
        ///</example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/ChoreData/UpdateChore/{id}")]
        public IHttpActionResult UpdateChore(int id, Chore chore)
        {
            
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != chore.ChoreId)
            {

                return BadRequest();
            }

            db.Entry(chore).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChoreExists(id))
                {
                    Debug.WriteLine("Chore not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);

         
        }



        private bool ChoreExists(int id)
        {
            return db.Chores.Count(e => e.ChoreId == id) > 0;
        }

        /// <summary>
        /// Reflects information about all chores related to a particular owner id
        /// </summary>
        /// <returns>
        /// Returns all chores in the database, including their associated owners matched with a particular owner id along with status 200(OK)
        /// </returns>
        /// <param name="id">Owner Id</param>
        /// <example>
        /// GET: api/ChoreData/ListChoresForOwner/1
        /// </example>
        [HttpGet]
        [Route("api/ChoreData/ListChoresForOwner/{id}")]
        [ResponseType(typeof(ChoreDto))]
        public IHttpActionResult ListChoresForOwner(int id)
        {
            
            List<Chore> Chores = db.Chores.Where(a => a.OwnerId == id).ToList();
            List<ChoreDto> ChoreDtos = new List<ChoreDto>();

            Chores.ForEach(c => ChoreDtos.Add(new ChoreDto()
            {
                ChoreId = c.ChoreId,
                ChoreName = c.ChoreName,
                ChoreDescription = c.ChoreDescription,
                ChoreFrequency = c.ChoreFrequency,
                OwnerName = c.Owner.OwnerName
            }));

            return Ok(ChoreDtos);
        }

        /// <summary>
        /// Reflects information about all chores related to a particular room id
        /// </summary>
        /// <returns>
        /// Returns all chores in the database matched with a particular room id along with status 200(OK)
        /// </returns>
        /// <param name="id">Room Id</param>
        /// <example>
        /// GET: api/ChoreData/ListChoresForRoom/1
        /// </example>
        [HttpGet]
        [Route("api/ChoreData/ListChoresForRoom/{id}")]
        [ResponseType(typeof(ChoreDto))]
        public IHttpActionResult ListChoresForRoom(int id)
        {

            List<Chore> Chores = db.Chores.Where(c => c.Rooms.Any(r=>r.RoomId == id)).ToList();
            List<ChoreDto> ChoreDtos = new List<ChoreDto>();

            Chores.ForEach(c => ChoreDtos.Add(new ChoreDto()
            {
                ChoreId = c.ChoreId,
                ChoreName = c.ChoreName,
                ChoreDescription = c.ChoreDescription,
                ChoreFrequency = c.ChoreFrequency,
                OwnerName = c.Owner.OwnerName
            }));

            return Ok(ChoreDtos);
        }
        /// <summary>
        /// Assign a particular chore to a particular room
        /// </summary>
        /// <param name="ChoreId">Chore id primary key</param>
        /// <param name="RoomId">Room id primary key</param>
        /// <returns>
        /// Status 200(OK) or 404(Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ChoreData/AssignChoreToRoom/20/3
        /// </example>
        [HttpPost]
        [Route("api/ChoreData/AssignChoreToRoom/{ChoreId}/{RoomId}")]
        public IHttpActionResult AssignChoreToRoom(int ChoreId, int RoomId)
        {
            Chore SelectedChore = db.Chores.Include(c=>c.Rooms).Where(c=>c.ChoreId== ChoreId).FirstOrDefault();
            Room SelectedRoom = db.Rooms.Find(RoomId);
            
            if(SelectedRoom == null || SelectedChore == null)
            {
                return NotFound();
            }
            
            SelectedChore.Rooms.Add(SelectedRoom);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Unassign a particular chore from a particular room
        /// </summary>
        /// <param name="ChoreId">Chore id primary key</param>
        /// <param name="RoomId">Room id primary key</param>
        /// <returns>
        /// Status 200(OK) or 404(Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ChoreData/UnassignChoreFromRoom/20/3
        /// </example>
        [HttpPost]
        [Route("api/ChoreData/UnassignChoreFromRoom/{ChoreId}/{RoomId}")]
        [ResponseType(typeof(ChoreDto))]
        public IHttpActionResult UnassignChoreFromRoom(int ChoreId, int RoomId)
        {
            Chore SelectedChore = db.Chores.Include(c => c.Rooms).Where(c => c.ChoreId == ChoreId).FirstOrDefault();
            Room SelectedRoom = db.Rooms.Find(RoomId);

            if (SelectedRoom == null || SelectedChore == null)
            {
                return NotFound();
            }

            SelectedChore.Rooms.Remove(SelectedRoom);
            db.SaveChanges();

            return Ok();
        }


    }
}
