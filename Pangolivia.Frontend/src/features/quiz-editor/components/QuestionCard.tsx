import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, Trash2 } from "lucide-react";

import { AnswerItem } from "./AnswerItem";
import type { Question } from "./types";

interface Props {
  question: Question;
  index: number;
  onDeleteQuestion: (questionId: string) => void;
  onUpdateQuestionText: (questionId: string, text: string) => void;
  onAddAnswer: (questionId: string) => void;
  onDeleteAnswer: (questionId: string, answerId: string) => void;
  onUpdateAnswerText: (questionId: string, answerId: string, text: string) => void;
  onToggleCorrect: (questionId: string, answerId: string) => void;
}

export function QuestionCard({
  question,
  index,
  onDeleteQuestion,
  onUpdateQuestionText,
  onAddAnswer,
  onDeleteAnswer,
  onUpdateAnswerText,
  onToggleCorrect,
}: Props) {
  return (
    <Card>
      <CardHeader>
        <div className="flex items-start justify-between">
          <CardTitle className="text-lg">Question {index + 1}</CardTitle>
          <Button variant="outline" size="sm" onClick={() => onDeleteQuestion(question.id)}>
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor={`question-${question.id}`}>Question Text</Label>
          <Textarea
            id={`question-${question.id}`}
            placeholder="Enter your question..."
            value={question.text}
            onChange={(e) => onUpdateQuestionText(question.id, e.target.value)}
            rows={3}
          />
        </div>

        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <Label>Answers ({question.answers.length}/4)</Label>
            <Button
              variant="outline"
              size="sm"
              onClick={() => onAddAnswer(question.id)}
              disabled={question.answers.length >= 4}
            >
              <Plus className="mr-2 h-4 w-4" />
              Add Answer
            </Button>
          </div>

          {question.answers.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No answers yet. Add exactly 4 answers with one correct.
            </p>
          ) : (
            question.answers.map((answer, aIndex) => (
              <AnswerItem
                key={answer.id}
                answer={answer}
                index={aIndex}
                onChangeText={(text) => onUpdateAnswerText(question.id, answer.id, text)}
                onToggleCorrect={() => onToggleCorrect(question.id, answer.id)}
                onDelete={() => onDeleteAnswer(question.id, answer.id)}
              />
            ))
          )}
        </div>
      </CardContent>
    </Card>
  );
}
