import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { FastForward } from 'lucide-react';
import type { QuestionForPlayer } from '@/contexts/SignalRContext';

interface QuestionProps {
  question: QuestionForPlayer;
  answerSubmitted: boolean;
  onSelectAnswer: (answer: string) => void;
  isHost: boolean;
  onSkipQuestion: () => void;
}

export function Question({
  question,
  answerSubmitted,
  onSelectAnswer,
  isHost,
  onSkipQuestion,
}: QuestionProps) {
  const [selectedAnswer, setSelectedAnswer] = useState<string | null>(null);
  const [timer, setTimer] = useState(10); // Or get this from the server

  useEffect(() => {
    setSelectedAnswer(null);
    setTimer(10);
  }, [question]);

  useEffect(() => {
    if (timer > 0) {
      const interval = setInterval(() => {
        setTimer((prev) => (prev > 0 ? prev - 1 : 0));
      }, 1000);
      return () => clearInterval(interval);
    }
  }, [timer]);

  const handleSelect = (answer: string) => {
    if (answerSubmitted || isHost) return;
    setSelectedAnswer(answer);
    onSelectAnswer(answer);
  };

  const answers = [
    question.answer1,
    question.answer2,
    question.answer3,
    question.answer4,
  ];

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <div className="flex justify-between text-muted-foreground">
            <span>Question</span>
            <span className="font-bold text-lg" aria-live="polite" aria-atomic="true">
              {timer}s
              <span className="sr-only"> seconds remaining</span>
            </span>
          </div>
          <CardTitle className="text-2xl">{question.questionText}</CardTitle>
        </CardHeader>
        <CardContent role="radiogroup" aria-label="Answer options" className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {answers.map((answer, index) => (
            <Button
              key={index}
              role="radio"
              aria-checked={selectedAnswer === answer}
              variant={selectedAnswer === answer ? 'default' : 'outline'}
              className="h-auto min-h-[4rem] whitespace-normal justify-start p-4 text-left"
              onClick={() => handleSelect(answer)}
              disabled={answerSubmitted || isHost}
              aria-label={`Answer option: ${answer}`}
            >
              {answer}
            </Button>
          ))}
        </CardContent>
        {answerSubmitted && (
          <div aria-live="polite" className="sr-only">
            Answer submitted successfully
          </div>
        )}
        {isHost && (
          <CardFooter className="flex justify-end">
            <Button variant="secondary" onClick={onSkipQuestion}>
              <FastForward className="mr-2 h-4 w-4" aria-hidden="true" />
              Skip Question
            </Button>
          </CardFooter>
        )}
      </Card>
    </section>
  );
}