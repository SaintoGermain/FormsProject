namespace FormsProyect.Models.Entities
{
    public class AllowedUsers
    {
        public int NoForm { get; set; }
        public int UserId { get; set; }
        public Forms Forms { get; set; }
        public Users Users { get; set; }
    }
}
