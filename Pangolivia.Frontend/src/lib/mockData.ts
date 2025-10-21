import type { UserDto, GameRecordDto } from '../types/api';

export const MOCK_USERS: UserDto[] = [
  {
    id: 1,
    authUuid: 'auth-uuid-1',
    username: 'alice_wonderland',
  },
  {
    id: 2,
    authUuid: 'auth-uuid-2',
    username: 'bob_builder',
  },
  {
    id: 3,
    authUuid: 'auth-uuid-3',
    username: 'charlie_chocolate',
  },
  {
    id: 4,
    authUuid: 'auth-uuid-4',
    username: 'diana_prince',
  },
];

// MOCK_QUIZZES and MOCK_QUIZ_SUMMARIES have been removed as they are no longer used
// and their structure was outdated compared to the current QuestionDto.

export const MOCK_GAME_RECORDS: GameRecordDto[] = [
  {
    id: 1,
    hostUserId: 1,
    quizId: 1,
    quizName: 'JavaScript Fundamentals',
    datetimeCompleted: new Date('2025-01-15T14:30:00').toISOString(),
  },
  {
    id: 2,
    hostUserId: 2,
    quizId: 2,
    quizName: 'React Basics',
    datetimeCompleted: new Date('2025-01-16T10:15:00').toISOString(),
  },
];