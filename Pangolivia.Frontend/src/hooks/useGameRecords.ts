import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { gameRecordService } from '../services/gameRecordService';
import type { CreateGameRecordDto } from '../types/api';

// Query keys
export const gameRecordKeys = {
  all: ['gameRecords'] as const,
  lists: () => [...gameRecordKeys.all, 'list'] as const,
  details: () => [...gameRecordKeys.all, 'detail'] as const,
  detail: (id: number) => [...gameRecordKeys.details(), id] as const,
  byUser: (userId: number) => [...gameRecordKeys.all, 'user', userId] as const,
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

export const useGameRecordsByUser = (userId: number) => {
  return useQuery({
    queryKey: gameRecordKeys.byUser(userId),
    queryFn: () => gameRecordService.getGameRecordsByUserId(userId),
    enabled: !!userId,
  });
};

// Mutations
export const useCreateGameRecord = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (record: CreateGameRecordDto) => gameRecordService.createGameRecord(record),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.lists() });
    },
  });
};
