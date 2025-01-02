namespace FormsProyect.Models.Entities
{
    public class Topics
    {
        public int TopicID { get; set; }
        public string _TopicName { get; set; }
        public ICollection<Forms> Forms { get; set; }
    }
}
