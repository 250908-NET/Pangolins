# 🎮 Pangolivia

> A real-time multiplayer quiz platform built with .NET and React

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Local Development](#local-development)
  - [Docker Development](#docker-development)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [Team](#team)
- [License](#license)

## 🎯 Overview

Pangolivia is a modern, real-time multiplayer quiz platform that enables users to create, host, and participate in interactive trivia games. Built with cutting-edge technologies, it provides a seamless gaming experience with live updates, comprehensive statistics, and AI-powered quiz generation.

### Key Highlights

- **Real-time Gameplay**: Powered by SignalR WebSockets for instant synchronization
- **Custom Quiz Creation**: Build your own quizzes with multiple-choice questions
- **AI Integration**: Generate quizzes automatically using OpenAI
- **Comprehensive Analytics**: Track performance, view leaderboards, and game history
- **Secure Authentication**: JWT-based auth with protected routes
- **Cloud Deployment**: Fully containerized and deployed on Azure

## ✨ Features

### For Players

- 🎲 Join games using simple room codes
- ⚡ Real-time question display and answer submission
- 🏆 Live leaderboards and scoring
- 📊 Personal statistics and game history
- 👤 User profiles with performance tracking

### For Hosts

- ✏️ Create and edit custom quizzes
- 🤖 AI-powered quiz generation
- 🎮 Host live game sessions
- 👥 Manage player lobbies
- ⏭️ Control game flow (skip questions, timing)

### Technical Features

- 🔐 Secure JWT authentication
- 🔄 Real-time state synchronization
- 📱 Responsive design (mobile-friendly)
- 🎨 Modern UI with TailwindCSS and shadcn/ui
- 🚀 Optimized performance with React Query caching
- 🐳 Docker containerization
- ☁️ CI/CD pipeline with GitHub Actions

## 🛠️ Tech Stack

### Frontend

| Technology     | Version | Purpose                 |
| -------------- | ------- | ----------------------- |
| React          | 19.1    | UI Framework            |
| TypeScript     | 5.9     | Type Safety             |
| Vite           | 7.1     | Build Tool              |
| TailwindCSS    | 4.1     | Styling                 |
| shadcn/ui      | Latest  | Component Library       |
| React Query    | 5.90    | Server State Management |
| React Router   | 7.1     | Routing                 |
| SignalR Client | 9.0     | WebSocket Communication |
| Axios          | 1.12    | HTTP Client             |
| Motion         | 12.23   | Animations              |

### Backend

| Technology            | Version | Purpose                 |
| --------------------- | ------- | ----------------------- |
| .NET                  | 9.0     | Framework               |
| ASP.NET Core          | 9.0     | Web API                 |
| Entity Framework Core | 9.0     | ORM                     |
| SignalR               | 9.0     | Real-time Communication |
| SQL Server            | Latest  | Database                |
| AutoMapper            | 12.0    | Object Mapping          |
| BCrypt.Net            | 4.0     | Password Hashing        |
| JWT Bearer            | 9.0     | Authentication          |
| Serilog               | 4.3     | Logging                 |

### DevOps & Tools

- **Containerization**: Docker
- **CI/CD**: GitHub Actions
- **Cloud Platform**: Microsoft Azure
  - Azure App Service
  - Azure SQL Database
  - Azure Container Registry
- **Testing**: xUnit
- **Version Control**: Git/GitHub

## 🏗️ Architecture

### System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  React SPA (Vite + TypeScript)                       │  │
│  │  - Pages & Components                                │  │
│  │  - React Query (State Management)                    │  │
│  │  - SignalR Client (WebSocket)                        │  │
│  │  - Axios (HTTP Client)                               │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            │
                    HTTP/HTTPS │ WebSocket
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Backend API                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  ASP.NET Core Web API                                │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │  Controllers (REST Endpoints)                  │  │  │
│  │  │  - Auth, Quiz, Game, User, GameRecord          │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │  SignalR Hub (Real-time)                       │  │  │
│  │  │  - GameHub (Live game orchestration)           │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │  Services (Business Logic)                     │  │  │
│  │  │  - Auth, Quiz, GameManager, AI                 │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │  Repositories (Data Access)                    │  │  │
│  │  │  - User, Quiz, Question, GameRecord            │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Database Layer                           │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  SQL Server (Azure SQL)                              │  │
│  │  - Users, Quizzes, Questions                         │  │
│  │  - GameRecords, PlayerGameRecords                    │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Database Schema (ERD)

```
┌─────────────────┐
│     Users       │
├─────────────────┤
│ Id (PK)         │
│ AuthUuid        │
│ Username        │
│ PasswordHash    │
└─────────────────┘
        │
        │ 1:N (CreatedBy)
        ▼
┌─────────────────┐       1:N        ┌─────────────────┐
│    Quizzes      │◄─────────────────│   Questions     │
├─────────────────┤                  ├─────────────────┤
│ Id (PK)         │                  │ Id (PK)         │
│ QuizName        │                  │ QuizId (FK)     │
│ CreatedByUserId │                  │ QuestionText    │
└─────────────────┘                  │ CorrectAnswer   │
        │                            │ Answer2-4       │
        │ 1:N                        └─────────────────┘
        ▼
┌─────────────────┐
│  GameRecords    │
├─────────────────┤
│ Id (PK)         │
│ HostUserId (FK) │
│ QuizId (FK)     │
│ DateCompleted   │
└─────────────────┘
        │
        │ 1:N
        ▼
┌──────────────────────┐
│ PlayerGameRecords    │
├──────────────────────┤
│ Id (PK)              │
│ UserId (FK)          │
│ GameRecordId (FK)    │
│ Score                │
└──────────────────────┘
```

## 🚀 Getting Started

### Prerequisites

- **Node.js** 20+ and npm
- **.NET SDK** 9.0+
- **SQL Server** (LocalDB, Express, or Azure SQL)
- **Docker** (optional, for containerized development)
- **Git**

### Local Development

#### 1. Clone the Repository

```bash
git clone https://github.com/250908-NET/Pangolins.git
cd Pangolins
```

#### 2. Backend Setup

```bash
cd Pangolivia.API

# Restore dependencies
dotnet restore

# Set up environment variables
cp .env.example .env
# Edit .env with your configuration:
# - ConnectionStrings__Connection (SQL Server)
# - Jwt__Key, Jwt__Issuer, Jwt__Audience
# - OPENAI_API_KEY (optional, for AI features)

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`)

#### 3. Frontend Setup

```bash
cd Pangolivia.Frontend

# Install dependencies
npm install

# Set up environment variables
cp .env.development.example .env.development
# Edit .env.development:
# VITE_API_URL=http://localhost:5000

# Run the development server
npm run dev
```

The frontend will be available at `http://localhost:5173`

### Docker Development

For a complete local environment with Docker:

```bash
cd scripts/local-docker

# Start all services (API, Frontend, SQL Server)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Database Migrations

We provide a helper script for EF Core migrations:

```bash
# Add a new migration
./scripts/ef.sh add MigrationName -e Development

# Update database to latest migration
./scripts/ef.sh update -e Development

# List all migrations
./scripts/ef.sh list

# Generate SQL script
./scripts/ef.sh script -i -e Production

# Remove last migration (if not applied)
./scripts/ef.sh remove

# Drop database
./scripts/ef.sh drop
```

See `scripts/README.md` for more options.

## 📁 Project Structure

### Backend (`Pangolivia.API`)

```
Pangolivia.API/
├── Api/
│   ├── Controllers/          # REST API endpoints
│   │   ├── AuthController.cs
│   │   ├── QuizController.cs
│   │   ├── GameRecordController.cs
│   │   └── ...
│   ├── Services/             # Business logic layer
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── Repositories/         # Data access layer
│   ├── Models/               # Database entities
│   ├── DTOs/                 # Data transfer objects
│   ├── Hubs/                 # SignalR hubs
│   │   └── GameHub.cs
│   └── Data/                 # DbContext & migrations
├── GameEngine/               # Game logic
│   ├── GameSession.cs
│   └── Player.cs
├── Middleware/
├── Program.cs                # Application entry point
└── Pangolivia.API.csproj
```

### Frontend (`Pangolivia.Frontend`)

```
Pangolivia.Frontend/
├── src/
│   ├── components/           # Reusable UI components
│   │   ├── ui/              # shadcn/ui components
│   │   ├── header.tsx
│   │   ├── QuizList.tsx
│   │   └── ...
│   ├── pages/                # Route-level pages
│   │   ├── Home.tsx
│   │   ├── Login.tsx
│   │   ├── QuizEditor.tsx
│   │   ├── Game/            # Game-related pages
│   │   └── ...
│   ├── features/             # Feature modules
│   │   └── quiz-editor/
│   ├── services/             # API service layer
│   │   ├── authService.ts
│   │   ├── quizService.ts
│   │   └── ...
│   ├── hooks/                # Custom React hooks
│   │   ├── useAuth.ts
│   │   ├── useQuizzes.ts
│   │   └── ...
│   ├── contexts/             # React Context providers
│   │   ├── AuthContext.tsx
│   │   └── SignalRContext.tsx
│   ├── types/                # TypeScript type definitions
│   ├── lib/                  # Utilities
│   ├── App.tsx
│   └── main.tsx
├── public/
├── package.json
└── vite.config.ts
```

### Tests (`Pangolivia.Tests`)

```
Pangolivia.Tests/
├── Controllers/              # Controller tests
├── Services/                 # Service layer tests
├── Repositories/             # Repository tests
├── GameEngine/               # Game logic tests
└── Pangolivia.Tests.csproj
```

## 📚 API Documentation

### Authentication Endpoints

```http
POST /api/auth/register
POST /api/auth/login
```

### Quiz Endpoints

```http
GET    /api/quiz              # Get all quizzes
GET    /api/quiz/{id}         # Get quiz by ID
POST   /api/quiz              # Create quiz
PUT    /api/quiz/{id}         # Update quiz
DELETE /api/quiz/{id}         # Delete quiz
POST   /api/quiz/ai-generate  # Generate quiz with AI
```

### Game Endpoints

```http
POST   /api/game/create       # Create game session
GET    /api/game/{roomCode}   # Get game details
```

### Game Record Endpoints

```http
GET    /api/gamerecord                    # Get all game records
GET    /api/gamerecord/{id}               # Get game record by ID
GET    /api/gamerecord/{id}/leaderboard   # Get game leaderboard
```

### User Endpoints

```http
GET    /api/user/{id}         # Get user profile
GET    /api/user/{id}/detail  # Get detailed user stats
```

### SignalR Hub Events

**Client → Server:**

- `JoinGame(roomCode)` - Join a game lobby
- `StartGame(roomCode)` - Start the game (host only)
- `BeginGame(roomCode)` - Begin game loop (host only)
- `SubmitAnswer(roomCode, answer)` - Submit answer
- `SkipQuestion(roomCode)` - Skip current question (host only)

**Server → Client:**

- `ReceiveLobbyDetails(details)` - Lobby information
- `UpdatePlayerList(players)` - Updated player list
- `ShowQuestion(question)` - Display question
- `ShowRoundResults(results)` - Round results
- `GameEnded(finalResults)` - Game completion
- `Error(message)` - Error notification

For detailed API documentation, run the API and visit `/swagger` in development mode.

## 🧪 Testing

### Backend Tests

```bash
cd Pangolivia.Tests

# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "FullyQualifiedName~GameSessionTests"
```

### Frontend Tests

```bash
cd Pangolivia.Frontend

# Lint check
npm run lint

# Type check
npm run build
```

## 🚢 Deployment

### CI/CD Pipeline

The project uses GitHub Actions for automated deployment to Azure:

- **API Pipeline**: `.github/workflows/deploy-api.yaml`
- **Frontend Pipeline**: `.github/workflows/deploy-frontend.yaml`

#### Deployment Flow

1. **Trigger**: Push to `main` branch or manual workflow dispatch
2. **Build**: Docker images built for API and Frontend
3. **Push**: Images pushed to Azure Container Registry
4. **Deploy**: Images deployed to Azure App Service

### Manual Docker Deployment

#### Backend

```bash
cd Pangolivia.API

# Build image
docker build -t pangolivia-api .

# Run container
docker run -p 8080:80 \
  -e ConnectionStrings__Connection="your-connection-string" \
  -e Jwt__Key="your-secret-key" \
  -e Jwt__Issuer="your-issuer" \
  -e Jwt__Audience="your-audience" \
  pangolivia-api
```

#### Frontend

```bash
cd Pangolivia.Frontend

# Build image
docker build -t pangolivia-frontend \
  --build-arg VITE_API_URL=https://your-api-url.com \
  .

# Run container
docker run -p 8080:80 pangolivia-frontend
```

### Azure Deployment Setup

#### Prerequisites

1. **Azure Resources**:

   - Azure App Service (2 instances: API & Frontend)
   - Azure SQL Database
   - Azure Container Registry

2. **GitHub Secrets** (required for CI/CD):
   - `ACR_LOGIN_SERVER` - Container registry URL
   - `ACR_USERNAME` - Service principal client ID
   - `ACR_PASSWORD` - Service principal secret
   - `API_PUBLISH_PROFILE` - API App Service publish profile
   - `FRONTEND_PUBLISH_PROFILE` - Frontend App Service publish profile

#### Service Principal Setup

Follow the [Microsoft guide](https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal) to create a service principal:

1. Register an application in Azure AD
2. Use the **Application (client) ID** for credentials
3. Grant the service principal `AcrPush` and `AcrPull` roles in Container Registry

#### Environment Variables (Azure App Service)

**API Configuration**:

```
CONNECTIONSTRINGS__CONNECTION=<Azure SQL connection string>
JWT__KEY=<your-secret-key>
JWT__ISSUER=<your-issuer>
JWT__AUDIENCE=<your-audience>
OPENAI_API_KEY=<your-openai-key> (optional)
```

**Frontend Configuration**:

```
VITE_API_URL=<your-api-url>
```

## 🤝 Contributing

### Development Workflow

1. **Create a feature branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following the coding standards:

   - Follow existing code style and patterns
   - Write meaningful commit messages
   - Add tests for new features
   - Update documentation as needed

3. **Test your changes**:

   ```bash
   # Backend
   cd Pangolivia.Tests && dotnet test

   # Frontend
   cd Pangolivia.Frontend && npm run lint && npm run build
   ```

4. **Push and create a Pull Request**:
   ```bash
   git push origin feature/your-feature-name
   ```

### Coding Standards

- **C#**: Follow Microsoft's C# coding conventions
- **TypeScript/React**: Use ESLint configuration provided
- **Commits**: Use conventional commit messages
- **Branches**: Use prefixes: `feature/`, `fix/`, `docs/`, `refactor/`

## 👥 Team

**Team Pangolins** - Revature .NET Training Cohort

- Christian Brewer
- Husankhuja Nizomkhujaev
- Ledya Bakloug
- Matthew Schade
- Danny Scally
- Khoa Diep
- Kendell Rennie
- Charles Trangay
- Victor Torres
- Nizar El Hilali
- Tevin Johnson
- Abdelmajid Samir

## 🔗 Links

- **Live Demo**: [Pangolivia Frontend](https://pangolivia-frontend-gjhpf7gphvhmhgbm.canadacentral-01.azurewebsites.net)
- **API Endpoint**: [Pangolivia API](https://pangolivia-api-bpdehvd0b4bmcwhj.canadacentral-01.azurewebsites.net)
- **Repository**: [GitHub - Pangolins](https://github.com/250908-NET/Pangolins)

---

**Built by Team Pangolins**
