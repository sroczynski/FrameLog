using System;
using System.Collections.Generic;
using FrameLog.Filter;

namespace FrameLog.Example.Models
{
    public class Book : ICloneable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int NumberOfFans { get; set; }
        public Book Sequel { get; set; }

        // These methods allow the Book to be use in HistoryExplorer.ChangesTo(TModel model)
        public object Clone()
        {
            return Copy();
        }
        public Book Copy()
        {
            return new Book()
            {
                Title = this.Title,
                NumberOfFans = this.NumberOfFans,
                Sequel = this.Sequel,
            };
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
