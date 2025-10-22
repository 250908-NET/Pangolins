// TypeScript types matching the C# DTOs from Pangolivia.API

export interface QuestionDto {
  id: number;
  questionText: string;
  // This interface now matches the C# backend DTO
  correctAnswer: string;
  answer2: string;
  answer3: string;
  answer4: string;
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

export interface UserSummaryDto {
  id: number;
  username: string;
}

export interface UserDetailDto {
  id: number;
  username: string;
  createdQuizzes: QuizSummaryDto[];
  hostedGamesCount: number;
  gamesPlayedCount: number;
}

export interface CreateGameRecordDto {
  hostUserId: number;
  quizId: number;
  playerScores?: CreatePlayerGameRecordDto[];
  dateTimeCompleted?: string;
}

export interface GameRecordDto {
  id: number;
  hostUserId: number;
  hostUsername: string;
  quizId: number;
  quizName: string;
  playerScores: PlayerGameRecordDto[];
  dateTimeCompleted: string;
}

export interface PlayerGameRecordDto {
  id?: number;
  userId?: number;
  username: string;
  gameRecordId?: number;
  score: number;
}

export interface CreatePlayerGameRecordDto {
  gameRecordId?: number;
  userId: number;
  score: number;
}

export interface UpdatePlayerGameRecordDto {
  score: number;
}

export interface LeaderboardDto {
  username: string;
  score: number;
  rank: number;
}

export interface GenerateQuizAiRequestDto {
  topic: string;
  numberOfQuestions: number;
  difficulty: string;
}