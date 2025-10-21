import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

interface Props {
  isEditMode: boolean;
  onBack?: () => void;
}

export function QuizHeader({ isEditMode, onBack }: Props) {
  return (
    <div className="mb-8">
      {isEditMode && (
        <Button variant="ghost" onClick={onBack} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to Games List
        </Button>
      )}
      <h1 className="mb-2 text-3xl font-bold">{isEditMode ? "Edit Quiz" : "Create New Game"}</h1>
      <p className="text-muted-foreground">
        {isEditMode ? "Modify questions and answers for your quiz" : "Build your custom trivia game with questions and answers"}
      </p>
    </div>
  );
}
