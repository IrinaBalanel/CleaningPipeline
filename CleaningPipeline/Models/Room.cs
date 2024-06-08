using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleaningPipeline.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }

        //One room can have many chores assigned to this specific room
        public ICollection<Chore> Chores { get; set; }
    }


    
    //Data transfer object for Rooms
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }

    }
}