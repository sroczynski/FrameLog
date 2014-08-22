using System;

namespace FrameLog.History.Binders
{
    public class DateTimeBinder : IBinder
    {
        public bool Supports(Type type)
        {
            return typeof(DateTime?).IsAssignableFrom(type);
        }

        public object Bind(string raw, Type type)
        {
            try
            {
                return DateTime.Parse(raw);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
