import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface Props {
  isEditMode: boolean;
  gameName: string;
  hostName: string;
  onGameNameChange: (value: string) => void;
  onHostNameChange: (value: string) => void;
  creatorUsername?: string;
}

export function QuizDetailsCard({
  isEditMode,
  gameName,
  hostName,
  onGameNameChange,
  onHostNameChange,
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
        {!isEditMode && (
          <div className="space-y-2">
            <Label htmlFor="hostName">Your Name (Host)</Label>
            <Input
              id="hostName"
              placeholder="Enter your name..."
              value={hostName}
              onChange={(e) => onHostNameChange(e.target.value)}
            />
          </div>
        )}
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
