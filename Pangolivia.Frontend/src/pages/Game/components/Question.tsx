import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import type { QuestionForPlayer } from '@/contexts/SignalRContext';

interface QuestionProps {
  question: QuestionForPlayer;
  answerSubmitted: boolean;
  onSelectAnswer: (answer: string) => void;
}

export function Question({ question, answerSubmitted, onSelectAnswer }: QuestionProps) {
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
    if (answerSubmitted) return;
    setSelectedAnswer(answer);
    onSelectAnswer(answer);
  };

  const answers = [question.answer1, question.answer2, question.answer3, question.answer4];

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <div className="flex justify-between text-muted-foreground">
            <span>Question</span>
            <span className="font-bold text-lg">{timer}s</span>
          </div>
          <CardTitle className="text-2xl">{question.questionText}</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {answers.map((answer, index) => (
            <Button
              key={index}
              variant={selectedAnswer === answer ? 'default' : 'outline'}
              className="h-auto min-h-[4rem] whitespace-normal justify-start p-4 text-left"
              onClick={() => handleSelect(answer)}
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