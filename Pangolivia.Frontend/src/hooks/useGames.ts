import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { gameService } from '../services/gameService';
import { useAuth } from '@/hooks/useAuth';
import { toast } from 'sonner';

/**
 * Mutation hook to create a new game session.
 * Handles navigation to the lobby on success.
 */
export const useCreateGame = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  return useMutation({
    mutationFn: (quizId: number) => gameService.createGame(quizId),
    onSuccess: (data ) => {
      const { roomCode } = data;
      // Set the host as the current player for the lobby
      const hostPlayer = {
        id: user?.id,
        name: user?.name,
        isHost: true,
      };
      localStorage.setItem('currentPlayer', JSON.stringify(hostPlayer));

      toast.success(`Game created! Room code: ${roomCode}`);
      navigate(`/game-lobby?roomCode=${roomCode}`);
    },
    onError: (error: any) => {
      console.error('Failed to create game:', error);
      toast.error(error.response?.data?.message || 'Could not create the game. Please try again.');
    },
  });
};

/**
 * Mutation hook to join an existing game session.
 * Handles verifying the room code and navigating to the lobby on success.
 */
export const useJoinGame = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  return useMutation({
    mutationFn: async (roomCode: string) => {
      const gameDetails = await gameService.getGameDetailsByRoomCode(roomCode);
      console.info(gameDetails);
      return { roomCode, playerName: user!.name };
    },
    onSuccess: ({ roomCode, playerName }) => {
      // Set up local player data and navigate to the lobby
      const playerData = {
        id: user?.id,
        name: playerName,
        isHost: false,
      };
      localStorage.setItem('currentPlayer', JSON.stringify(playerData));
      navigate(`/game-lobby?roomCode=${roomCode.toUpperCase()}`);
    },
    onError: (err: any) => {
      console.error('Error joining game:', err);
      if (err.response?.status === 404) {
        toast.error('Game not found. Please check the Room Code.');
      } else {
        toast.error(err.response?.data?.message || 'Failed to join game. Please try again.');
      }
    },
  });
};