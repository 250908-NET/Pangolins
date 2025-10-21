// Hardcoded mock data for development
import type { UserDto, QuizDetailDto, QuizSummaryDto, GameRecordDto } from '../types/api';

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

export const MOCK_QUIZZES: QuizDetailDto[] = [
  {
    id: 1,
    quizName: 'JavaScript Fundamentals',
    createdByUserId: 1,
    creatorUsername: 'alice_wonderland',
    questions: [
      {
        id: 1,
        questionText: 'What is the output of typeof null?',
        options: ['null', 'undefined', 'object', 'number'],
        correctOptionIndex: 2,
      },
      {
        id: 2,
        questionText: 'Which method adds elements to the end of an array?',
        options: ['push()', 'pop()', 'shift()', 'unshift()'],
        correctOptionIndex: 0,
      },
      {
        id: 3,
        questionText: 'What does "===" check for?',
        options: ['Value only', 'Type only', 'Value and type', 'Reference'],
        correctOptionIndex: 2,
      },
    ],
  },
  {
    id: 2,
    quizName: 'React Basics',
    createdByUserId: 2,
    creatorUsername: 'bob_builder',
    questions: [
      {
        id: 4,
        questionText: 'What hook is used for side effects?',
        options: ['useState', 'useEffect', 'useContext', 'useMemo'],
        correctOptionIndex: 1,
      },
      {
        id: 5,
        questionText: 'What is JSX?',
        options: ['A JavaScript library', 'A syntax extension', 'A framework', 'A compiler'],
        correctOptionIndex: 1,
      },
    ],
  },
  {
    id: 3,
    quizName: 'CSS Grid & Flexbox',
    createdByUserId: 1,
    creatorUsername: 'alice_wonderland',
    questions: [
      {
        id: 6,
        questionText: 'Which property defines the main axis in Flexbox?',
        options: ['align-items', 'justify-content', 'flex-direction', 'flex-wrap'],
        correctOptionIndex: 2,
      },
    ],
  },
];

export const MOCK_QUIZ_SUMMARIES: QuizSummaryDto[] = MOCK_QUIZZES.map((quiz) => ({
  id: quiz.id,
  quizName: quiz.quizName,
  questionCount: quiz.questions.length,
  creatorUsername: quiz.creatorUsername,
}));

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
