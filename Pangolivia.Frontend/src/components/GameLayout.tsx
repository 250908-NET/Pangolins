import { useEffect } from 'react';
import { Outlet, useNavigate, useSearchParams } from 'react-router-dom';
import { useSignalR } from '@/hooks/useSignalR';
import { toast } from 'sonner';
import { Loader2 } from 'lucide-react';

export function GameLayout() {
  const { connection, connectToHub, disconnect } = useSignalR();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const roomCode = searchParams.get('roomCode');

  useEffect(() => {
    if (!roomCode) {
      toast.error('No room code provided. Redirecting...');
      navigate('/join-game');
      return;
    }

    if (!connection) {
      connectToHub('/gamehub');
    }
  }, [connectToHub, navigate, roomCode, connection]);

  useEffect(() => {
    // This cleanup function should ONLY run when the GameLayout unmounts,
    // for instance when the user navigates away from the lobby or active game.
    // By providing an empty dependency array, we ensure this effect runs once on mount
    // and its cleanup runs once on unmount, preventing the premature disconnection.
    return () => {
      disconnect();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!connection) {
    return (
      <div className="flex h-screen items-center justify-center">
        <Loader2 className="mr-2 h-8 w-8 animate-spin" />
        Connecting to game server...
      </div>
    );
  }

  return <Outlet />;
}