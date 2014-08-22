using System.Data.Entity.Core.Objects.DataClasses;

namespace FrameLog.Example.Models.Testing
{
    public class ClassWithEntityCollection
    {
        public ClassWithEntityCollection()
        {
            Users = new EntityCollection<User>();
        }

        public virtual int Id { get; set; }
        public virtual EntityCollection<User> Users { get; set; }
    }
}
