﻿using FormsProyect.Models.Entities;

namespace FormsProyect.ViewModels
{
    public class FormViewModel
    {
        public int NoForm { get; set; }
        public string? _Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int SelectedTopicId { get; set; }
        public string? Tags { get; set; }
        public bool IsPublic { get; set; }
        public int numberOfSingleLineQuestions { get; set; }
        public int numberOfMultipleLinesQuestions { get; set; }
        public int numberOfPositiveIntegersQuestions { get; set; }
        public int numberOfCheckboxQuestions { get; set; }
        public int FormID { get; set; }
        public string? Users { get; set; }
        public List<Topics> Topics { get; set; } = new List<Topics>();
        public List<string> TagsL { get; set; } = new List<string>();
        public List<string> TagsEdit { get; set; } = new List<string>();
        public List<int> QuestionsEdit { get; set; } = new List<int>();
        public List<string> UsersL { get; set; } = new List<string>();
        public List<AllowedUsersModel> AllowedUsers { get; set; }
        public List<QDetailsViewModel> Questions { get; set; } = new List<QDetailsViewModel>(); 
    }

    public class TagModel
    {
        public string? Value { get; set; }
        public int TagID { get; set; }
    }

    public class AllowedUsersModel
    {
        public int UserIdentifier { get; set; }
        public string Email { get; set; }
    }

    public class QDetailsViewModel
    {
        public int QuestionID { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionDescription { get; set; }
        public bool QuestionShow { get; set; }
        public int QuestionType { get; set; }
        public int NoForm { get; set; }
    }

    public class ProfileViewModel
    {
        public List<int> UsersIDS { get; set; }
        public List<Forms> Forms { get; set; }
    }
    public class TicketRequestModel
    {
        public string Summary { get; set; }
        public string Priority { get; set; }
        public string Link { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentTemplate { get; set; }
    }
}
