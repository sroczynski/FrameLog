using FrameLog.Models;
using System.Collections.Generic;

namespace FrameLog.Example.Models
{
    public class ChangeSetFactory : IChangeSetFactory<ChangeSet, User>
    {
        public ChangeSet ChangeSet()
        {
            var set = new ChangeSet();
            set.ObjectChanges = new List<ObjectChange>();
            return set;
        }

        public IObjectChange<User> ObjectChange()
        {
            var o = new ObjectChange();
            o.PropertyChanges = new List<PropertyChange>();
            return o;
        }

        public IPropertyChange<User> PropertyChange()
        {
            return new PropertyChange();
        }
    }
}
