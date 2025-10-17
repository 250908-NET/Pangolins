"use client";

import { useEffect, useState } from "react";
import axios from "axios";

interface Quiz {
  id: number;
  quizName: string;
}

export default function QuizPage() {
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);

  useEffect(() => {
    const fetchQuizzes = async () => {
      try {
        const res = await axios.get("/api/quiz");
        setQuizzes(res.data);
      } catch (err) {
        console.error("Error fetching quizzes:", err);
      }
    };
    fetchQuizzes();
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">All Quizzes</h1>
      <ul className="space-y-2">
        {quizzes.map((q) => (
          <li key={q.id} className="p-2 border rounded">
            {q.quizName}
          </li>
        ))}
      </ul>
    </div>
  );
}
