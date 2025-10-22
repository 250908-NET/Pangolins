import { api } from '../lib/api';
import type {
  PlayerGameRecordDto,
  CreatePlayerGameRecordDto,
  UpdatePlayerGameRecordDto,
  LeaderboardDto,
} from '../types/api';

export const playerGameRecordService = {
  // POST: api/PlayerGameRecord
  recordScore: async (record: CreatePlayerGameRecordDto): Promise<PlayerGameRecordDto> => {
    const response = await api.post<PlayerGameRecordDto>('/PlayerGameRecord', record);
    return response.data;
  },

  // GET: api/PlayerGameRecord/leaderboard/{gameRecordId}
  getLeaderboard: async (gameRecordId: number): Promise<LeaderboardDto[]> => {
    const response = await api.get<LeaderboardDto[]>(
      `/PlayerGameRecord/leaderboard/${gameRecordId}`
    );
    return response.data;
  },

  // GET: api/PlayerGameRecord/history/{userId}
  getPlayerHistory: async (userId: number): Promise<PlayerGameRecordDto[]> => {
    const response = await api.get<PlayerGameRecordDto[]>(
      `/PlayerGameRecord/history/${userId}`
    );
    return response.data;
  },

  // GET: api/PlayerGameRecord/average/{userId}
  getAverageScore: async (userId: number): Promise<{ userId: number; averageScore: number }> => {
    const response = await api.get<{ userId: number; averageScore: number }>(
      `/PlayerGameRecord/average/${userId}`
    );
    return response.data;
  },

  // PUT: api/PlayerGameRecord/{recordId}
  updateScore: async (recordId: number, dto: UpdatePlayerGameRecordDto): Promise<void> => {
    await api.put(`/PlayerGameRecord/${recordId}`, dto);
  },

  // DELETE: api/PlayerGameRecord/{recordId}
  deleteRecord: async (recordId: number): Promise<void> => {
    await api.delete(`/PlayerGameRecord/${recordId}`);
  },
};
