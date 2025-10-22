import { api } from '../lib/api';
import type { GameRecordDto, CreateGameRecordDto } from '../types/api';

export const gameRecordService = {
  // GET: api/GameRecord
  getAllGameRecords: async (): Promise<GameRecordDto[]> => {
    const response = await api.get<GameRecordDto[]>('/GameRecord');
    return response.data;
  },

  // GET: api/GameRecord/{id}
  getGameRecordById: async (id: number): Promise<GameRecordDto> => {
    const response = await api.get<GameRecordDto>(`/GameRecord/${id}`);
    return response.data;
  },

  // POST: api/GameRecord
  createGameRecord: async (record: CreateGameRecordDto): Promise<GameRecordDto> => {
    const response = await api.post<GameRecordDto>('/GameRecord', record);
    return response.data;
  },

  // GET: api/GameRecord/host/{hostUserId}
  getGameRecordsByHost: async (hostUserId: number): Promise<GameRecordDto[]> => {
    const response = await api.get<GameRecordDto[]>(`/GameRecord/host/${hostUserId}`);
    return response.data;
  },

  // GET: api/GameRecord/quiz/{quizId}
  getGameRecordsByQuiz: async (quizId: number): Promise<GameRecordDto[]> => {
    const response = await api.get<GameRecordDto[]>(`/GameRecord/quiz/${quizId}`);
    return response.data;
  },

  // PUT: api/GameRecord/{id}/complete
  completeGame: async (id: number): Promise<GameRecordDto> => {
    const response = await api.put<GameRecordDto>(`/GameRecord/${id}/complete`);
    return response.data;
  },

  // DELETE: api/GameRecord/{id}
  deleteGameRecord: async (id: number): Promise<void> => {
    await api.delete(`/GameRecord/${id}`);
  },
};
