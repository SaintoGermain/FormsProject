namespace FormsProyect.Models.Entities
{
    public class Likes
    { 
        public int LikeID { get; set; }
        public int UserId { get; set; }
        public int NoForm { get; set; }

        public Forms Forms { get; set; }
        public Users Users { get; set; }

    }
}
