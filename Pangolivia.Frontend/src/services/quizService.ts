import { api } from "../lib/api";
import type {
  QuizSummaryDto,
  QuizDetailDto,
  CreateQuizRequestDto,
  UpdateQuizRequestDto,
  GenerateQuizAiRequestDto,
} from "../types/api";

export const quizService = {
  // GET: api/Quiz
  getAllQuizzes: async (): Promise<QuizSummaryDto[]> => {
    const response = await api.get<QuizSummaryDto[]>("/Quiz");
    return response.data;
  },

  // GET: api/Quiz/{quizId}
  getQuizById: async (quizId: number): Promise<QuizDetailDto> => {
    const response = await api.get<QuizDetailDto>(`/Quiz/${quizId}`);
    return response.data;
  },

  // GET: api/Quiz/user/{userId}
  getQuizzesByUserId: async (userId: number): Promise<QuizSummaryDto[]> => {
    const response = await api.get<QuizSummaryDto[]>(`/Quiz/user/${userId}`);
    return response.data;
  },

  // GET: api/Quiz/search?query={query}
  searchQuizzesByName: async (query: string): Promise<QuizSummaryDto[]> => {
    const response = await api.get<QuizSummaryDto[]>("/Quiz/search", {
      params: { query },
    });
    return response.data;
  },

  // POST: api/Quiz?creatorUserId={creatorUserId}
  createQuiz: async (
    quiz: CreateQuizRequestDto,
    creatorUserId: number
  ): Promise<QuizDetailDto> => {
    const response = await api.post<QuizDetailDto>("/Quiz", quiz, {
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
    const response = await api.put<QuizDetailDto>(`/Quiz/${quizId}`, quiz, {
      params: { currentUserId },
    });
    return response.data;
  },

  // DELETE: api/Quiz/{id}?currentUserId={currentUserId}
  deleteQuiz: async (id: number, currentUserId: number): Promise<void> => {
    await api.delete(`/Quiz/${id}`, {
      params: { currentUserId },
    });
  },

  // POST: api/Quiz/ai/generate
  generateAiQuestions: async (
    payload: GenerateQuizAiRequestDto
  ): Promise<QuizDetailDto["questions"]> => {
    const response = await api.post(`/Quiz/ai/generate`, payload);
    return response.data as QuizDetailDto["questions"];
  },
};