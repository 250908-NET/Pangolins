import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Plus, Trash2, Loader2, ArrowLeft } from "lucide-react";
import {
  useCreateQuiz,
  useQuiz,
  useUpdateQuiz,
} from "@/hooks/useQuizzes";
import type { QuestionDto } from "@/types/api";
import { toast } from "sonner";

interface Answer {
  id: string;
  text: string;
  isCorrect: boolean;
}

interface Question {
  id: string;
  text: string;
  answers: Answer[];
}

const CURRENT_USER_ID = 1;

export default function QuizEditorPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const quizId = searchParams.get("id");
  const isEditMode = !!quizId;

  const { data: existingQuiz, isLoading: loadingQuiz } = useQuiz(
    quizId ? parseInt(quizId) : 0
  );
  const createQuiz = useCreateQuiz();
  const updateQuiz = useUpdateQuiz();

  const [gameName, setGameName] = useState("");
  const [questions, setQuestions] = useState<Question[]>([]);
  const [hostName, setHostName] = useState("");

  // Load existing quiz data when editing
  useEffect(() => {
    if (existingQuiz && isEditMode) {
      setGameName(existingQuiz.quizName);
      const localQuestions: Question[] = existingQuiz.questions.map((q) => ({
        id: q.id.toString(),
        text: q.questionText,
        answers: q.options.map((opt, idx) => ({
          id: `${q.id}-${idx}`,
          text: opt,
          isCorrect: idx === q.correctOptionIndex,
        })),
      }));
      setQuestions(localQuestions);
    }
  }, [existingQuiz, isEditMode]);

  const addQuestion = () => {
    setQuestions((prev) => [
      ...prev,
      { id: crypto.randomUUID(), text: "", answers: [] },
    ]);
  };

  const deleteQuestion = (questionId: string) => {
    setQuestions((prev) => prev.filter((q) => q.id !== questionId));
  };

  const updateQuestionText = (questionId: string, text: string) => {
    setQuestions((prev) =>
      prev.map((q) => (q.id === questionId ? { ...q, text } : q))
    );
  };

  const addAnswer = (questionId: string) => {
    setQuestions((prev) =>
      prev.map((q) =>
        q.id === questionId
          ? {
              ...q,
              answers: [
                ...q.answers,
                { id: crypto.randomUUID(), text: "", isCorrect: false },
              ],
            }
          : q
      )
    );
  };

  const deleteAnswer = (questionId: string, answerId: string) => {
    setQuestions((prev) =>
      prev.map((q) =>
        q.id === questionId
          ? { ...q, answers: q.answers.filter((a) => a.id !== answerId) }
          : q
      )
    );
  };

  const updateAnswerText = (
    questionId: string,
    answerId: string,
    text: string
  ) => {
    setQuestions((prev) =>
      prev.map((q) =>
        q.id === questionId
          ? {
              ...q,
              answers: q.answers.map((a) =>
                a.id === answerId ? { ...a, text } : a
              ),
            }
          : q
      )
    );
  };

  const toggleCorrectAnswer = (questionId: string, answerId: string) => {
    setQuestions((prev) =>
      prev.map((q) =>
        q.id === questionId
          ? {
              ...q,
              answers: q.answers.map((a) => ({
                ...a,
                isCorrect: a.id === answerId ? !a.isCorrect : false,
              })),
            }
          : q
      )
    );
  };

  const validateForm = () => {
    if (!isEditMode && !hostName.trim()) {
      toast.error("Please enter your name before creating the game");
      return false;
    }

    for (const question of questions) {
      const questionLabel = question.text || "Untitled question";

      if (question.answers.length !== 4) {
        toast.error(
          `Each question must have exactly 4 answers. "${questionLabel}" has ${question.answers.length}.`
        );
        return false;
      }

      const correctCount = question.answers.filter((a) => a.isCorrect).length;
      if (correctCount !== 1) {
        toast.error(
          `Each question must have exactly one correct answer. "${questionLabel}" has ${correctCount}.`
        );
        return false;
      }
    }

    return true;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    try {
      const apiQuestions: QuestionDto[] = questions.map((q) => ({
        id: parseInt(q.id) || 0,
        questionText: q.text,
        options: q.answers.map((a) => a.text),
        correctOptionIndex: q.answers.findIndex((a) => a.isCorrect),
      }));

      if (isEditMode && existingQuiz) {
        // Update existing quiz
        await updateQuiz.mutateAsync({
          quizId: existingQuiz.id,
          quiz: {
            quizName: gameName,
            questions: apiQuestions,
          },
          currentUserId: CURRENT_USER_ID,
        });

        toast.success("Quiz updated successfully!");
        navigate("/edit-game");
      } else {
        // Create new quiz
        const newQuiz = await createQuiz.mutateAsync({
          quiz: {
            quizName: gameName,
            questions: apiQuestions,
          },
          creatorUserId: CURRENT_USER_ID,
        });

        const hostPlayer = {
          id: crypto.randomUUID(),
          name: hostName,
          quizId: newQuiz.id,
          isHost: true,
          joinedAt: new Date().toISOString(),
        };

        localStorage.setItem("currentPlayer", JSON.stringify(hostPlayer));
        localStorage.setItem(
          `players_${newQuiz.id}`,
          JSON.stringify([hostPlayer])
        );

        toast.success("Quiz created successfully!");
        navigate(`/game-lobby?quiz=${newQuiz.id}`);
      }
    } catch (error) {
      console.error(`Failed to ${isEditMode ? "update" : "create"} quiz:`, error);
      toast.error(`Failed to ${isEditMode ? "update" : "create"} quiz. Please try again.`);
    }
  };

  if (isEditMode && loadingQuiz) {
    return (
      <section className="min-h-screen px-4 py-16">
        <div className="mx-auto max-w-4xl flex items-center justify-center">
          <Loader2 className="h-8 w-8 animate-spin" />
        </div>
      </section>
    );
  }

  const isSaving = isEditMode ? updateQuiz.isPending : createQuiz.isPending;

  return (
    <section className="min-h-screen px-4 py-16">
      <div className="mx-auto max-w-4xl">
        <div className="mb-8">
          {isEditMode && (
            <Button
              variant="ghost"
              onClick={() => navigate("/edit-game")}
              className="mb-4"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Games List
            </Button>
          )}
          <h1 className="mb-2 text-3xl font-bold">
            {isEditMode ? "Edit Quiz" : "Create New Game"}
          </h1>
          <p className="text-muted-foreground">
            {isEditMode
              ? "Modify questions and answers for your quiz"
              : "Build your custom trivia game with questions and answers"}
          </p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>
              {isEditMode ? "Quiz Details" : "Game Details"}
            </CardTitle>
            <CardDescription>
              {isEditMode
                ? `Created by: ${existingQuiz?.creatorUsername || "Unknown"}`
                : "Set up your game information"}
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
                  onChange={(e) => setHostName(e.target.value)}
                />
              </div>
            )}
            <div className="space-y-2">
              <Label htmlFor="gameName">
                {isEditMode ? "Quiz Name" : "Game Name"}
              </Label>
              <Input
                id="gameName"
                placeholder="Enter game name..."
                value={gameName}
                onChange={(e) => setGameName(e.target.value)}
              />
            </div>
          </CardContent>
        </Card>

        <div className="mb-6 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-semibold">Questions</h2>
            <Button onClick={addQuestion} size="sm">
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
              <Card key={question.id}>
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <CardTitle className="text-lg">
                      Question {qIndex + 1}
                    </CardTitle>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => deleteQuestion(question.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor={`question-${question.id}`}>
                      Question Text
                    </Label>
                    <Textarea
                      id={`question-${question.id}`}
                      placeholder="Enter your question..."
                      value={question.text}
                      onChange={(e) =>
                        updateQuestionText(question.id, e.target.value)
                      }
                      rows={3}
                    />
                  </div>

                  <div className="space-y-3">
                    <div className="flex items-center justify-between">
                      <Label>Answers ({question.answers.length}/4)</Label>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => addAnswer(question.id)}
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
                        <div
                          key={answer.id}
                          className="flex items-start gap-2 rounded-lg border p-3"
                        >
                          <div className="flex-1 space-y-2">
                            <Input
                              placeholder={`Answer ${aIndex + 1}...`}
                              value={answer.text}
                              onChange={(e) =>
                                updateAnswerText(
                                  question.id,
                                  answer.id,
                                  e.target.value
                                )
                              }
                            />
                            <label className="flex items-center gap-2 text-sm">
                              <input
                                type="checkbox"
                                checked={answer.isCorrect}
                                onChange={() =>
                                  toggleCorrectAnswer(question.id, answer.id)
                                }
                                className="h-4 w-4 rounded border-gray-300"
                              />
                              <span
                                className={
                                  answer.isCorrect
                                    ? "font-semibold text-green-600"
                                    : ""
                                }
                              >
                                Correct Answer
                              </span>
                            </label>
                          </div>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() =>
                              deleteAnswer(question.id, answer.id)
                            }
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      ))
                    )}
                  </div>
                </CardContent>
              </Card>
            ))
          )}
        </div>

        <div className="flex justify-end gap-3">
          <Button
            variant="outline"
            onClick={() => navigate(isEditMode ? "/edit-game" : "/")}
          >
            Cancel
          </Button>
          <Button
            onClick={handleSave}
            disabled={
              (!isEditMode && !hostName.trim()) ||
              !gameName ||
              questions.length === 0 ||
              isSaving
            }
          >
            {isSaving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {isSaving
              ? isEditMode
                ? "Saving..."
                : "Creating..."
              : isEditMode
              ? "Save Changes"
              : "Create & Go to Lobby"}
          </Button>
        </div>
      </div>
    </section>
  );
}
