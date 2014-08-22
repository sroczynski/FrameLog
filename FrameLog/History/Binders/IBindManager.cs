using System;

namespace FrameLog.History.Binders
{
    public interface IBindManager
    {
        ItemType Bind<ItemType>(string reference);
        object Bind(string raw, Type type);
    }
}
