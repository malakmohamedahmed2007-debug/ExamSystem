// ============================================================
//   EXAMINATION SYSTEM - C# Console Application
//   Demonstrates: Abstraction, Encapsulation,
//                 Inheritance, Polymorphism
// ============================================================

using System;
using System.Collections.Generic;

namespace ExaminationSystem
{
    // ─────────────────────────────────────────────────────────
    // ABSTRACTION: Abstract base class for all question types.
    // Defines a contract that every question must follow.
    // ─────────────────────────────────────────────────────────
    abstract class Question
    {
        // ENCAPSULATION: Properties with private setters
        public string Text  { get; private set; }
        public int    Marks { get; private set; }

        protected Question(string text, int marks)
        {
            Text  = text;
            Marks = marks;
        }

        // Each question type knows how to display itself
        public abstract void Display();

        // Each question type knows how to validate its answer
        // POLYMORPHISM: overridden differently in each subclass
        public abstract bool CheckAnswer(string userAnswer);
    }


    // ─────────────────────────────────────────────────────────
    // INHERITANCE: MCQ extends Question
    // ─────────────────────────────────────────────────────────
    class MultipleChoiceQuestion : Question
    {
        private List<string> _options;      // answer choices
        private int          _correctIndex; // 1-based correct option

        public MultipleChoiceQuestion(
            string text,
            int marks,
            List<string> options,
            int correctOptionNumber)        // e.g. pass 2 for option B
            : base(text, marks)
        {
            _options      = options;
            _correctIndex = correctOptionNumber;
        }

        // Display question text + lettered options
        public override void Display()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  {Text}");
            Console.ResetColor();

            char letter = 'A';
            foreach (string option in _options)
            {
                Console.WriteLine($"    {letter}) {option}");
                letter++;
            }
            Console.WriteLine($"  [Marks: {Marks}]");
        }

        // POLYMORPHISM: MCQ checks by letter (A/B/C/D)
        public override bool CheckAnswer(string userAnswer)
        {
            // Convert user's letter to an index (A=1, B=2, …)
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            char input = char.ToUpper(userAnswer.Trim()[0]);
            int  index = input - 'A' + 1; // A→1, B→2, etc.
            return index == _correctIndex;
        }
    }


    // ─────────────────────────────────────────────────────────
    // INHERITANCE: True/False extends Question
    // ─────────────────────────────────────────────────────────
    class TrueFalseQuestion : Question
    {
        private bool _correctAnswer; // true = "True", false = "False"

        public TrueFalseQuestion(string text, int marks, bool correctAnswer)
            : base(text, marks)
        {
            _correctAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  {Text}");
            Console.ResetColor();
            Console.WriteLine("    A) True    B) False");
            Console.WriteLine($"  [Marks: {Marks}]");
        }

        // POLYMORPHISM: T/F checks for True or False keywords
        public override bool CheckAnswer(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            string input = userAnswer.Trim().ToUpper();

            // Accept "TRUE" / "T" / "A"  or  "FALSE" / "F" / "B"
            bool answeredTrue  = (input == "TRUE"  || input == "T" || input == "A");
            bool answeredFalse = (input == "FALSE" || input == "F" || input == "B");

            if (!answeredTrue && !answeredFalse)
                return false; // invalid input treated as wrong

            return answeredTrue == _correctAnswer;
        }
    }


    // ─────────────────────────────────────────────────────────
    // Exam class: orchestrates the entire examination session
    // ─────────────────────────────────────────────────────────
    class Exam
    {
        private string          _title;
        private List<Question>  _questions;
        private string          _studentName;

        public Exam(string title, string studentName)
        {
            _title       = title;
            _studentName = studentName;
            _questions   = new List<Question>();
        }

        // Add any Question subtype (MCQ, T/F, or future types)
        public void AddQuestion(Question question)
        {
            _questions.Add(question);
        }

        // Run the full exam session
        public void Start()
        {
            PrintHeader();

            int totalMarks   = 0;
            int earnedMarks  = 0;
            int questionNum  = 1;

            foreach (Question q in _questions)
            {
                totalMarks += q.Marks;

                Console.WriteLine($"\n  Question {questionNum} of {_questions.Count}");
                Console.WriteLine("  " + new string('─', 45));

                // POLYMORPHISM: calls the correct Display() at runtime
                q.Display();

                string userAnswer = GetValidInput();

                // POLYMORPHISM: calls the correct CheckAnswer() at runtime
                bool isCorrect = q.CheckAnswer(userAnswer);

                if (isCorrect)
                {
                    earnedMarks += q.Marks;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ✔  Correct!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  ✘  Wrong answer.");
                }

                Console.ResetColor();
                questionNum++;
            }

            PrintResult(earnedMarks, totalMarks);
        }

        // Prompt user and guard against empty input
        private string GetValidInput()
        {
            string input;
            do
            {
                Console.Write("\n  Your answer: ");
                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  ⚠  Please enter an answer.");
                    Console.ResetColor();
                }
            }
            while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        private void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ╔══════════════════════════════════════════════╗");
            Console.WriteLine($"  ║  {_title.PadRight(44)}║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"  Student : {_studentName}");
            Console.WriteLine($"  Questions: {_questions.Count}");
            Console.WriteLine("  " + new string('─', 45));
            Console.WriteLine("  Press ENTER to begin...");
            Console.ReadLine();
        }

        private void PrintResult(int earned, int total)
        {
            double percentage = total > 0 ? (double)earned / total * 100 : 0;
            string grade      = GetGrade(percentage);

            Console.WriteLine("\n  " + new string('═', 45));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("              EXAM COMPLETE");
            Console.ResetColor();
            Console.WriteLine("  " + new string('═', 45));
            Console.WriteLine($"  Student   : {_studentName}");
            Console.WriteLine($"  Score     : {earned} / {total}");
            Console.WriteLine($"  Percentage: {percentage:F1}%");

            Console.ForegroundColor = percentage >= 50
                ? ConsoleColor.Green
                : ConsoleColor.Red;
            Console.WriteLine($"  Grade     : {grade}");
            Console.ResetColor();

            Console.WriteLine("  " + new string('═', 45));
            Console.WriteLine("\n  Thank you for taking the exam!");
        }

        private string GetGrade(double percentage)
        {
            if (percentage >= 90) return "A+ (Excellent)";
            if (percentage >= 80) return "A  (Very Good)";
            if (percentage >= 70) return "B  (Good)";
            if (percentage >= 60) return "C  (Above Average)";
            if (percentage >= 50) return "D  (Pass)";
            return "F  (Fail)";
        }
    }


    // ─────────────────────────────────────────────────────────
    // Entry Point
    // ─────────────────────────────────────────────────────────
    class Program
    {
        static void Main(string[] args)
        {
            // Greet user and collect name
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║         EXAMINATION SYSTEM  v1.0             ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.Write("\n  Enter your name: ");
            string studentName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(studentName))
                studentName = "Student";

            // ── Build the exam ────────────────────────────────
            Exam exam = new Exam("C# & OOP Fundamentals Quiz", studentName);

            // MCQ #1
            exam.AddQuestion(new MultipleChoiceQuestion(
                text: "Which keyword is used to define an abstract class in C#?",
                marks: 2,
                options: new List<string> { "virtual", "abstract", "interface", "override" },
                correctOptionNumber: 2   // B) abstract
            ));

            // MCQ #2
            exam.AddQuestion(new MultipleChoiceQuestion(
                text: "What does OOP stand for?",
                marks: 2,
                options: new List<string> {
                    "Object-Oriented Programming",
                    "Object-Ordered Processing",
                    "Open Object Platform",
                    "Operator-Oriented Programming"
                },
                correctOptionNumber: 1   // A
            ));

            // True/False #1
            exam.AddQuestion(new TrueFalseQuestion(
                text: "A class can inherit from multiple classes in C#.",
                marks: 1,
                correctAnswer: false     // False — C# uses single inheritance
            ));

            // True/False #2
            exam.AddQuestion(new TrueFalseQuestion(
                text: "Encapsulation means hiding internal data using access modifiers.",
                marks: 1,
                correctAnswer: true
            ));

            // MCQ #3
            exam.AddQuestion(new MultipleChoiceQuestion(
                text: "Which OOP concept allows a method to behave differently based on the object?",
                marks: 3,
                options: new List<string> { "Encapsulation", "Abstraction", "Polymorphism", "Inheritance" },
                correctOptionNumber: 3   // C) Polymorphism
            ));

            // True/False #3
            exam.AddQuestion(new TrueFalseQuestion(
                text: "An interface in C# can contain method implementations (default methods).",
                marks: 2,
                correctAnswer: true      // True — since C# 8.0
            ));

            // ── Run the exam ──────────────────────────────────
            exam.Start();

            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey();
        }
    }
}
