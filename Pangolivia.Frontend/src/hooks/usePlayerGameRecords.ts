import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { playerGameRecordService } from '../services/playerGameRecordService';
import type { CreatePlayerGameRecordDto, UpdatePlayerGameRecordDto } from '../types/api';
import { userKeys } from './useUsers';

// Query keys
export const playerGameRecordKeys = {
  all: ['playerGameRecords'] as const,
  leaderboards: () => [...playerGameRecordKeys.all, 'leaderboard'] as const,
  leaderboard: (gameRecordId: number) =>
    [...playerGameRecordKeys.leaderboards(), gameRecordId] as const,
  histories: () => [...playerGameRecordKeys.all, 'history'] as const,
  history: (userId: number) => [...playerGameRecordKeys.histories(), userId] as const,
  averages: () => [...playerGameRecordKeys.all, 'average'] as const,
  average: (userId: number) => [...playerGameRecordKeys.averages(), userId] as const,
};

// Queries
export const useLeaderboard = (gameRecordId: number) => {
  return useQuery({
    queryKey: playerGameRecordKeys.leaderboard(gameRecordId),
    queryFn: () => playerGameRecordService.getLeaderboard(gameRecordId),
    enabled: !!gameRecordId,
  });
};

export const usePlayerHistory = (userId: number) => {
  return useQuery({
    queryKey: playerGameRecordKeys.history(userId),
    queryFn: () => playerGameRecordService.getPlayerHistory(userId),
    enabled: !!userId,
  });
};

export const useAverageScore = (userId: number) => {
  return useQuery({
    queryKey: playerGameRecordKeys.average(userId),
    queryFn: () => playerGameRecordService.getAverageScore(userId),
    enabled: !!userId,
  });
};

// Mutations
export const useRecordScore = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (record: CreatePlayerGameRecordDto) =>
      playerGameRecordService.recordScore(record),
    onSuccess: (data) => {
      // Invalidate relevant queries
      if (data.userId) {
        queryClient.invalidateQueries({
          queryKey: playerGameRecordKeys.history(data.userId),
        });
        queryClient.invalidateQueries({
          queryKey: playerGameRecordKeys.average(data.userId),
        });
        // Invalidate user data to update gamesPlayedCount in Profile
        queryClient.invalidateQueries({ queryKey: userKeys.detail(data.userId) });
      }
      if (data.gameRecordId) {
        queryClient.invalidateQueries({
          queryKey: playerGameRecordKeys.leaderboard(data.gameRecordId),
        });
      }
    },
  });
};

export const useUpdateScore = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ recordId, dto }: { recordId: number; dto: UpdatePlayerGameRecordDto }) =>
      playerGameRecordService.updateScore(recordId, dto),
    onSuccess: () => {
      // Invalidate all player game records and user data
      queryClient.invalidateQueries({ queryKey: playerGameRecordKeys.all });
      // Invalidate all user data since we don't know which user's score was updated
      queryClient.invalidateQueries({ queryKey: userKeys.all });
    },
  });
};

export const useDeleteRecord = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (recordId: number) => playerGameRecordService.deleteRecord(recordId),
    onSuccess: () => {
      // Invalidate all player game records and user data
      queryClient.invalidateQueries({ queryKey: playerGameRecordKeys.all });
      // Invalidate all user data since we don't know which user's record was deleted
      queryClient.invalidateQueries({ queryKey: userKeys.all });
    },
  });
};
