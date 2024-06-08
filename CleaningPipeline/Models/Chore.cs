using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CleaningPipeline.Models
{
    public class Chore
    {
        [Key]
        public int ChoreId { get; set; }
        public string ChoreName { get; set; }
        public string ChoreDescription { get; set; }
        public string ChoreFrequency { get; set; }

        //One chore/task can have only one owner/person, responsable for the task
        //One owner can have several chores/tasks to do
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }

        //One chore can have many rooms where they must be completed
        public ICollection<Room> Rooms { get; set; }
    }

    //Data transfer object for Chores
    public class ChoreDto
    {
        public int ChoreId { get; set; }
        public string ChoreName { get; set; }
        public string ChoreDescription { get; set; }
        public string ChoreFrequency { get; set; }

        public int OwnerId { get; set; }

        public string OwnerName { get; set; }
    }
}
    