import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Loader2, Sparkles } from "lucide-react";

interface Props {
  topic: string;
  count: number;
  difficulty: "easy" | "medium" | "hard";
  isGenerating: boolean;
  onTopicChange: (value: string) => void;
  onCountChange: (value: number) => void;
  onDifficultyChange: (value: "easy" | "medium" | "hard") => void;
  onGenerate: () => void;
}

export function AiGeneratorCard({
  topic,
  count,
  difficulty,
  isGenerating,
  onTopicChange,
  onCountChange,
  onDifficultyChange,
  onGenerate,
}: Props) {
  return (
    <Card className="mb-6">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Sparkles className="h-5 w-5" />
          AI Question Generator
        </CardTitle>
        <CardDescription>
          Generate quiz questions automatically using AI
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
          <div className="space-y-2">
            <Label htmlFor="aiTopic">Topic</Label>
            <Input
              id="aiTopic"
              placeholder="e.g., World Geography"
              value={topic}
              onChange={(e) => onTopicChange(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="aiCount">Number of Questions</Label>
            <Input
              id="aiCount"
              placeholder="e.g., 5"
              type="number"
              min={1}
              max={50}
              value={count}
              onChange={(e) => onCountChange(Number(e.target.value))}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="aiDifficulty">Difficulty</Label>
            <select
              id="aiDifficulty"
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              value={difficulty}
              onChange={(e) =>
                onDifficultyChange(e.target.value as "easy" | "medium" | "hard")
              }
            >
              <option value="easy">Easy</option>
              <option value="medium">Medium</option>
              <option value="hard">Hard</option>
            </select>
          </div>
        </div>
        <div className="flex justify-end">
          <Button
            onClick={onGenerate}
            disabled={isGenerating || topic.trim().length === 0}
            variant="secondary"
          >
            {isGenerating && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {isGenerating ? "Generating..." : "Generate with AI"}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
