import { useState, useEffect, useMemo } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useSignalR } from '@/hooks/useSignalR';
import { useAuth } from '@/hooks/useAuth';
import { toast } from 'sonner';
import { Loader2, CheckCircle, XCircle, Trophy } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

export default function GameActivePage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const roomCode = searchParams.get('roomCode');
  const { user } = useAuth();

  // --- Consume all game state and actions from the centralized hook ---
  const {
    connection,
    currentQuestion,
    roundResults,
    finalResults,
    answerSubmitted,
    submitPlayerAnswer,
    beginGameLoop,
  } = useSignalR();

  // State managed purely by the client for UI purposes
  const [selectedAnswer, setSelectedAnswer] = useState<string | null>(null);
  const [timer, setTimer] = useState(0);

  // --- Effect for the HOST to kick off the game loop ---
  useEffect(() => {
    if (!connection || !roomCode) return;

    const currentPlayer = JSON.parse(localStorage.getItem('currentPlayer') || '{}');
    if (currentPlayer.isHost) {
      const timerId = setTimeout(() => {
        beginGameLoop(roomCode).catch(err => {
          toast.error(`Failed to start game: ${err.message}`);
        });
      }, 1500);
      return () => clearTimeout(timerId);
    }
  }, [connection, roomCode, beginGameLoop]);
  
  // --- Effect to handle the countdown timer ---
  useEffect(() => {
    // When a new question arrives, we reset the timer.
    if (currentQuestion) {
      setTimer(10); // Assuming a 10-second timer; you could pass this from the server
    }
  }, [currentQuestion]);
  
  useEffect(() => {
    if (timer > 0) {
      const interval = setInterval(() => {
        setTimer((prev) => (prev > 0 ? prev - 1 : 0));
      }, 1000);
      return () => clearInterval(interval);
    }
  }, [timer]);


  // --- UI HANDLERS ---
  const handleSelectAnswer = (answer: string) => {
    if (answerSubmitted || !connection || !roomCode) return;
    setSelectedAnswer(answer);
    // Call the action from the context
    submitPlayerAnswer(roomCode, answer)
      .then(() => {
        toast.success("Answer submitted!");
      })
      .catch(err => {
        toast.error(`Failed to submit answer: ${err.message}`);
      });
  };

  const handleExitGame = () => {
    navigate('/');
  };

  // --- DERIVED DATA ---
  const finalLeaderboard = useMemo(() => {
    if (!finalResults) return [];
    return [...finalResults.playerScores].sort((a, b) => b.score - a.score);
  }, [finalResults]);

  // --- RENDER LOGIC ---
  
  // Loading View
  if (!connection) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center space-y-2">
        <Loader2 className="h-8 w-8 animate-spin" />
        <p>Connecting to game...</p>
      </div>
    );
  }

  // Final Results View (rendered if `finalResults` has data)
  if (finalResults) {
    return (
      <section className="flex min-h-screen items-center justify-center px-4 py-16">
        <Card className="w-full max-w-2xl">
          <CardHeader className="text-center">
             <Trophy className="mx-auto h-12 w-12 text-yellow-500" />
            <CardTitle className="text-3xl">Game Over!</CardTitle>
            <CardDescription>Here are the final results.</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {finalLeaderboard.map((player, index) => (
                <div key={player.userId} className="flex justify-between rounded-lg border p-3">
                  <p className="font-bold">{index + 1}. {player.username}</p>
                  <p>{player.score} Points</p>
                </div>
              ))}
            </div>
            <Button onClick={handleExitGame} className="mt-6 w-full">Back to Home</Button>
          </CardContent>
        </Card>
      </section>
    );
  }

  // Round Results View (rendered if `roundResults` has data)
  if (roundResults) {
     const myScoreThisRound = roundResults.playerScores.find(p => p.userId === user?.id)?.score ?? 0;
    return (
      <section className="flex min-h-screen items-center justify-center px-4 py-16">
        <Card className="w-full max-w-2xl text-center">
          <CardHeader>
            {myScoreThisRound > 0 ? (
                <CheckCircle className="mx-auto h-12 w-12 text-green-500" />
            ) : (
                <XCircle className="mx-auto h-12 w-12 text-red-500" />
            )}
            <CardTitle className="text-3xl">{myScoreThisRound > 0 ? "Correct!" : "Incorrect!"}</CardTitle>
            <CardDescription>The correct answer was:</CardDescription>
            <p className="text-xl font-bold">{roundResults.answer}</p>
          </CardHeader>
          <CardContent>
            <p className="font-semibold">Waiting for the next question...</p>
          </CardContent>
        </Card>
      </section>
    );
  }

  // Question View (rendered if `currentQuestion` has data)
  if (currentQuestion) {
    const answers = [currentQuestion.answer1, currentQuestion.answer2, currentQuestion.answer3, currentQuestion.answer4];
    return (
       <section className="flex min-h-screen items-center justify-center px-4 py-16">
          <Card className="w-full max-w-2xl">
            <CardHeader>
                <div className="flex justify-between text-muted-foreground">
                    <span>Question</span>
                    <span className="font-bold text-lg">{timer}s</span>
                </div>
                <CardTitle className="text-2xl">{currentQuestion.questionText}</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 gap-3 md:grid-cols-2">
                {answers.map((answer, index) => (
                    <Button
                        key={index}
                        variant={selectedAnswer === answer ? 'default' : 'outline'}
                        className="h-auto min-h-[4rem] whitespace-normal justify-start p-4 text-left"
                        onClick={() => handleSelectAnswer(answer)}
                        disabled={answerSubmitted}
                    >
                        {answer}
                    </Button>
                ))}
            </CardContent>
          </Card>
       </section>
    );
  }
  
  // Waiting View (initial state before first question arrives)
  return (
    <div className="flex min-h-screen flex-col items-center justify-center space-y-2">
      <Loader2 className="h-8 w-8 animate-spin" />
      <p>Waiting for the game to start...</p>
    </div>
  );
}