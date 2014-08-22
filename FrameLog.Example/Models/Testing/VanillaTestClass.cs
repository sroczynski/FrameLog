
namespace FrameLog.Example.Models.Testing
{
    public class VanillaTestClass
    {
        public virtual int Id { get; set; }
        public virtual int ScalarProperty { get; set; }
        public virtual User NavigationProperty { get; set; }
    }
}
