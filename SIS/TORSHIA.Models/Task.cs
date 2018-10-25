using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TORSHIA.Models
{
    using System;

    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsReported { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public int Level { get; set; }

        public virtual ICollection<UserTask> Participants { get; set; } = new HashSet<UserTask>();

        public virtual ICollection<TaskSector> AffectedSectors { get; set; } = new HashSet<TaskSector>();

        public virtual ICollection<Report> Reports { get; set; } = new HashSet<Report>();
    }
}
