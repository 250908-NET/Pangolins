import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Loader2 } from "lucide-react";
import { useCreateQuiz, useQuiz, useUpdateQuiz } from "@/hooks/useQuizzes";
import type { QuestionDto, QuizDetailDto } from "@/types/api";
import { useAuth } from "@/contexts/AuthContext";
import { toast } from "sonner";
import {
  QuizHeader,
  QuizDetailsCard,
  QuestionsList,
} from "@/features/quiz-editor/components";
import type { Question } from "@/features/quiz-editor/components";

// const CURRENT_USER_ID = 1;

// Helper: Convert API to local format
const toLocalQuestions = (questions: QuestionDto[]): Question[] =>
  questions.map((q) => ({
    id: q.id.toString(),
    text: q.questionText,
    answers: q.options.map((opt, idx) => ({
      id: `${q.id}-${idx}`,
      text: opt,
      isCorrect: idx === q.correctOptionIndex,
    })),
  }));

interface QuizEditorProps {
  mode: "create" | "edit";
}

export default function QuizEditorPage({ mode }: QuizEditorProps) {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditMode = mode === "edit";
  const quizId = isEditMode ? id : null;

  // Fetch existing quiz if editing
  const { data: existingQuiz, isLoading: loadingQuiz } = useQuiz(
    quizId ? parseInt(quizId) : 0
  );
  const createQuizMutation = useCreateQuiz();
  const updateQuizMutation = useUpdateQuiz();

  // Local state for draft quiz and host name
  const [hostName, setHostName] = useState<string>("");
  const [draftQuiz, setDraftQuiz] = useState<QuizDetailDto>({
    id: 0,
    quizName: "",
    questions: [],
    createdByUserId: user?.id ?? 0,
    creatorUsername: user?.name ?? "",
  });

  // Load existing quiz data into local state when editing
  useEffect(() => {
    if (isEditMode && existingQuiz) {
      setDraftQuiz(existingQuiz);
    }
  }, [isEditMode, existingQuiz]);

  // Current quiz data
  const currentQuiz = draftQuiz;

  // Update quiz state
  const updateQuizState = (
    updater: (current: QuizDetailDto) => QuizDetailDto
  ) => {
    setDraftQuiz((prev) => updater(prev));
  };

  // Actions: Simple state updates
  const actions = {
    setGameName: (value: string) => {
      updateQuizState((current) => ({ ...current, quizName: value }));
    },

    setHostName: (value: string) => {
      setHostName(value);
    },

    addQuestion: () => {
      updateQuizState((current) => ({
        ...current,
        questions: [
          ...current.questions,
          {
            id: Date.now(),
            questionText: "",
            options: [],
            correctOptionIndex: 0,
          },
        ],
      }));
    },

    deleteQuestion: (questionId: string) => {
      const id = parseInt(questionId);
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.filter((q) => q.id !== id),
      }));
    },

    updateQuestionText: (questionId: string, text: string) => {
      const id = parseInt(questionId);
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === id ? { ...q, questionText: text } : q
        ),
      }));
    },

    addAnswer: (questionId: string) => {
      const id = parseInt(questionId);
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === id ? { ...q, options: [...q.options, ""] } : q
        ),
      }));
    },

    deleteAnswer: (questionId: string, answerId: string) => {
      const qId = parseInt(questionId);
      const answerIndex = parseInt(answerId.split("-")[1]);

      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === qId
            ? {
                ...q,
                options: q.options.filter((_, idx) => idx !== answerIndex),
                correctOptionIndex:
                  q.correctOptionIndex === answerIndex
                    ? 0
                    : q.correctOptionIndex > answerIndex
                    ? q.correctOptionIndex - 1
                    : q.correctOptionIndex,
              }
            : q
        ),
      }));
    },

    updateAnswerText: (questionId: string, answerId: string, text: string) => {
      const qId = parseInt(questionId);
      const answerIndex = parseInt(answerId.split("-")[1]);

      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === qId
            ? {
                ...q,
                options: q.options.map((opt, idx) =>
                  idx === answerIndex ? text : opt
                ),
              }
            : q
        ),
      }));
    },

    toggleCorrectAnswer: (questionId: string, answerId: string) => {
      const qId = parseInt(questionId);
      const answerIndex = parseInt(answerId.split("-")[1]);

      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === qId ? { ...q, correctOptionIndex: answerIndex } : q
        ),
      }));
    },
  };

  const validateForm = () => {
    if (!currentQuiz) return false;

    const { questions, quizName } = currentQuiz;

    if (!isEditMode && !hostName.trim()) {
      toast.error("Please enter your name before creating the game");
      return false;
    }

    if (!quizName.trim()) {
      toast.error("Please enter a quiz name");
      return false;
    }

    if (questions.length === 0) {
      toast.error("Please add at least one question");
      return false;
    }

    for (const question of questions) {
      const questionLabel = question.questionText || "Untitled question";

      if (!question.questionText.trim()) {
        toast.error(`Please enter text for "${questionLabel}"`);
        return false;
      }

      if (question.options.length !== 4) {
        toast.error(
          `Each question must have exactly 4 answers. "${questionLabel}" has ${question.options.length}.`
        );
        return false;
      }

      // Check all options have text
      for (let i = 0; i < question.options.length; i++) {
        if (!question.options[i].trim()) {
          toast.error(`"${questionLabel}" - Answer ${i + 1} cannot be empty`);
          return false;
        }
      }

      if (
        question.correctOptionIndex < 0 ||
        question.correctOptionIndex >= question.options.length
      ) {
        toast.error(`"${questionLabel}" must have a correct answer selected.`);
        return false;
      }
    }

    return true;
  };

  const handleSave = async () => {
    if (!validateForm() || !currentQuiz) return;

    try {
      if (isEditMode && existingQuiz) {
        console.log("Updating quiz:", {
          quizName: currentQuiz.quizName,
          questions: currentQuiz.questions,
        });
        await updateQuizMutation.mutateAsync({
          quizId: existingQuiz.id,
          quiz: {
            quizName: currentQuiz.quizName,
            questions: currentQuiz.questions,
          },
          currentUserId: user?.id ?? 0,
        });
        toast.success("Quiz updated successfully!");
        navigate("/edit-game");
      } else {
        // For new quiz, set question IDs to 0 (API will assign real IDs)
        const questionsForApi = currentQuiz.questions.map((q) => ({
          ...q,
          id: 0, // API assigns IDs on creation
        }));

        console.log("Creating quiz:", {
          quizName: currentQuiz.quizName,
          questions: questionsForApi,
        });
        const newQuiz = await createQuizMutation.mutateAsync({
          quiz: { quizName: currentQuiz.quizName, questions: questionsForApi },
          creatorUserId: user?.id ?? 0,
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
    } catch (error: any) {
      console.error(
        `Failed to ${isEditMode ? "update" : "create"} quiz:`,
        error
      );
      console.error("Error details:", error.response?.data || error.message);
      const errorMessage =
        error.response?.data?.message || error.message || "Please try again.";
      toast.error(
        `Failed to ${isEditMode ? "update" : "create"} quiz: ${errorMessage}`
      );
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

  if (!currentQuiz) return null;

  const isSaving = isEditMode
    ? updateQuizMutation.isPending
    : createQuizMutation.isPending;
  const localQuestions = toLocalQuestions(currentQuiz.questions);

  return (
    <section className="min-h-screen px-4 py-16">
      <div className="mx-auto max-w-4xl">
        <QuizHeader
          isEditMode={isEditMode}
          onBack={() => navigate("/edit-game")}
        />

        <QuizDetailsCard
          isEditMode={isEditMode}
          gameName={currentQuiz.quizName}
          hostName={hostName}
          creatorUsername={currentQuiz.creatorUsername}
          onGameNameChange={actions.setGameName}
          onHostNameChange={actions.setHostName}
        />

        <QuestionsList
          questions={localQuestions}
          onAddQuestion={actions.addQuestion}
          onDeleteQuestion={actions.deleteQuestion}
          onUpdateQuestionText={actions.updateQuestionText}
          onAddAnswer={actions.addAnswer}
          onDeleteAnswer={actions.deleteAnswer}
          onUpdateAnswerText={actions.updateAnswerText}
          onToggleCorrect={actions.toggleCorrectAnswer}
        />

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
              !currentQuiz.quizName ||
              currentQuiz.questions.length === 0 ||
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
