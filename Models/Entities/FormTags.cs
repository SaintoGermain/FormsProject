namespace FormsProyect.Models.Entities
{
    public class FormTags
    {
        public int NoForm { get; set; }
        public int TagID { get; set; }
        public Forms Forms { get; set; }
        public Tags Tags { get; set; }
    }
}
