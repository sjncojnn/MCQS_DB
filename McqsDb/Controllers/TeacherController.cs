using Mcq.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace Mcq.Controllers
{
    public class TeacherController : Controller
    {
        private readonly McqsDbContext _context;

        public TeacherController(McqsDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            Console.WriteLine("im not here");
            // Chuyển đến dashboard của Teacher
            return View();
        }

        // GET: Create Exam Form
        public IActionResult CreateExam()
        {
            var topics = _context.Topics.Include(t => t.Property)
            .OrderBy(t => t.Property.Grade)
            .ToList(); 
            ViewBag.Topics = topics;
    
    return View("CreateExam");
        }

        // POST: Create Exam
        [HttpPost]
        public IActionResult CreateExam(CreateExamViewModel model)
        {

            var exam = new Exam
            {
                Idexam = Guid.NewGuid(),
                Name = model.ExamName,
                Duration = model.Duration,
                number_of_question = model.number_of_question,
                Idtopic = model.IdTopic,
                Idteacher = Guid.Parse(HttpContext.Session.GetString("UserId")),
                created_time = DateTime.Now
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            // Truyền Idexam đến View CreateQuestion
            return RedirectToAction("CreateQuestion", new { id = exam.Idexam });
        }

        public IActionResult CreateQuestion(Guid id)
        {
            var model = new CreateQuestionViewModel
            {
                ExamId = id
            };

            return View(model);
        }
 
        [HttpPost]
        public IActionResult CreateQuestion(CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("ModelState Error: " + error.ErrorMessage);
            }
                return BadRequest("Invalid data.");
            }

            // Lấy ExamId từ model
            var examId = model.ExamId;

            // Danh sách thông báo lỗi
            var errors = new List<string>();
            // Tạo danh sách câu hỏi
            var questions = model.Questions.Select(q =>
            {
                var question = new Question
                {
                    Idquestion = Guid.NewGuid(),
                    Content = q.Content,
                    difficulty_level = q.DifficultyLevel,
                    Idexam = examId,
                    QuestionOptions = new List<QuestionOption>()
                };
                var existingOptions = new HashSet<string>(); // Dùng để kiểm tra trùng lặp option

                foreach (var option in q.Options)
                {
                    if (existingOptions.Contains(option.OptionContent))
                    {
                        // Thêm lỗi nếu phát hiện option_content trùng
                        errors.Add($"Option '{option.OptionContent}' bị trùng trong câu hỏi '{q.Content}'.");
                    }
                    else
                    {
                        existingOptions.Add(option.OptionContent);

                        question.QuestionOptions.Add(new QuestionOption
                        {
                            Idquestion = question.Idquestion,
                            option_content = option.OptionContent,
                            IsCorrect = option.IsCorrect,
                            Explanation = option.Explanation
                        });
                    }
                }
                return question;
            }).ToList();
            if (errors.Any())
            {
                // Nếu có lỗi trùng lặp, trả về giao diện với lỗi
                ViewBag.Errors = errors;          
                return View("CreateQuestion", model);
            }

            try
            {
                // Lưu danh sách câu hỏi vào database
                _context.Questions.AddRange(questions);
                _context.SaveChanges();

                // Cập nhật số lượng câu hỏi trong Exam
                var exam = _context.Exams.FirstOrDefault(e => e.Idexam == examId);
                if (exam != null)
                {
                    exam.number_of_question += questions.Count;
                    _context.Exams.Update(exam);
                    _context.SaveChanges();
                }

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về phản hồi thích hợp
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while creating questions.");
            }
        }


        public IActionResult ExamList()
        {
            var teacherId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            var exams = _context.Exams
                .Where(e => e.Idteacher == teacherId)
                .Include(e => e.Questions)
                .Include(b => b.Topic)
                    .ThenInclude(c => c.Property)
                .OrderBy(t => t.Topic.Property.Grade)
                .ToList();

            return View(exams);
        }

        public IActionResult ExamDetails(Guid id)
        {
            var exam = _context.Exams
                .Include(e => e.Questions)
                .ThenInclude(q => q.QuestionOptions)
                .FirstOrDefault(e => e.Idexam == id);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        [HttpPost]
        public IActionResult EditExam(Guid Idexam, string Name, int Duration)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Idexam == Idexam);
            if (exam == null) return NotFound("Exam not found.");

            exam.Name = Name;
            exam.Duration = Duration;

            _context.SaveChanges();
            return RedirectToAction("ExamList");
        }



        [HttpPost]
        public IActionResult DeleteExam(Guid id)
        {
            var exam = _context.Exams
                .Include(e => e.Questions)
                .ThenInclude(q => q.QuestionOptions)
                .FirstOrDefault(e => e.Idexam == id);

            if (exam != null)
            {
                _context.Exams.Remove(exam);
                _context.SaveChanges();
            }

            return RedirectToAction("ExamList");
        }

        [HttpPost]
        public IActionResult DeleteQuestion(Guid id)
        {
            // Lấy thông tin câu hỏi bao gồm cả kỳ thi
            var question = _context.Questions
                .Include(q => q.QuestionOptions)
                .Include(q => q.Exam) // Bao gồm cả kỳ thi để dễ truy cập ExamId
                .FirstOrDefault(q => q.Idquestion == id);

            if (question != null)
            {
                // Giảm số lượng câu hỏi trong kỳ thi
                var exam = question.Exam;
                if (exam != null)
                {
                    exam.number_of_question--;
                }

                // Xóa câu hỏi
                _context.Questions.Remove(question);
                _context.SaveChanges();
            }

            // Redirect về trang chi tiết kỳ thi
            return RedirectToAction("ExamDetails", new { id = question.Idexam });
        }


        public IActionResult UpdateQuestion(Guid id)
        {
            var question = _context.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefault(q => q.Idquestion == id);

            if (question == null)
            {
                return NotFound();
            }

            var model = new UpdateQuestionViewModel
            {
                Idquestion = question.Idquestion,
                Idexam = question.Idexam,
                Content = question.Content,
                DifficultyLevel = question.difficulty_level,
                Options = question.QuestionOptions.Select(o => new QuestionOptionViewModel
                {
                    OptionContent = o.option_content,
                    IsCorrect = o.IsCorrect,
                    Explanation = o.Explanation
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateQuestion(UpdateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var question = _context.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefault(q => q.Idquestion == model.Idquestion);

            if (question == null)
            {
                return NotFound();
            }

            question.Content = model.Content;
            question.difficulty_level = model.DifficultyLevel;

            // Update existing options or add new ones
            foreach (var optionModel in model.Options)
            {
                var option = question.QuestionOptions
                    .FirstOrDefault(o => o.option_content == optionModel.OptionContent);

                if (option != null)
                {
                    option.option_content = optionModel.OptionContent;
                    option.IsCorrect = optionModel.IsCorrect;
                    option.Explanation = optionModel.Explanation;
                }
                else
                {
                    question.QuestionOptions.Add(new QuestionOption
                    {
                        Idquestion = question.Idquestion,
                        option_content = optionModel.OptionContent,
                        IsCorrect = optionModel.IsCorrect,
                        Explanation = optionModel.Explanation
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("ExamDetails", new { id = model.Idexam });
        }



        public IActionResult CreateTopic()
        {
            // Lấy danh sách Properties để gắn vào dropdown
            var properties = _context.Properties.ToList();
            ViewBag.Properties = properties;
            return View("CreateTopic");
        }

        // POST: Create Topic
        [HttpPost]
        public IActionResult CreateTopic(CreateTopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var topic = new Topic
            {
                Idtopic = Guid.NewGuid(),
                Name = model.Name,
                Idproperty = model.IdProperty,
                Idteacher = Guid.Parse(HttpContext.Session.GetString("UserId")),
                created_date = DateTime.Now
            };

            _context.Topics.Add(topic);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        public IActionResult Analytics()
        {
            // Lấy danh sách các bài kiểm tra của giáo viên
            Guid teacherId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            var exams = _context.Exams
                .Where(e => e.Idteacher == teacherId)
                .Include(e => e.Questions)
                .ToList();

            // Tạo model để chứa các thống kê
            var model = new TeacherAnalyticsViewModel();
            model.Exams = new List<ExamAnalytics>();

            foreach (var exam in exams)
            {
                var examAttemptStats = _context.ExamAttempts
                    .Where(ea => ea.Idexam == exam.Idexam)
                    .ToList();

                var totalStudents = examAttemptStats.Count();
                var totalScore = examAttemptStats.Sum(ea => ea.ExamScore);
                var avgScore = totalStudents > 0 ? totalScore / totalStudents : 0;

                var correctAnswers = _context.StudentAnswers
                    .Where(sa => sa.Question.Idexam == exam.Idexam && sa.IsCorrect)
                    .Count();

                var incorrectAnswers = _context.StudentAnswers
                    .Where(sa => sa.Question.Idexam == exam.Idexam && !sa.IsCorrect)
                    .Count();

                var questionDifficultyStats = new Dictionary<string, int>();
                foreach (var question in exam.Questions)
                {
                    var correctForQuestion = _context.StudentAnswers
                        .Where(sa => sa.Idquestion == question.Idquestion && sa.IsCorrect)
                        .Count();

                    var incorrectForQuestion = _context.StudentAnswers
                        .Where(sa => sa.Idquestion == question.Idquestion && !sa.IsCorrect)
                        .Count();

                    questionDifficultyStats.Add(question.difficulty_level, correctForQuestion);
                }

                // Thêm thông tin thống kê cho bài kiểm tra vào model
                model.Exams.Add(new ExamAnalytics
                {
                    ExamName = exam.Name,
                    TotalStudents = totalStudents,
                    AverageScore = avgScore,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = incorrectAnswers,
                    QuestionDifficultyStats = questionDifficultyStats
                });
            }

            return View(model);
        }
    }
}
