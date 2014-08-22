using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameLog.Models;

namespace FrameLog.Example.Models
{
    public class ObjectChange : IObjectChange<User>
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string ObjectReference { get; set; }
        public ChangeSet ChangeSet { get; set; }
        public virtual List<PropertyChange> PropertyChanges { get; set; }

        IEnumerable<IPropertyChange<User>> IObjectChange<User>.PropertyChanges
        {
            get { return PropertyChanges; }
        }
        void IObjectChange<User>.Add(IPropertyChange<User> propertyChange)
        {
            PropertyChanges.Add((PropertyChange)propertyChange);
        }
        IChangeSet<User> IObjectChange<User>.ChangeSet
        {
            get { return ChangeSet; }
            set { ChangeSet = (ChangeSet)value; }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", TypeName, ObjectReference);
        }
    }
}
