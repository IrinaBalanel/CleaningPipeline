using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleaningPipeline.Models.ViewModels
{
    public class DetailsRoom
    {
        public RoomDto SelectedRoom { get; set; }
        public IEnumerable<ChoreDto> RelatedChores { get; set; }
    }
}