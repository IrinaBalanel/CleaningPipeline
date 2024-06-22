using CleaningPipeline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace CleaningPipeline.Controllers
{
    public class RoomDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ///<summary>
        ///Lists rooms in the db and returns 200(OK)
        ///</summary>
        ///<returns>An array of rooms objects</returns>
        ///<example>
        ///GET: api/RoomData/ListRooms -> 
        ///[{"RoomId":1, "RoomName":"Living-room"},
        ///{"RoomId":2, "RoomName":"Kitchen"}]
        ///</example>

        [ResponseType(typeof(RoomDto))]
        [HttpGet]
        [Route("api/RoomData/ListRooms")]
        public IHttpActionResult ListRooms()
        {
            List<Room> Rooms = db.Rooms.ToList();
            List<RoomDto> RoomDtos = new List<RoomDto>();

            Rooms.ForEach(r => RoomDtos.Add(new RoomDto()
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName
            }));

            return Ok(RoomDtos);
        }

        ///<summary>
        ///Lists rooms in the db for a particular chore and returns 200(OK)
        ///</summary>
        ///<returns>all rooms assigned to this particular chore</returns>
        ///<param name="id">chore primary key</param>
        ///<example>
        ///GET: api/RoomData/ListRoomsForChore/1
        ///</example>

        [ResponseType(typeof(RoomDto))]
        [HttpGet]
        [Route("api/RoomData/ListRoomsForChore/{id}")]
        public IHttpActionResult ListRoomsForChore(int id)
        {
            List<Room> Rooms = db.Rooms.Where(r=>r.Chores.Any(c=>c.ChoreId==id)).ToList();
            List<RoomDto> RoomDtos = new List<RoomDto>();

            Rooms.ForEach(r => RoomDtos.Add(new RoomDto()
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName
            }));

            return Ok(RoomDtos);
        }

        ///<summary>
        ///Lists rooms in the db which are not associated with this particular chore and returns 200(OK)
        ///</summary>
        ///<returns>all rooms not assigned to this particular chore</returns>
        ///<param name="id">chore primary key</param>
        ///<example>
        ///GET: api/RoomData/ListRoomsNotForChore/1
        ///</example>

        [ResponseType(typeof(RoomDto))]
        [HttpGet]
        [Route("api/RoomData/ListRoomsNotForChore/{id}")]
        public IHttpActionResult ListRoomsNotForChore(int id)
        {
            List<Room> Rooms = db.Rooms.Where(r => !r.Chores.Any(c => c.ChoreId == id)).ToList();
            List<RoomDto> RoomDtos = new List<RoomDto>();

            Rooms.ForEach(r => RoomDtos.Add(new RoomDto()
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName
            }));

            return Ok(RoomDtos);
        }
        ///<summary>
        /// Finds a room in db by room id and output information associated with this specific room + 200(OK)
        ///</summary>
        ///<returns>An object of a room with given id</returns>
        ///<param name="id">The primary key of the rooms table</param>
        ///<example>
        ///GET: api/RoomData/FindRoom/1 -> 
        ///{"RoomId":1, "RoomName":"Kitchen"}
        ///</example>

        [ResponseType(typeof(RoomDto))]
        [HttpGet]
        [Route("api/RoomData/FindRoom/{id}")]
        public IHttpActionResult FindRoom(int id)
        {
            Room Room = db.Rooms.Find(id);
            RoomDto RoomDto = new RoomDto()
            {
                RoomId = Room.RoomId,
                RoomName = Room.RoomName
            };

            if (Room == null)
            {
                return NotFound();
            }

            return Ok(RoomDto);
        }

        ///<summary>
        /// Adds a room to the db
        ///</summary>
        ///<returns>room id and room data with status 201(created) or 400(Bad request)</returns>
        ///<param name="room">JSON form data of a room</param>
        ///<example>
        ///POST: api/RoomData/AddRoom 
        /// FORM DATA: Room JSON data: {"RoomId":1, "roomName":"Living-room"}
        ///</example>

        [ResponseType(typeof(Room))]
        [HttpPost]
        [Route("api/RoomData/AddRoom")]
        [Authorize]

        public IHttpActionResult AddRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rooms.Add(room);
            db.SaveChanges();

            return Ok();

            /*return CreatedAtRoute("DefaultApi", new { id = room.RoomId }, room);*/
        }

        ///<summary>
        /// Deletes a room from the db by it's ID
        ///</summary>
        ///<returns>status 200(Ok) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the room</param>
        ///<example>
        ///POST: api/RoomData/DeleteRoom/2
        ///</example>

        [ResponseType(typeof(Room))]
        [HttpPost]
        [Route("api/RoomData/DeleteRoom/{id}")]
        [Authorize]

        public IHttpActionResult DeleteRoom(int id)
        {
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            db.Rooms.Remove(room);
            db.SaveChanges();

            return Ok();
        }


        ///<summary>
        /// Updates a particular room by it's ID in the db with JSON post data
        ///</summary>
        ///<returns>status 204(Success) or 400(Bad request) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the room tabke</param>
        ///<param name="room">JSON post data of a room to update</param>
        ///<example>
        ///POST: api/RoomData/UpdateRoom/1
        ///</example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/RoomData/UpdateRoom/{id}")]
        [Authorize]
        public IHttpActionResult UpdateRoom(int id, Room room)
        {
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != room.RoomId)
            {

                return BadRequest();
            }

            db.Entry(room).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    Debug.WriteLine("Room not found");
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

        private bool RoomExists(int id)
        {
            return db.Rooms.Count(e => e.RoomId == id) > 0;
        }
    }
}
