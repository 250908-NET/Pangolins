import { api } from '../lib/api';
import type { UserDto, CreateUserDto, UserSummaryDto, UserDetailDto } from '../types/api';

export const userService = {
  // GET: api/User
  getAllUsers: async (): Promise<UserSummaryDto[]> => {
    const response = await api.get<UserSummaryDto[]>('/User');
    return response.data;
  },

  // GET: api/User/ById/{userId}
  getUserById: async (userId: number): Promise<UserDetailDto> => {
    const response = await api.get<UserDetailDto>(`/User/ById/${userId}`);
    return response.data;
  },

  // POST: api/User
  createUser: async (user: CreateUserDto): Promise<UserDto> => {
    const response = await api.post<UserDto>('/User', user);
    return response.data;
  },

  // GET: api/User/{username}
  getUserByUsername: async (username: string): Promise<UserDetailDto> => {
    const response = await api.get<UserDetailDto>(`/User/${username}`);
    return response.data;
  },

  // DELETE: api/User/{userId}
  deleteUser: async (userId: number): Promise<void> => {
    await api.delete(`/User/${userId}`);
  },
};
