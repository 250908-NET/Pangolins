import { api } from '../lib/api';
import type { UserDto, CreateUserDto } from '../types/api';
import { MOCK_USERS } from '../lib/mockData';

// Toggle between mock and real API
const USE_MOCK = true; // Keep as true since UserController is not implemented yet

export const userService = {
  // GET: api/User (placeholder - not implemented in backend yet)
  getAllUsers: async (): Promise<UserDto[]> => {
    if (USE_MOCK) {
      return Promise.resolve(MOCK_USERS);
    }
    const response = await api.get<UserDto[]>('/User');
    return response.data;
  },

  // GET: api/User/{userId}
  getUserById: async (userId: number): Promise<UserDto> => {
    if (USE_MOCK) {
      const user = MOCK_USERS.find((u) => u.id === userId);
      if (!user) throw new Error('User not found');
      return Promise.resolve(user);
    }
    const response = await api.get<UserDto>(`/User/${userId}`);
    return response.data;
  },

  // POST: api/User
  createUser: async (user: CreateUserDto): Promise<UserDto> => {
    if (USE_MOCK) {
      const newUser: UserDto = {
        id: MOCK_USERS.length + 1,
        authUuid: user.authUuid,
        username: user.username,
      };
      MOCK_USERS.push(newUser);
      return Promise.resolve(newUser);
    }
    const response = await api.post<UserDto>('/User', user);
    return response.data;
  },

  // GET: api/User/username/{username}
  getUserByUsername: async (username: string): Promise<UserDto> => {
    if (USE_MOCK) {
      const user = MOCK_USERS.find((u) => u.username === username);
      if (!user) throw new Error('User not found');
      return Promise.resolve(user);
    }
    const response = await api.get<UserDto>(`/User/username/${username}`);
    return response.data;
  },
};
