namespace FormsProyect.Models.Entities
{
    public class AnsweredQuestions
    {
        public int IDQuest { get; set; }
        public int NoFilledForm { get; set; }

        public string? QuestT1 { get; set; }
        public string? QuestT2 { get; set; }
        public int? QuestT3 { get; set; }
        public bool? QuestT4 { get; set; }

        public Questions Questions { get; set; }
        public FormFilled FormFilled { get; set; }
    }
}
