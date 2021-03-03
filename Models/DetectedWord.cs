using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KeyLoggerApi.Models
{
    public class DetectedWord
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName ="nvarchar(250)")]
        [Required]
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public DetectedWord()
        {
            this.CreationDate = DateTime.UtcNow.ToLocalTime();
        }
    }

    public class WordList
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        [Required]
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public WordList()
        {
            this.CreationDate = DateTime.UtcNow.ToLocalTime();
        }
    }

}
