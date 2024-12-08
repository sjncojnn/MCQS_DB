using Mcq.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
// using Newtonsoft.Json;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace Mcq.Controllers
{
    public class StudentController : Controller
    {
        private readonly McqsDbContext _context;
        public StudentController(McqsDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> ExamList()
        {
            var examList = await _context.Exams
                .Include(e => e.Topic)
                    .ThenInclude(t => t.Property)
                .Include(e => e.Teacher)
                    .ThenInclude(t => t.User)
                .Select(e => new 
                {
                    Grade = e.Topic.Property.Grade,
                    Subject = e.Topic.Property.Subject,
                    TopicName = e.Topic.Name,
                    ExamId = e.Idexam,
                    ExamName = e.Name,
                    Duration = e.Duration,
                    NumberOfQuestions = e.number_of_question,
                    CreatedBy = e.Teacher.User.LastName + " " + e.Teacher.User.FirstName,
                })
                .ToListAsync();

            var groupedData = examList
                .GroupBy(e => e.Grade)
                .Select(g => new
                {
                    Grade = g.Key,
                    Subjects = g.GroupBy(e => e.Subject).Select(s => new
                    {
                        Subject = s.Key,
                        Topics = s.GroupBy(e => e.TopicName).Select(t => new
                        {
                            Topic = t.Key,
                            Exams = t.Select(e => new
                            {
                                e.ExamId,
                                e.ExamName,
                                e.Duration,
                                e.NumberOfQuestions,
                                e.CreatedBy
                            }).ToList()
                        }).ToList()
                    }).ToList()
                });

            return View(groupedData);
        }

        public IActionResult Detail(Guid examId)
        {
            Guid studentID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            var examDetail = _context.Exams
                .Where(e => e.Idexam == examId )
                .Include(e => e.Teacher)
                    .ThenInclude(t => t.User)
                .Select(e => new ExamDetailViewModel
                {
                    Idexam = e.Idexam,
                    ExamName = e.Name,
                    Duration = e.Duration,
                    NumberOfQuestions = e.Questions.Count,
                    CreatedBy = e.Teacher.User.LastName + " " + e.Teacher.User.FirstName,
                    AttemptHistories = _context.ExamAttempts
                        .Where(a => a.Idexam == e.Idexam && a.Idstudent == studentID)
                        .Select(a => new AttemptHistory
                        {
                            StartTime = a.StartTime,
                            CompletionTime = a.CompletionTime,
                            ExamScore = a.ExamScore,
                            NumsCorrectAns = a.NumsCorrectAns,
                            StudentAnswers = _context.StudentAnswers
                            .Where(sa => sa.Idattempt == a.Idattempt && sa.ExamAttempt.Idexam == e.Idexam)
                            .Include(e => e.Question)
                            .Select(sa => new StudentAnswerViewModel
                            {
                                QuestionName = sa.Question.Content,
                                SelectedAns = sa.SelectedAns,
                                IsCorrect = sa.IsCorrect
                            }).ToList()
                        }).ToList(),
                    
                })
                .FirstOrDefault();

            if (examDetail == null)
            {
                return NotFound();
            }

            return View(examDetail);
        }

        public IActionResult StartExam(Guid examId)
        {
            HttpContext.Session.SetString("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return RedirectToAction("ExamSession", new { examId });
        }


        [HttpPost]
        public IActionResult SubmitExam(Guid examId, Dictionary<Guid, List<string>> answers)
        {
            Guid studentId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            DateTime startTime = DateTime.Parse(HttpContext.Session.GetString("StartTime"));
            
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("ModelState Error: " + error.ErrorMessage);
                }
                return BadRequest("Invalid data.");
            }

            try
            {
                // Tạo chuỗi JSON thủ công từ dictionary answers
                var answersJson = "[";

                foreach (var answer in answers)
                {
                    var questionId = answer.Key;  // Giữ nguyên GUID của câu hỏi
                    var questionIdBytes = questionId.ToByteArray(); // Chuyển GUID thành mảng byte (BINARY(16))
                    
                    // Chuyển List<string> thành chuỗi JSON
                    var selectedAnswers = string.Join(",", answer.Value.Select(a => $"\"{a}\""));

                    // Tạo đối tượng JSON cho từng câu hỏi, lưu ý giữ question_id dưới dạng GUID (chuỗi) trong JSON
                    answersJson += $"{{\"question_id\": \"{Convert.ToBase64String(questionIdBytes)}\", \"selected_answers\": [{selectedAnswers}]}},"; 
                }

                // Loại bỏ dấu phẩy cuối cùng
                answersJson = answersJson.TrimEnd(',') + "]";

                // Kiểm tra và in ra dữ liệu để debug
                Console.WriteLine("Student ID: " + studentId);
                Console.WriteLine("Exam ID: " + examId);
                Console.WriteLine("Answers JSON: " + answersJson);

                // Gọi procedure `attempt_exam` với các tham số đã chuẩn bị
                _context.Database.ExecuteSqlRaw(
                    "CALL attempt_exam(@p_idstudent, @p_idexam, @p_answers, @p_start_time)",
                    new[]
                    {
                        new MySqlConnector.MySqlParameter("@p_idstudent", studentId.ToByteArray()),  // Chuyển GUID sinh viên thành BINARY(16)
                        new MySqlConnector.MySqlParameter("@p_idexam", examId.ToByteArray()),        // Chuyển GUID bài thi thành BINARY(16)
                        new MySqlConnector.MySqlParameter("@p_answers", answersJson),
                        new MySqlConnector.MySqlParameter("@p_start_time", startTime)                
                    }
                );

                // Thông báo thành công và chuyển hướng tới trang kết quả thi
                TempData["Message"] = "Nộp bài thành công!";
                HttpContext.Session.Remove("StartTime");
                return RedirectToAction("ExamResult", new { examId });
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi nếu có
                Console.WriteLine("Error occurred while submitting exam: " + ex);
                ViewBag.ErrorMessage = "Lỗi khi nộp bài thi: " + ex.Message;
                return RedirectToAction("ExamSession", new { examId });
            }
        }



        public IActionResult ExamSession(Guid examId)
        {
            // Lấy thông tin bài thi và câu hỏi
            var exam = _context.Exams
                .Include(e => e.Questions)
                    .ThenInclude (t => t.QuestionOptions)
                .FirstOrDefault(e => e.Idexam == examId)
                ;

            if (exam == null)
            {
                return NotFound();
            }

            try{
            var model = new ExamSessionViewModel
            {
                ExamId = exam.Idexam,
                ExamName = exam.Name,
                Duration = exam.Duration,
                Questions = exam.Questions.Select(q => new StudentQuestionViewModel
                {
                    QuestionId = q.Idquestion,
                    Content = q.Content,
                    Options = q.QuestionOptions.Select(o => new OptionViewModel
                    {
                        Content = o.option_content
                    }).ToList(),
                    Flagged = false
                }).ToList()
            };
                return View(model);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = "Lỗi khi nộp bài thi: " + ex.Message;
                Console.WriteLine("haaaaaa");
                return View();
                
            }

            
        }



        private Guid GetAttemptId(Guid studentId, Guid examId)
        {
            // Truy vấn ID Attempt dựa trên studentId và examId
            return _context.ExamAttempts
                .Where(a => a.Idstudent == studentId && a.Idexam == examId)
                .Select(a => a.Idattempt)
                .FirstOrDefault();
        }

        public IActionResult ExamResult(Guid examId)
        {
            Guid studentId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            // Lấy dữ liệu bài thi và kết quả làm bài
            var examResult = (from attempt in _context.ExamAttempts
                            join exam in _context.Exams on attempt.Idexam equals exam.Idexam
                            where attempt.Idstudent == studentId && attempt.Idexam == examId
                            orderby attempt.StartTime descending
                            select new
                            {
                                ExamName = exam.Name,
                                ExamScore = attempt.ExamScore,
                                TotalQuestions = exam.number_of_question,
                                CorrectAnswers = attempt.NumsCorrectAns,
                                StartTime = attempt.StartTime,
                                CompletionTime = attempt.CompletionTime
                            }).FirstOrDefault();

            if (examResult == null)
            {
                return NotFound("Exam result not found.");
            }

            // Tính thời gian làm bài
            var duration = (examResult.CompletionTime - examResult.StartTime).TotalMinutes;

            // Chuẩn bị dữ liệu để hiển thị
            var model = new ExamResultViewModel
            {
                ExamName = examResult.ExamName,
                ExamScore = (double)examResult.ExamScore,
                TotalQuestions = examResult.TotalQuestions,
                CorrectAnswers = examResult.CorrectAnswers,
                StartTime = examResult.StartTime,
                CompletionTime = examResult.CompletionTime,
                Duration = duration
            };

            return View(model);
        }


        public IActionResult Performance()
        {
            // Lấy id của sinh viên hiện tại
            Guid studentId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            // Lấy dữ liệu từ bảng EXAM_ATTEMPT
            var performanceData = (from attempt in _context.ExamAttempts
                                join exam in _context.Exams on attempt.Idexam equals exam.Idexam
                                where attempt.Idstudent == studentId
                                orderby attempt.StartTime descending
                                select new
                                {
                                    ExamName = exam.Name,
                                    Score = attempt.ExamScore,
                                    StartTime = attempt.StartTime,
                                    CompletionTime = attempt.CompletionTime,
                                    CorrectAnswers = attempt.NumsCorrectAns,
                                    TotalQuestions = exam.number_of_question
                                }).ToList();

            // Tổng số bài kiểm tra
            var totalExams = performanceData.Count;

            // Điểm trung bình
            var averageScore = performanceData.Any() ? (double)performanceData.Average(p => p.Score) : 0;

            // Tỷ lệ chính xác
            var totalCorrectAnswers = performanceData.Sum(p => p.CorrectAnswers);
            var totalQuestions = performanceData.Sum(p => p.TotalQuestions);
            var accuracyRate = totalQuestions > 0 ? (totalCorrectAnswers / (double)totalQuestions * 100) : 0;

            // Chuẩn bị ViewModel
            var viewModel = new PerformanceViewModel
            {
                PerformanceData = performanceData.Select(p => new ExamPerformance
                {
                    ExamName = p.ExamName,
                    Score = p.Score,
                    StartTime = p.StartTime,
                    EndTime = p.CompletionTime,
                    CorrectAnswers = p.CorrectAnswers,
                    TotalQuestions = p.TotalQuestions,
                    AccuracyRate = p.TotalQuestions > 0 ? (p.CorrectAnswers / (double)p.TotalQuestions * 100) : 0
                }).ToList(),
                TotalExams = totalExams,
                AverageScore = averageScore,
                AccuracyRate = accuracyRate
            };

            return View(viewModel);
        }











    }
}