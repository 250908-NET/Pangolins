import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface Props {
  isEditMode: boolean;
  gameName: string;
  onGameNameChange: (value: string) => void;
  creatorUsername?: string;
}

export function QuizDetailsCard({
  isEditMode,
  gameName,
  onGameNameChange,
  creatorUsername,
}: Props) {
  return (
    <Card className="mb-6">
      <CardHeader>
        <CardTitle>{isEditMode ? "Quiz Details" : "Game Details"}</CardTitle>
        <CardDescription>
          {isEditMode ? `Created by: ${creatorUsername || "Unknown"}` : "Set up your game information"}
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="gameName">{isEditMode ? "Quiz Name" : "Game Name"}</Label>
          <Input
            id="gameName"
            placeholder="Enter game name..."
            value={gameName}
            onChange={(e) => onGameNameChange(e.target.value)}
          />
        </div>
      </CardContent>
    </Card>
  );
}
