
namespace FrameLog.Example.Models
{
    public class ModelWithDynamicProxy
    {
        public int Id { get; set; }

        public int Field { get; set; }

        /// <summary>
        /// Because we have a virtual navigation property this model
        /// will use dynamic proxies
        /// http://msdn.microsoft.com/en-us/data/jj592886.aspx
        /// http://stackoverflow.com/questions/7111109/should-i-enable-or-disable-dynamic-proxies-with-entity-framework-4-1-and-mvc3
        /// </summary>
        public virtual Book Book { get; set; }
    }
}
