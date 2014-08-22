using FrameLog.Models;

namespace FrameLog.Example.Models
{
    public class PropertyChange : IPropertyChange<User>
    {
        public int Id { get; set; }
        public ObjectChange ObjectChange { get; set; }
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public int? ValueAsInt { get; set; }

        IObjectChange<User> IPropertyChange<User>.ObjectChange 
        { 
            get { return ObjectChange; }
            set { ObjectChange = (ObjectChange)value; }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", PropertyName, Value);
        }
    }
}
