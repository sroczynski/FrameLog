using FrameLog.Contexts;
using FrameLog.Example.Models;
using FrameLog.Example.Models.Testing;
using FrameLog.Filter;
using FrameLog.History;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace FrameLog.Example
{
    public class ExampleContext : DbContext
    {
        public ExampleContext(ILoggingFilterProvider filterProvider = null)
        {
            Logger = new FrameLogModule<ChangeSet, User>(new ChangeSetFactory(), FrameLogContext, filterProvider);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ModelWithDynamicProxy> ModelsWithDynamicProxies { get; set; }

        #region unit test support
        public DbSet<ClassWithSomeExcludedProperties> ClassesWithSomeExcludedProperties { get; set; }
        public DbSet<ClassWithDoNotLog> ClassesWithDoNotLog { get; set; }
        public DbSet<ClassWithEntityCollection> ClassesWithEntityCollection { get; set; }
        public DbSet<VanillaTestClass> VanillaTestClasses { get; set; }
        public DbSet<ClassWithSomeIncludedProperties> ClassesWithSomeIncludedProperties { get; set; }
        #endregion

        #region logging
        public DbSet<ChangeSet> ChangeSets { get; set; }
        public DbSet<ObjectChange> ObjectChanges { get; set; }
        public DbSet<PropertyChange> PropertyChanges { get; set; }

        public readonly FrameLogModule<ChangeSet, User> Logger;
        public IFrameLogContext<ChangeSet, User> FrameLogContext
        {
            get { return new ExampleContextAdapter(this); }
        }
        public HistoryExplorer<ChangeSet, User> HistoryExplorer
        {
            get { return new HistoryExplorer<ChangeSet, User>(FrameLogContext); }
        }

        public void Save(User author)
        {
            Logger.SaveChanges(author);
        }
        #endregion
    }
}
