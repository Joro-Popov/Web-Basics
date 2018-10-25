using System;
using System.ComponentModel.DataAnnotations;

namespace TORSHIA.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        public ReportStatus Status { get; set; }

        public DateTime ReportedOn { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; }

        public int ReportedId { get; set; }
        public virtual User Reporter { get; set; }
    }
}
