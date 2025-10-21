import { api } from '../lib/api';
import type { GameRecordDto, CreateGameRecordDto } from '../types/api';
import { MOCK_GAME_RECORDS } from '../lib/mockData';

// Toggle between mock and real API
const USE_MOCK = true; // Keep as true since GameRecordController is not implemented yet

export const gameRecordService = {
  // GET: api/GameRecord (placeholder)
  getAllGameRecords: async (): Promise<GameRecordDto[]> => {
    if (USE_MOCK) {
      return Promise.resolve(MOCK_GAME_RECORDS);
    }
    const response = await api.get<GameRecordDto[]>('/GameRecord');
    return response.data;
  },

  // GET: api/GameRecord/{id}
  getGameRecordById: async (id: number): Promise<GameRecordDto> => {
    if (USE_MOCK) {
      const record = MOCK_GAME_RECORDS.find((r) => r.id === id);
      if (!record) throw new Error('Game record not found');
      return Promise.resolve(record);
    }
    const response = await api.get<GameRecordDto>(`/GameRecord/${id}`);
    return response.data;
  },

  // POST: api/GameRecord
  createGameRecord: async (record: CreateGameRecordDto): Promise<GameRecordDto> => {
    if (USE_MOCK) {
      const newRecord: GameRecordDto = {
        id: MOCK_GAME_RECORDS.length + 1,
        hostUserId: record.hostUserId,
        quizId: record.quizId,
        quizName: 'New Game',
        datetimeCompleted: new Date().toISOString(),
      };
      MOCK_GAME_RECORDS.push(newRecord);
      return Promise.resolve(newRecord);
    }
    const response = await api.post<GameRecordDto>('/GameRecord', record);
    return response.data;
  },

  // GET: api/GameRecord/user/{userId}
  getGameRecordsByUserId: async (userId: number): Promise<GameRecordDto[]> => {
    if (USE_MOCK) {
      const records = MOCK_GAME_RECORDS.filter((r) => r.hostUserId === userId);
      return Promise.resolve(records);
    }
    const response = await api.get<GameRecordDto[]>(`/GameRecord/user/${userId}`);
    return response.data;
  },
};
