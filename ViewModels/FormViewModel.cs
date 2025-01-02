using FormsProyect.Models.Entities;

namespace FormsProyect.ViewModels
{
    public class FormViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int SelectedTopicId { get; set; }
        public string Tags { get; set; }
        public int numberOfSingleLineQuestions { get; set; }
        public int numberOfMultipleLinesQuestions { get; set; }
        public int numberOfPositiveIntegersQuestions { get; set; }
        public int numberOfCheckboxQuestions { get; set; }
        public List<Topics> Topics { get; set; } = new List<Topics>();
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    public class QuestionViewModel
    {
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class TagModel
    {
        public string Value { get; set; }
    }

    public class QuestionsViewModel
    {
        public int NumberOfSingleLineQuestions { get; set; }
        public int NumberOfMultipleLinesQuestions { get; set; }
        public int NumberOfPositiveIntegersQuestions { get; set; }
        public int NumberOfCheckboxQuestions { get; set; }
    }
}
