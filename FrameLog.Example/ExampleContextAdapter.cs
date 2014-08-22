using System;
using System.Linq;
using FrameLog.Contexts;
using FrameLog.Example.Models;
using FrameLog.Models;

namespace FrameLog.Example
{
    public class ExampleContextAdapter : DbContextAdapter<ChangeSet, User>
    {
        private ExampleContext context;

        public ExampleContextAdapter(ExampleContext context)
            : base(context)
        {
            this.context = context;
        }

        public override IQueryable<IChangeSet<User>> ChangeSets
        {
            get { return context.ChangeSets; }
        }
        public override IQueryable<IObjectChange<User>> ObjectChanges
        {
            get { return context.ObjectChanges; }
        }
        public override IQueryable<IPropertyChange<User>> PropertyChanges
        {
            get { return context.PropertyChanges; }
        }
        public override void AddChangeSet(ChangeSet changeSet)
        {
            context.ChangeSets.Add(changeSet);
        }

        public override Type UnderlyingContextType
        {
            get { return typeof(ExampleContext); }
        }
    }
}
