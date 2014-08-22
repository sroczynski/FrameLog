using System;

namespace FrameLog.History
{
    public interface IBinder
    {
        bool Supports(Type type);
        object Bind(string raw, Type type);
    }
}
