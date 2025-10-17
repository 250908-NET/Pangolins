# API Integration Guide

This document explains how to use the API infrastructure in the Pangolivia frontend.

## Overview

The frontend uses:
- **Axios** for HTTP requests
- **React Query** for data fetching, caching, and state management
- **TypeScript** for type safety

## Architecture

```
src/
├── types/
│   └── api.ts              # TypeScript types matching C# DTOs
├── lib/
│   ├── api.ts              # Axios instance with interceptors
│   └── mockData.ts         # Hardcoded mock data for development
├── services/
│   ├── quizService.ts      # Quiz API calls
│   ├── userService.ts      # User API calls (hardcoded for now)
│   └── gameRecordService.ts # Game record API calls
└── hooks/
    ├── useQuizzes.ts       # React Query hooks for quizzes
    ├── useUsers.ts         # React Query hooks for users
    └── useGameRecords.ts   # React Query hooks for game records
```

## Configuration

### Environment Variables

Create `.env.development` and `.env.production` files:

```env
VITE_API_URL=http://localhost:3001/api
```

### Toggle Mock Data

In each service file, set `USE_MOCK = true` for development with hardcoded data, or `false` to use the real API:

```typescript
// src/services/quizService.ts
const USE_MOCK = true; // Change to false when API is ready
```

## Usage Examples

### 1. Fetching Data (Queries)

#### Get All Quizzes

```tsx
import { useQuizzes } from '@/hooks/useQuizzes';

function QuizList() {
  const { data: quizzes, isLoading, error } = useQuizzes();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {quizzes?.map(quiz => (
        <div key={quiz.id}>{quiz.quizName}</div>
      ))}
    </div>
  );
}
```

#### Get Single Quiz

```tsx
import { useQuiz } from '@/hooks/useQuizzes';

function QuizDetail({ quizId }: { quizId: number }) {
  const { data: quiz, isLoading } = useQuiz(quizId);

  if (isLoading) return <div>Loading...</div>;

  return (
    <div>
      <h1>{quiz?.quizName}</h1>
      <p>Created by: {quiz?.creatorUsername}</p>
      <ul>
        {quiz?.questions.map(q => (
          <li key={q.id}>{q.questionText}</li>
        ))}
      </ul>
    </div>
  );
}
```

#### Search Quizzes

```tsx
import { useSearchQuizzes } from '@/hooks/useQuizzes';
import { useState } from 'react';

function QuizSearch() {
  const [query, setQuery] = useState('');
  const { data: results } = useSearchQuizzes(query);

  return (
    <div>
      <input
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search quizzes..."
      />
      {results?.map(quiz => (
        <div key={quiz.id}>{quiz.quizName}</div>
      ))}
    </div>
  );
}
```

### 2. Mutating Data (Create, Update, Delete)

#### Create Quiz

```tsx
import { useCreateQuiz } from '@/hooks/useQuizzes';

function CreateQuizForm() {
  const createQuiz = useCreateQuiz();

  const handleSubmit = async () => {
    try {
      const newQuiz = await createQuiz.mutateAsync({
        quiz: {
          quizName: 'My New Quiz',
          questions: [
            {
              id: 0,
              questionText: 'What is React?',
              options: ['Library', 'Framework', 'Language', 'Tool'],
              correctOptionIndex: 0,
            },
          ],
        },
        creatorUserId: 1, // Hardcoded for now
      });
      console.log('Created quiz:', newQuiz);
    } catch (error) {
      console.error('Failed to create quiz:', error);
    }
  };

  return (
    <button
      onClick={handleSubmit}
      disabled={createQuiz.isPending}
    >
      {createQuiz.isPending ? 'Creating...' : 'Create Quiz'}
    </button>
  );
}
```

#### Update Quiz

```tsx
import { useUpdateQuiz } from '@/hooks/useQuizzes';

function EditQuizForm({ quizId }: { quizId: number }) {
  const updateQuiz = useUpdateQuiz();

  const handleUpdate = async () => {
    try {
      await updateQuiz.mutateAsync({
        quizId,
        quiz: {
          quizName: 'Updated Quiz Name',
          questions: [/* updated questions */],
        },
        currentUserId: 1,
      });
    } catch (error) {
      console.error('Failed to update quiz:', error);
    }
  };

  return <button onClick={handleUpdate}>Update Quiz</button>;
}
```

#### Delete Quiz

```tsx
import { useDeleteQuiz } from '@/hooks/useQuizzes';

function DeleteQuizButton({ quizId }: { quizId: number }) {
  const deleteQuiz = useDeleteQuiz();

  const handleDelete = async () => {
    if (confirm('Are you sure?')) {
      try {
        await deleteQuiz.mutateAsync({
          id: quizId,
          currentUserId: 1,
        });
      } catch (error) {
        console.error('Failed to delete quiz:', error);
      }
    }
  };

  return (
    <button
      onClick={handleDelete}
      disabled={deleteQuiz.isPending}
    >
      Delete
    </button>
  );
}
```

### 3. Working with Users (Hardcoded)

```tsx
import { useUsers, useUser } from '@/hooks/useUsers';

function UserList() {
  const { data: users } = useUsers();

  return (
    <div>
      {users?.map(user => (
        <div key={user.id}>
          {user.username} (ID: {user.id})
        </div>
      ))}
    </div>
  );
}

function UserProfile({ userId }: { userId: number }) {
  const { data: user } = useUser(userId);

  return <div>Username: {user?.username}</div>;
}
```

### 4. Working with Game Records

```tsx
import { useGameRecords, useCreateGameRecord } from '@/hooks/useGameRecords';

function GameHistory() {
  const { data: records } = useGameRecords();

  return (
    <div>
      {records?.map(record => (
        <div key={record.id}>
          {record.quizName} - {new Date(record.datetimeCompleted).toLocaleDateString()}
        </div>
      ))}
    </div>
  );
}

function StartGameButton({ quizId }: { quizId: number }) {
  const createRecord = useCreateGameRecord();

  const handleStart = async () => {
    await createRecord.mutateAsync({
      hostUserId: 1,
      quizId,
    });
  };

  return <button onClick={handleStart}>Start Game</button>;
}
```

## API Endpoints Mapped

### Quiz Controller (`/api/Quiz`)

| Method | Endpoint | Hook | Description |
|--------|----------|------|-------------|
| GET | `/Quiz` | `useQuizzes()` | Get all quizzes |
| GET | `/Quiz/{id}` | `useQuiz(id)` | Get quiz by ID |
| GET | `/Quiz/user/{userId}` | `useQuizzesByUser(userId)` | Get user's quizzes |
| GET | `/Quiz/search?query={q}` | `useSearchQuizzes(query)` | Search quizzes |
| POST | `/Quiz?creatorUserId={id}` | `useCreateQuiz()` | Create quiz |
| PUT | `/Quiz/{id}?currentUserId={id}` | `useUpdateQuiz()` | Update quiz |
| DELETE | `/Quiz/{id}?currentUserId={id}` | `useDeleteQuiz()` | Delete quiz |

### User Controller (Hardcoded - `/api/User` not implemented yet)

| Method | Endpoint | Hook | Description |
|--------|----------|------|-------------|
| GET | `/User` | `useUsers()` | Get all users (mock) |
| GET | `/User/{id}` | `useUser(id)` | Get user by ID (mock) |
| GET | `/User/username/{username}` | `useUserByUsername(username)` | Get user by username (mock) |
| POST | `/User` | `useCreateUser()` | Create user (mock) |

### Game Record Controller (Placeholder - `/api/GameRecord` not fully implemented)

| Method | Endpoint | Hook | Description |
|--------|----------|------|-------------|
| GET | `/GameRecord` | `useGameRecords()` | Get all records (mock) |
| GET | `/GameRecord/{id}` | `useGameRecord(id)` | Get record by ID (mock) |
| GET | `/GameRecord/user/{userId}` | `useGameRecordsByUser(userId)` | Get user's records (mock) |
| POST | `/GameRecord` | `useCreateGameRecord()` | Create record (mock) |

## Mock Data

The following hardcoded data is available in `src/lib/mockData.ts`:

- **4 users**: alice_wonderland, bob_builder, charlie_chocolate, diana_prince
- **3 quizzes**: JavaScript Fundamentals (3 questions), React Basics (2 questions), CSS Grid & Flexbox (1 question)
- **2 game records**: Completed games for testing

## Switching to Real API

When your backend is ready:

1. Set `USE_MOCK = false` in each service file
2. Ensure your API is running on `http://localhost:3001`
3. Update `.env.development` if using a different port
4. Implement the UserController in your backend
5. Test each endpoint individually

## React Query Features

- **Automatic caching**: Data is cached and reused
- **Background refetching**: Stale data is refetched automatically
- **Optimistic updates**: UI updates before server confirms
- **Error handling**: Built-in error states
- **Loading states**: Built-in loading indicators
- **Automatic retries**: Failed requests retry automatically

## Example Component

See `src/components/QuizList.tsx` for a complete example using the hooks.

## Troubleshooting

### CORS Errors
If you see CORS errors, ensure your API has CORS configured:

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### Network Errors
Check that:
1. Your API is running on the correct port
2. The `VITE_API_URL` environment variable is correct
3. `USE_MOCK` is set appropriately in service files
