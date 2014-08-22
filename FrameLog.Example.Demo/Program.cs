using System;
using System.Data.Entity;
using FrameLog.Example.Models;

namespace FrameLog.Example.Demo
{
    class Program
    {
        private const string publisherName = "Acme Publishing";

        static void Main(string[] args)
        {
            Database.SetInitializer<ExampleContext>(new DropCreateDatabaseIfModelChanges<ExampleContext>());
            using (var db = new ExampleContext())
            {
                Console.WriteLine("CodeFirst is creating the database");
                db.Database.Delete();
                db.Database.Create();

                var user = new User() { Name = "Nicolas" };

                bookDemo(db, user);
                publisherDemo(db, user);


                Console.ReadKey();
            }
        }

        private static Book bookDemo(ExampleContext db, User user)
        {
            Console.WriteLine("\nDemo for tracking a simple property");
            var book = makeBook("Dracula", db, user);

            Console.WriteLine("Changing title of book to 'Dune'");
            book.Title = "Dune";
            db.Save(user);

            displayTitleChanges(db, book);
            return book;
        }

        private static void publisherDemo(ExampleContext db, User user)
        {
            Console.WriteLine("\nDemo for tracking a collection property");

            var book1 = makeBook("Dracula", db, user);
            var book2 = makeBook("Dune", db, user);

            Console.WriteLine("Creating publisher '{0}' and saving to database", publisherName);
            var publisher = db.Publishers.Add(new Publisher() { Name = publisherName });
            db.Save(user);

            addToPublisher(db, user, book1, publisher);
            addToPublisher(db, user, book2, publisher);

            var changes = db.HistoryExplorer.ChangesTo(publisher, b => b.Books);
            Console.WriteLine("History of Publisher.Books:");
            foreach (var change in changes)
            {
                Console.WriteLine(string.Format("{0}:{1}:{2}",
                    change.Author,
                    change.Timestamp,
                    string.Join(", ", change.Value)));
            }
        }

        private static void addToPublisher(ExampleContext db, User user, Book book, Publisher publisher)
        {
            Console.WriteLine("Adding {0} to publisher", book.Title);
            publisher.Books.Add(book);
            db.Save(user);
        }

        private static void displayTitleChanges(ExampleContext db, Book book)
        {
            var changes = db.HistoryExplorer.ChangesTo(book, b => b.Title);
            Console.WriteLine("History of book title:");
            foreach (var change in changes)
                Console.WriteLine(change);
        }

        private static Book makeBook(string title, ExampleContext db, User user)
        {
            Console.WriteLine("Creating book '{0}' and saving to database", title);
            var book = new Book() { Title = title };
            db.Books.Add(book);
            db.Save(user);
            return book;
        }
    }
}
