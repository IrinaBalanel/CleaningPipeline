using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleaningPipeline.Models
{
    public class Owner
    {
        [Key]
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAvailability { get; set; }
    }
    
    //Data transfer object for Owners
    public class OwnerDto
    {
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAvailability { get; set; }
    }
}