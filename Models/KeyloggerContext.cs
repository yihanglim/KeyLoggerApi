using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KeyLoggerApi.Models
{
    public class KeyloggerContext:DbContext
    {
        public KeyloggerContext(DbContextOptions<KeyloggerContext> options): base(options)
        {

        }
        public DbSet<Keylogger> Keyloggers { get; set; }

        public DbSet<DetectedWord> DetectedWords { get; set; }

        public DbSet<WordList> WordLists { get; set; }
    }    
}
