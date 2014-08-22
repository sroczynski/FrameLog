using System.Collections.Generic;
using FrameLog.Filter;

namespace FrameLog.Example.Models.Testing
{
    [DoNotLog]
    public class ClassWithDoNotLog
    {
        public int Id { get; set; }
        public string Property { get; set; } 
    }
}
