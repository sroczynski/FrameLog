using System;
using System.Collections.Generic;
using FrameLog.Models;

namespace FrameLog.Example.Models
{
    public class ChangeSet : IChangeSet<User>
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public User Author { get; set; }
        public virtual List<ObjectChange> ObjectChanges { get; set; }

        IEnumerable<IObjectChange<User>> IChangeSet<User>.ObjectChanges
        {
            get { return ObjectChanges; }
        }

        void IChangeSet<User>.Add(IObjectChange<User> objectChange)
        {
            ObjectChanges.Add((ObjectChange)objectChange);
        }

        public override string ToString()
        {
            return string.Format("By {0} on {1}, with {2} ObjectChanges",
                Author, Timestamp, ObjectChanges.Count);
        }
    }
}
