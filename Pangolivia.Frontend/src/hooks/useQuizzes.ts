import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { quizService } from '../services/quizService';
import type { CreateQuizRequestDto, UpdateQuizRequestDto, GenerateQuizAiRequestDto } from '../types/api';
import { userKeys } from './useUsers';

// Query keys
export const quizKeys = {
  all: ['quizzes'] as const,
  lists: () => [...quizKeys.all, 'list'] as const,
  list: (filters?: string) => [...quizKeys.lists(), { filters }] as const,
  details: () => [...quizKeys.all, 'detail'] as const,
  detail: (id: number) => [...quizKeys.details(), id] as const,
  byUser: (userId: number) => [...quizKeys.all, 'user', userId] as const,
  search: (query: string) => [...quizKeys.all, 'search', query] as const,
};

// Queries
export const useQuizzes = () => {
  return useQuery({
    queryKey: quizKeys.lists(),
    queryFn: () => quizService.getAllQuizzes(),
  });
};

export const useGenerateAiQuestions = () => {
  return useMutation({
    mutationFn: (payload: GenerateQuizAiRequestDto) =>
      quizService.generateAiQuestions(payload),
  });
};

export const useQuiz = (quizId: number) => {
  return useQuery({
    queryKey: quizKeys.detail(quizId),
    queryFn: () => quizService.getQuizById(quizId),
    enabled: !!quizId,
  });
};

export const useQuizzesByUser = (userId: number) => {
  return useQuery({
    queryKey: quizKeys.byUser(userId),
    queryFn: () => quizService.getQuizzesByUserId(userId),
    enabled: !!userId,
  });
};

export const useSearchQuizzes = (query: string) => {
  return useQuery({
    queryKey: quizKeys.search(query),
    queryFn: () => quizService.searchQuizzesByName(query),
    enabled: query.length > 0,
  });
};

// Mutations
export const useCreateQuiz = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ quiz, creatorUserId }: { quiz: CreateQuizRequestDto; creatorUserId: number }) =>
      quizService.createQuiz(quiz, creatorUserId),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: quizKeys.lists() });
      queryClient.invalidateQueries({ queryKey: quizKeys.byUser(variables.creatorUserId) });
      // Invalidate user data to update createdQuizzes count in Profile
      queryClient.invalidateQueries({ queryKey: userKeys.detail(variables.creatorUserId) });
    },
  });
};

export const useUpdateQuiz = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      quizId,
      quiz,
      currentUserId,
    }: {
      quizId: number;
      quiz: UpdateQuizRequestDto;
      currentUserId: number;
    }) => quizService.updateQuiz(quizId, quiz, currentUserId),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: quizKeys.detail(variables.quizId) });
      queryClient.invalidateQueries({ queryKey: quizKeys.lists() });
      queryClient.invalidateQueries({ queryKey: quizKeys.byUser(variables.currentUserId) });
      // Invalidate user data to update createdQuizzes in Profile
      queryClient.invalidateQueries({ queryKey: userKeys.detail(variables.currentUserId) });
    },
  });
};

export const useDeleteQuiz = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, currentUserId }: { id: number; currentUserId: number }) =>
      quizService.deleteQuiz(id, currentUserId),
    onSuccess: (_data, variables) => {
      // Invalidate all quiz-related queries to ensure UI updates
      queryClient.invalidateQueries({ queryKey: quizKeys.all });
      // Invalidate user data to update createdQuizzes count in Profile
      queryClient.invalidateQueries({ queryKey: userKeys.detail(variables.currentUserId) });
    },
  });
};
