using FrameLog.Filter;
using System.ComponentModel.DataAnnotations;

namespace FrameLog.Example.Models.Testing
{
    [DoLog]
    public class ClassWithSomeIncludedProperties
    {
        [Key, DoLog]
        public int Id { get; set; }

        [DoLog]
        public virtual User IncludedNavigationProperty { get; set; }
        [DoLog]
        public string IncludedScalarProperty { get; set; }

        public virtual User ExcludedNavigationProperty { get; set; }

        public string ExcludedScalarProperty { get; set; }
    }
}
