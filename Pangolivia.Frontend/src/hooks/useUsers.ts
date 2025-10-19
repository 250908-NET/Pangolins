import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userService } from '../services/userService';
import type { CreateUserDto } from '../types/api';

// Query keys
export const userKeys = {
  all: ['users'] as const,
  lists: () => [...userKeys.all, 'list'] as const,
  details: () => [...userKeys.all, 'detail'] as const,
  detail: (id: number) => [...userKeys.details(), id] as const,
  byUsername: (username: string) => [...userKeys.all, 'username', username] as const,
};

// Queries
export const useUsers = () => {
  return useQuery({
    queryKey: userKeys.lists(),
    queryFn: () => userService.getAllUsers(),
  });
};

export const useUser = (userId: number) => {
  return useQuery({
    queryKey: userKeys.detail(userId),
    queryFn: () => userService.getUserById(userId),
    enabled: !!userId,
  });
};

export const useUserByUsername = (username: string) => {
  return useQuery({
    queryKey: userKeys.byUsername(username),
    queryFn: () => userService.getUserByUsername(username),
    enabled: !!username,
  });
};

// Mutations
export const useCreateUser = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (user: CreateUserDto) => userService.createUser(user),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.lists() });
    },
  });
};
