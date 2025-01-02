namespace FormsProyect.Models.Entities
{
    public class Comments
    {
        public int CommentID { get; set; }
        public int UserId { get; set; }
        public int NoForm { get; set; }

        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public Forms Forms { get; set; }
        public Users Users { get; set; }
    }
}
