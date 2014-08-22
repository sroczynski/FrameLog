using System;

namespace FrameLog.History.Binders
{
    public class PrimitiveBinder : IBinder
    {
        public bool Supports(Type type)
        {
            return type.IsPrimitive 
                || type == typeof(string)
                || type == typeof(decimal);
        }

        public object Bind(string raw, Type type)
        {
            return Convert.ChangeType(raw, type);
        }
    }
}
