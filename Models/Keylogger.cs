using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KeyLoggerApi.Models
{
    public class Keylogger
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName ="nvarchar(max)")]
        [Required]
        public string Keystroke { get; set; }
        public DateTime CreationDate { get; set; }

        public Keylogger()
        {
            this.CreationDate = DateTime.UtcNow.ToLocalTime();
        }
    }
}
