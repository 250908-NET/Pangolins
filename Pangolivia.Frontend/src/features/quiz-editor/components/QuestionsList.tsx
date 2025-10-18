import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Plus } from "lucide-react";

import { QuestionCard } from "./QuestionCard";
import type { Question } from "./types";

interface Props {
  questions: Question[];
  onAddQuestion: () => void;
  onDeleteQuestion: (questionId: string) => void;
  onUpdateQuestionText: (questionId: string, text: string) => void;
  onAddAnswer: (questionId: string) => void;
  onDeleteAnswer: (questionId: string, answerId: string) => void;
  onUpdateAnswerText: (questionId: string, answerId: string, text: string) => void;
  onToggleCorrect: (questionId: string, answerId: string) => void;
}

export function QuestionsList({
  questions,
  onAddQuestion,
  onDeleteQuestion,
  onUpdateQuestionText,
  onAddAnswer,
  onDeleteAnswer,
  onUpdateAnswerText,
  onToggleCorrect,
}: Props) {
  return (
    <div className="mb-6 space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold">Questions</h2>
        <Button onClick={onAddQuestion} size="sm">
          <Plus className="mr-2 h-4 w-4" />
          Add Question
        </Button>
      </div>

      {questions.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center">
            <p className="text-muted-foreground">
              {'No questions yet. Click "Add Question" to get started.'}
            </p>
          </CardContent>
        </Card>
      ) : (
        questions.map((question, qIndex) => (
          <QuestionCard
            key={question.id}
            question={question}
            index={qIndex}
            onDeleteQuestion={onDeleteQuestion}
            onUpdateQuestionText={onUpdateQuestionText}
            onAddAnswer={onAddAnswer}
            onDeleteAnswer={onDeleteAnswer}
            onUpdateAnswerText={onUpdateAnswerText}
            onToggleCorrect={onToggleCorrect}
          />
        ))
      )}
    </div>
  );
}
