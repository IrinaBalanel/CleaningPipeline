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
    public class OwnerDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>
        ///Lists owners in the db and returns 200(OK)
        ///</summary>
        ///<returns>An array of owners objects</returns>
        ///<example>
        ///GET: api/OwnerData/ListOwners -> 
        ///[{"OwnerId":1, "OwnerName":"Irina", "OwnerAvailability":"Friday"},
        ///{"OwnerId":2, "OwnerName":"Andrew", "OwnerAvailability":"Monday"}]
        ///</example>

        [ResponseType(typeof(OwnerDto))]
        [HttpGet]
        [Route("api/OwnerData/ListOwners")]
        public IHttpActionResult ListOwners()
        {
            List<Owner> Owners = db.Owners.ToList();
            List<OwnerDto> OwnerDtos = new List<OwnerDto>();

            Owners.ForEach(o => OwnerDtos.Add(new OwnerDto()
            {
                OwnerId = o.OwnerId,
                OwnerName = o.OwnerName,
                OwnerAvailability = o.OwnerAvailability
            }));

            return Ok(OwnerDtos);
        }



        ///<summary>
        /// Finds an owners in db by owner id and output information associated with this specific owner + 200(OK)
        ///</summary>
        ///<returns>An object of an owner with given id</returns>
        ///<param name="id">The primary key of the owner</param>
        ///<example>
        ///GET: api/OwnerData/FindOwner/1 -> 
        ///{"OwnerId":1, "OwnerName":"Irina", "OwnerAvailability":"Friday"}
        ///</example>

        [ResponseType(typeof(Owner))]
        [HttpGet]
        [Route("api/OwnerData/FindOwner/{id}")]
        public IHttpActionResult FindOwner(int id)
        {
            Owner Owner = db.Owners.Find(id);
            OwnerDto OwnerDto = new OwnerDto()
            {
                OwnerId = Owner.OwnerId,
                OwnerName = Owner.OwnerName,
                OwnerAvailability = Owner.OwnerAvailability
            };

            if (Owner == null)
            {
                return NotFound();
            }

            return Ok(OwnerDto);
        }

        ///<summary>
        /// Adds an owner to the db
        ///</summary>
        ///<returns>owner id and owner data with status 201(created) or 400(Bad request)</returns>
        ///<param name="owner">JSON form data of an owner</param>
        ///<example>
        ///POST: api/OwnerData/AddOwner
        /// FORM DATA: Owner JSON data: {"OwnerId":2, "OwnerName":"Andrew", "OwnerAvailability":"Sunday"}
        ///</example>

        [ResponseType(typeof(Owner))]
        [HttpPost]
        [Route("api/OwnerData/AddOwner")]

        public IHttpActionResult AddOwner(Owner owner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Owners.Add(owner);
            db.SaveChanges();

            return Ok();

            /*return CreatedAtRoute("DefaultApi", new { id = owner.OwnerId }, owner);*/
        }

        ///<summary>
        /// Deletes an owner from the db by it's ID
        ///</summary>
        ///<returns>status 200(Ok) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the owner</param>
        ///<example>
        ///POST: api/OwnerData/DeleteOwner/2
        ///</example>

        [ResponseType(typeof(Owner))]
        [HttpPost]
        [Route("api/OwnerData/DeleteOwner/{id}")]

        public IHttpActionResult DeleteOwner(int id)
        {
            Owner owner = db.Owners.Find(id);
            if (owner == null)
            {
                return NotFound();
            }

            db.Owners.Remove(owner);
            db.SaveChanges();

            return Ok();
        }


        ///<summary>
        /// Updates a particular owner by it's ID in the db with JSON post data
        ///</summary>
        ///<returns>status 204(Success) or 400(Bad request) or 404(Not Found)</returns>
        ///<param name="id">The primary key of the owner</param>
        ///<param name="chore">JSON post data of an owner to update</param>
        ///<example>
        ///POST: api/OwnerData/UpdateOwner/1
        ///</example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/OwnerData/UpdateOwner/{id}")]
        public IHttpActionResult UpdateOwner(int id, Owner owner)
        {
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != owner.OwnerId)
            {

                return BadRequest();
            }

            db.Entry(owner).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(id))
                {
                    Debug.WriteLine("Owner not found");
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

        private bool OwnerExists(int id)
        {
            return db.Owners.Count(e => e.OwnerId == id) > 0;
        }

    }
}
