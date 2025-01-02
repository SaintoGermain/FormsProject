namespace FormsProyect.Models.Entities
{
    public class FormFilled
    {
        public int NoFilledForm { get; set; }
        public int UserId { get; set; }
        public int NoForm { get; set; }

        public Users Users { get; set; }
        public Forms Forms { get; set; }
        public ICollection<AnsweredQuestions> AnsweredQuestions { get; set; }
    }
}
