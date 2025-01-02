using System.ComponentModel.DataAnnotations.Schema;

namespace FormsProyect.Models.Entities
{
    public class Forms
    {
        public int NoForm { get; set; }
        public int UserId { get; set; }
        public int TopicID { get; set; }

        public string Title { get; set; }
        public string Descr { get; set; }
        public string? ImagePath { get; set; }
        public bool IsPublic { get; set; }

        public Topics Topics { get; set; }
        public Users Users { get; set; }
        public ICollection<Questions> Questions { get; set; } 
        public ICollection<FormTags> FormTags { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Likes> Likes { get; set; }
        public ICollection<AllowedUsers> AllowedUsers { get; set; }
        public ICollection<FormFilled> FormFilled { get; set; }
    }
}
