namespace FormsProyect.Models.Entities
{
    public class Tags
    {
        public int TagID { get; set; }
        public string _TagName { get; set; }
        public ICollection<FormTags> FormTags { get; set; }
    }
}
