import { api } from '../lib/api';

export const gameService = {
  createGame: async (quizId: number): Promise<{ roomCode: string }> => {
    const response = await api.post<{ roomCode: string }>('/games', { quizId });
    return response.data;
  },

  getGameDetailsByRoomCode: async (roomCode: string): Promise<{ quizId: number }> => {
    const response = await api.get<{ quizId: number }>(`/games/${roomCode.toUpperCase()}/details`);
    return response.data;
  },
};