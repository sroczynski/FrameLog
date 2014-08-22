using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrameLog.Example.Models
{
    public class Publisher : IHasLoggingReference
    {
        public Publisher()
        {
            Books = new List<Book>();
        }

        [Key]
        public string Name { get; set; }
        public virtual List<Book> Books { get; set; }

        public object Reference
        {
            get { return Name; }
        }
    }
}
