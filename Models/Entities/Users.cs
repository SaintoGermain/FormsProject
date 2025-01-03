namespace FormsProyect.Models.Entities
{
    public class Users
    {
        public int UserId { get; set; }

        public string _Name { get; set; }
        public string Email { get; set; }  
        public string PasswordHash { get; set; }
        public bool Active { get; set; }
        public bool Admin { get; set;}

        public ICollection<Forms> Forms { get; set; }
        public ICollection<AllowedUsers> AllowedUsers { get; set; }
        public ICollection<FormFilled> FormFilled { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Likes> Likes { get; set; }

    }
}
