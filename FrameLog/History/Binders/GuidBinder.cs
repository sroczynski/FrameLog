using System;

namespace FrameLog.History.Binders
{
    public class GuidBinder : IBinder
    {
        public bool Supports(Type type)
        {
            return type == typeof(Guid);
        }

        public object Bind(string raw, Type type)
        {
            return new Guid(raw);
        }
    }
}
