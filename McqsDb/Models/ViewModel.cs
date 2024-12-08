namespace Mcq.Models
{
    public class CreateExamViewModel
    {
        public string ExamName { get; set; }
        public int Duration { get; set; }
        public int number_of_question { get; set; }
        public Guid IdTopic { get; set; }
    }

    public class CreateQuestionViewModel
    {
        public Guid ExamId { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    public class QuestionViewModel
    {
        public string Content { get; set; }
        public string DifficultyLevel { get; set; }
        public List<QuestionOptionViewModel> Options { get; set; } = new List<QuestionOptionViewModel>();
    }

    public class QuestionOptionViewModel
    {
        public string OptionContent { get; set; }
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; }
    }

    public class CreateTopicViewModel
    {
        public string Name { get; set; }
        public Guid IdProperty { get; set; } // Thuộc tính property cần chọn từ danh sách có sẵn
    }

    public class UpdateQuestionViewModel
    {
        public Guid Idquestion { get; set; }
        public Guid Idexam { get; set; }
        public string Content { get; set; }
        public string DifficultyLevel { get; set; }
        public List<QuestionOptionViewModel> Options { get; set; }
    }

    public class ExamDetailViewModel
    {
        public Guid Idexam { get; set; }
        public string ExamName { get; set; }
        public int Duration { get; set; }
        public int NumberOfQuestions { get; set; }
        public string CreatedBy { get; set; }
        public List<AttemptHistory> AttemptHistories { get; set; }
        
    }

    public class AttemptHistory
    {
        public DateTime StartTime { get; set; }
        public DateTime CompletionTime { get; set; }
        public decimal ExamScore { get; set; }
        public int NumsCorrectAns { get; set; }
        public List<StudentAnswerViewModel> StudentAnswers { get; set; }
    }

    public class StudentAnswerViewModel
    {
        public Guid Idquestion { get; set; }
        public string QuestionName {get; set;}
        public string SelectedAns { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ExamSessionViewModel
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; }
    public int Duration { get; set; } // Thời gian làm bài (phút)
    public List<StudentQuestionViewModel> Questions { get; set; }
}

public class StudentQuestionViewModel
{
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public List<OptionViewModel> Options { get; set; }
    public bool Flagged { get; set; }
}

public class OptionViewModel
{
    public string Content { get; set; }
}

public class ExamResultViewModel
    {
        public string ExamName { get; set; }
        public double ExamScore { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompletionTime { get; set; }
        public double Duration { get; set; } // Thời gian làm bài (phút)
    }

public class PerformanceViewModel
{
    public List<ExamPerformance> PerformanceData { get; set; }
    public int TotalExams { get; set; }
    public double AverageScore { get; set; }
    public double AccuracyRate { get; set; }
}

public class ExamPerformance
{
    public string ExamName { get; set; }
    public decimal Score { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double AccuracyRate { get; set; }
}

public class TeacherAnalyticsViewModel
{
    public List<ExamAnalytics> Exams { get; set; }
}

public class ExamAnalytics
{
    public string ExamName { get; set; }
    public int TotalStudents { get; set; }
    public decimal AverageScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public Dictionary<string, int> QuestionDifficultyStats { get; set; }
}

}
