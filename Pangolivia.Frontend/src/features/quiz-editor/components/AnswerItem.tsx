import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Trash2 } from "lucide-react";

import type { Answer } from "./types";

interface Props {
  answer: Answer;
  index: number;
  onChangeText: (text: string) => void;
  onToggleCorrect: () => void;
  onDelete: () => void;
}

export function AnswerItem({ answer, index, onChangeText, onToggleCorrect, onDelete }: Props) {
  return (
    <div className="flex items-start gap-2 rounded-lg border p-3">
      <div className="flex-1 space-y-2">
        <Input
          placeholder={`Answer ${index + 1}...`}
          value={answer.text}
          onChange={(e) => onChangeText(e.target.value)}
        />
        <label className="flex items-center gap-2 text-sm">
          <input
            type="checkbox"
            checked={answer.isCorrect}
            onChange={onToggleCorrect}
            className="h-4 w-4 rounded border-gray-300"
          />
          <span className={answer.isCorrect ? "font-semibold text-green-600" : ""}>
            Correct Answer
          </span>
        </label>
      </div>
      <Button variant="outline" size="sm" onClick={onDelete}>
        <Trash2 className="h-4 w-4" />
      </Button>
    </div>
  );
}
