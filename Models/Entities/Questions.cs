using System.ComponentModel.DataAnnotations.Schema;

namespace FormsProyect.Models.Entities
{
    public class Questions
    {
        public int IDQuest { get; set; }
        public int NoForm { get; set; }


        public int _Type { get; set; }
        public string TitleQ { get; set; }
        public string DescrQ { get; set; }
        public bool _Show { get; set; }

        public Forms Forms { get; set; }
        public ICollection<AnsweredQuestions> AnsweredQuestions { get; set; }
    }
}
