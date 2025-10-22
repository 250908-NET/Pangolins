import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { gameRecordService } from '../services/gameRecordService';
import type { CreateGameRecordDto } from '../types/api';
import { userKeys } from './useUsers';

// Query keys
export const gameRecordKeys = {
  all: ['gameRecords'] as const,
  lists: () => [...gameRecordKeys.all, 'list'] as const,
  details: () => [...gameRecordKeys.all, 'detail'] as const,
  detail: (id: number) => [...gameRecordKeys.details(), id] as const,
  byHost: (hostUserId: number) => [...gameRecordKeys.all, 'host', hostUserId] as const,
  byQuiz: (quizId: number) => [...gameRecordKeys.all, 'quiz', quizId] as const,
};

// Queries
export const useGameRecords = () => {
  return useQuery({
    queryKey: gameRecordKeys.lists(),
    queryFn: () => gameRecordService.getAllGameRecords(),
  });
};

export const useGameRecord = (id: number) => {
  return useQuery({
    queryKey: gameRecordKeys.detail(id),
    queryFn: () => gameRecordService.getGameRecordById(id),
    enabled: !!id,
  });
};

export const useGameRecordsByHost = (hostUserId: number) => {
  return useQuery({
    queryKey: gameRecordKeys.byHost(hostUserId),
    queryFn: () => gameRecordService.getGameRecordsByHost(hostUserId),
    enabled: !!hostUserId,
  });
};

export const useGameRecordsByQuiz = (quizId: number) => {
  return useQuery({
    queryKey: gameRecordKeys.byQuiz(quizId),
    queryFn: () => gameRecordService.getGameRecordsByQuiz(quizId),
    enabled: !!quizId,
  });
};

// Mutations
export const useCreateGameRecord = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (record: CreateGameRecordDto) => gameRecordService.createGameRecord(record),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.all });
      // Invalidate user data to update hostedGamesCount in Profile
      queryClient.invalidateQueries({ queryKey: userKeys.detail(variables.hostUserId) });
    },
  });
};

export const useCompleteGame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => gameRecordService.completeGame(id),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.all });
      // Invalidate host user data to update hostedGamesCount in Profile
      if (data.hostUserId) {
        queryClient.invalidateQueries({ queryKey: userKeys.detail(data.hostUserId) });
      }
    },
  });
};

export const useDeleteGameRecord = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => gameRecordService.deleteGameRecord(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.all });
      // Invalidate all user data since we don't know which user hosted the deleted game
      queryClient.invalidateQueries({ queryKey: userKeys.all });
    },
  });
};
