import { api } from '../lib/api';
import type {
  QuizSummaryDto,
  QuizDetailDto,
  CreateQuizRequestDto,
  UpdateQuizRequestDto,
} from '../types/api';
import { MOCK_QUIZZES, MOCK_QUIZ_SUMMARIES } from '../lib/mockData';

// Toggle between mock and real API
const USE_MOCK = false;

export const quizService = {
  // GET: api/Quiz
  getAllQuizzes: async (): Promise<QuizSummaryDto[]> => {
    if (USE_MOCK) {
      return Promise.resolve(MOCK_QUIZ_SUMMARIES);
    }
    const response = await api.get<QuizSummaryDto[]>('/Quiz');
    return response.data;
  },

  // GET: api/Quiz/{quizId}
  getQuizById: async (quizId: number): Promise<QuizDetailDto> => {
    if (USE_MOCK) {
      const quiz = MOCK_QUIZZES.find((q) => q.id === quizId);
      if (!quiz) throw new Error('Quiz not found');
      return Promise.resolve(quiz);
    }
    const response = await api.get<QuizDetailDto>(`/Quiz/${quizId}`);
    return response.data;
  },

  // GET: api/Quiz/user/{userId}
  getQuizzesByUserId: async (userId: number): Promise<QuizSummaryDto[]> => {
    if (USE_MOCK) {
      const userQuizzes = MOCK_QUIZ_SUMMARIES.filter(
        (q) => MOCK_QUIZZES.find((mq) => mq.id === q.id)?.createdByUserId === userId
      );
      return Promise.resolve(userQuizzes);
    }
    const response = await api.get<QuizSummaryDto[]>(`/Quiz/user/${userId}`);
    return response.data;
  },

  // GET: api/Quiz/search?query={query}
  searchQuizzesByName: async (query: string): Promise<QuizSummaryDto[]> => {
    if (USE_MOCK) {
      const filtered = MOCK_QUIZ_SUMMARIES.filter((q) =>
        q.quizName.toLowerCase().includes(query.toLowerCase())
      );
      return Promise.resolve(filtered);
    }
    const response = await api.get<QuizSummaryDto[]>('/Quiz/search', {
      params: { query },
    });
    return response.data;
  },

  // POST: api/Quiz?creatorUserId={creatorUserId}
  createQuiz: async (
    quiz: CreateQuizRequestDto,
    creatorUserId: number
  ): Promise<QuizDetailDto> => {
    if (USE_MOCK) {
      const newQuiz: QuizDetailDto = {
        id: MOCK_QUIZZES.length + 1,
        quizName: quiz.quizName,
        createdByUserId: creatorUserId,
        creatorUsername: 'mock_user',
        questions: quiz.questions,
      };
      MOCK_QUIZZES.push(newQuiz);
      return Promise.resolve(newQuiz);
    }
    const response = await api.post<QuizDetailDto>('/Quiz', quiz, {
      params: { creatorUserId },
    });
    return response.data;
  },

  // PUT: api/Quiz/{quizId}?currentUserId={currentUserId}
  updateQuiz: async (
    quizId: number,
    quiz: UpdateQuizRequestDto,
    currentUserId: number
  ): Promise<QuizDetailDto> => {
    if (USE_MOCK) {
      const index = MOCK_QUIZZES.findIndex((q) => q.id === quizId);
      if (index === -1) throw new Error('Quiz not found');
      const updated = {
        ...MOCK_QUIZZES[index],
        quizName: quiz.quizName,
        questions: quiz.questions,
      };
      MOCK_QUIZZES[index] = updated;
      return Promise.resolve(updated);
    }
    const response = await api.put<QuizDetailDto>(`/Quiz/${quizId}`, quiz, {
      params: { currentUserId },
    });
    return response.data;
  },

  // DELETE: api/Quiz/{id}?currentUserId={currentUserId}
  deleteQuiz: async (id: number, currentUserId: number): Promise<void> => {
    if (USE_MOCK) {
      const index = MOCK_QUIZZES.findIndex((q) => q.id === id);
      if (index !== -1) {
        MOCK_QUIZZES.splice(index, 1);
      }
      return Promise.resolve();
    }
    await api.delete(`/Quiz/${id}`, {
      params: { currentUserId },
    });
  },
};
