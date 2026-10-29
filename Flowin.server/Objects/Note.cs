using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Flowin.server.Objects
{
    public class Note
    {
        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Note() { }
        public Note(Guid id, string title, string content)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public Note(string title, string content, Guid id, DateTime crAt, DateTime updAt)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedAt = crAt;
            UpdatedAt = updAt;
        }

        public override string ToString()
            => $"ID -> {this.Id}; title -> {this.Title}";

        public static bool operator ==(Note? left, Note? right)
        {
            if (left is null || right is null)
                return left is null && right is null;

            return left.Id == right.Id && left.Content == right.Content && left.Title == right.Title;
        }

        public static bool operator !=(Note? left, Note? right)
            => !(left == right);

        public bool IsValidNote()
        {
            if (!(this.Id is Guid) || this.Id == null || this.Id == Guid.Empty)
                return false;
            if (this.Content == null || this.Title == null)
                return false;
            return true;
        }
    }
}
