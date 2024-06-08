using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleaningPipeline.Models.ViewModels
{
    public class DetailsChore
    {
        public ChoreDto SelectedChore { get; set; }
        public IEnumerable<RoomDto> AssignedRooms { get; set; }

        public IEnumerable<RoomDto> NotAssignedRooms { get; set; }
    }
}