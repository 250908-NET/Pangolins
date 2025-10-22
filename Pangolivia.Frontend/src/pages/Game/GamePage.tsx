import { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { useSignalR } from '@/hooks/useSignalR';
import { useAuth } from '@/hooks/useAuth';
import { toast } from 'sonner';
import { gameRecordKeys } from '@/hooks/useGameRecords';
import { playerGameRecordKeys } from '@/hooks/usePlayerGameRecords';
import { userKeys } from '@/hooks/useUsers';

import { Lobby } from '@/pages/Game/components/Lobby';
import { Question } from '@/pages/Game/components/Question';
import { RoundResults } from '@/pages/Game/components/RoundResults';
import { FinalResults } from '@/pages/Game/components/FinalResults';
import { Waiting } from '@/pages/Game/components/Waiting';

export default function GamePage() {
  const { roomCode } = useParams<{ roomCode: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const isHost = JSON.parse(localStorage.getItem('currentPlayer') || '{}').isHost;

  const {
    connection,
    connectToHub,
    disconnect,
    joinLobby,
    startLobbyGame,
    submitPlayerAnswer,
    beginGameLoop,
    lobbyDetails,
    players,
    gameStarted,
    currentQuestion,
    roundResults,
    finalResults,
    answerSubmitted,
    skipCurrentQuestion,
  } = useSignalR();

  // --- Connection and Lobby Management ---
  useEffect(() => {
    if (!roomCode) {
      toast.error('No room code provided. Redirecting...');
      navigate('/join-game');
      return;
    }

    if (!connection) {
      connectToHub('/gamehub');
    }

    return () => {
      // Disconnect when the user navigates away from the game entirely
      disconnect();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Run only on mount and unmount

  useEffect(() => {
    if (connection && roomCode) {
      joinLobby(roomCode).catch((err) => {
        toast.error(`Failed to join room: ${err.message}`);
        navigate('/join-game');
      });
    }
  }, [connection, roomCode, joinLobby, navigate]);
  
  // --- Game Start Trigger ---
  useEffect(() => {
    if (gameStarted && isHost && roomCode) {
      const timerId = setTimeout(() => {
        beginGameLoop(roomCode).catch(err => {
          toast.error(`Failed to start game: ${err.message}`);
        });
      }, 1500); // Small delay to let clients process "game started"
      return () => clearTimeout(timerId);
    }
  }, [gameStarted, isHost, roomCode, beginGameLoop]);

  // --- Invalidate queries when game ends ---
  useEffect(() => {
    if (finalResults && user?.id) {
      // Invalidate all game-related queries when the game ends
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.all });
      queryClient.invalidateQueries({ queryKey: playerGameRecordKeys.all });
      queryClient.invalidateQueries({ queryKey: userKeys.detail(user.id) });
    }
  }, [finalResults, user?.id, queryClient]);


  // --- Event Handlers passed to components ---
  const handleStartGame = () => {
    if (roomCode && isHost) {
      startLobbyGame(roomCode).catch(err => toast.error(`Failed to start: ${err.message}`));
    }
  };

  const handleLeaveGame = async () => {
    localStorage.removeItem('currentPlayer');
    await disconnect(); // This now also resets state via the context
    
    // Invalidate all game-related queries to refresh data on Profile page
    if (user?.id) {
      queryClient.invalidateQueries({ queryKey: gameRecordKeys.all });
      queryClient.invalidateQueries({ queryKey: playerGameRecordKeys.all });
      queryClient.invalidateQueries({ queryKey: userKeys.detail(user.id) });
    }
    
    navigate('/');
  };

  const handleSelectAnswer = (answer: string) => {
    if (!roomCode) return;
    submitPlayerAnswer(roomCode, answer)
      .then(() => toast.success('Answer submitted!'))
      .catch(err => toast.error(`Submit failed: ${err.message}`));
  };

  const handleSkipQuestion = () => {
    if (!roomCode || !isHost) return;
    skipCurrentQuestion(roomCode).catch((err) =>
      toast.error(`Failed to skip: ${err.message}`),
    );
  };

  // --- Render Logic (State Machine) ---
  if (!connection) {
    return <Waiting message="Connecting to game server..." />;
  }

  if (finalResults) {
    return <FinalResults results={finalResults} onExitGame={handleLeaveGame} />;
  }

  if (roundResults) {
    return <RoundResults results={roundResults} currentUserId={user?.id} isHost={isHost} />;
  }
  
  if (currentQuestion) {
    return (
      <Question
        question={currentQuestion}
        answerSubmitted={answerSubmitted}
        onSelectAnswer={handleSelectAnswer}
        isHost={isHost}
        onSkipQuestion={handleSkipQuestion}
      />
    );
  }

  if (gameStarted) {
    return <Waiting message="The game is starting..." />;
  }

  if (lobbyDetails && players.length > 0) {
    return (
      <Lobby
        roomCode={roomCode!}
        lobbyDetails={lobbyDetails}
        players={players}
        isHost={isHost}
        onStartGame={handleStartGame}
        onLeaveGame={handleLeaveGame}
      />
    );
  }

  return <Waiting message="Joining lobby..." />;
}