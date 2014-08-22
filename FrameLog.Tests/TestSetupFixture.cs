using System.Data.Entity;
using FrameLog.Example;
using NUnit.Framework;

namespace FrameLog.Tests
{
    [SetUpFixture]
    public class TestSetupFixture
    {
        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer<ExampleContext>(new DropCreateDatabaseIfModelChanges<ExampleContext>());
        }
    }
}
