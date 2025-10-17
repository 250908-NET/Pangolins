// TypeScript types matching the C# DTOs from Pangolivia.API

export interface QuestionDto {
  id: number;
  questionText: string;
  options: string[];
  correctOptionIndex: number;
}

export interface QuizSummaryDto {
  id: number;
  quizName: string;
  questionCount: number;
  creatorUsername: string;
}

export interface QuizDetailDto {
  id: number;
  quizName: string;
  createdByUserId: number;
  creatorUsername: string;
  questions: QuestionDto[];
}

export interface CreateQuizRequestDto {
  quizName: string;
  questions: QuestionDto[];
}

export interface UpdateQuizRequestDto {
  quizName: string;
  questions: QuestionDto[];
}

export interface CreateUserDto {
  authUuid: string;
  username: string;
}

export interface UserDto {
  id: number;
  authUuid: string;
  username: string;
}

export interface CreateGameRecordDto {
  hostUserId: number;
  quizId: number;
}

export interface GameRecordDto {
  id: number;
  hostUserId: number;
  quizId: number;
  quizName?: string;
  datetimeCompleted: string;
}
