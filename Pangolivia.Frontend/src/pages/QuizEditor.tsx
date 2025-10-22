import { useState, useEffect, useRef } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Loader2 } from "lucide-react";
import {
  useCreateQuiz,
  useQuiz,
  useUpdateQuiz,
  useGenerateAiQuestions,
} from "@/hooks/useQuizzes";
import { useCreateGame } from "@/hooks/useGames"; // Import the useCreateGame hook
import type { QuestionDto } from "@/types/api";
import { useAuth } from "@/hooks/useAuth";
import { toast } from "sonner";
import {
  QuizHeader,
  QuizDetailsCard,
  QuestionsList,
  AiGeneratorCard,
} from "@/features/quiz-editor/components";
import type { Question, Answer } from "@/features/quiz-editor/components";

// --- TRANSFORMATION HELPERS ---
/**
 * Converts API question format (flat) to the local state format used by the UI.
 * The UI works better with a structured array of answers.
 */
const apiToLocalQuestions = (apiQuestions: QuestionDto[]): Question[] => {
  return apiQuestions.map((q) => ({
    id: q.id.toString(), // The UI uses string IDs for temporary items
    text: q.questionText,
    answers: [
      { id: `${q.id}-0`, text: q.correctAnswer, isCorrect: true },
      { id: `${q.id}-1`, text: q.answer2, isCorrect: false },
      { id: `${q.id}-2`, text: q.answer3, isCorrect: false },
      { id: `${q.id}-3`, text: q.answer4, isCorrect: false },
    ],
  }));
};

/**
 * Converts local UI state format (structured) back to the API format (flat) before submission.
 */
const localToApiQuestions = (localQuestions: Question[]): QuestionDto[] => {
  return localQuestions.map((q) => {
    const correct = q.answers.find((a) => a.isCorrect);
    const incorrect = q.answers.filter((a) => !a.isCorrect);

    return {
      id: parseInt(q.id),
      questionText: q.text,
      correctAnswer: correct?.text ?? "",
      answer2: incorrect[0]?.text ?? "",
      answer3: incorrect[1]?.text ?? "",
      answer4: incorrect[2]?.text ?? "",
    };
  });
};


interface QuizEditorProps {
  mode: "create" | "edit";
}

// Define a type for the local state to ensure type safety
interface LocalQuizState {
  id: number;
  quizName: string;
  createdByUserId: number;
  creatorUsername: string;
  questions: Question[]; // Use the UI-friendly Question type
}

export default function QuizEditorPage({ mode }: QuizEditorProps) {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditMode = mode === "edit";
  const quizId = isEditMode ? id : null;

  const nextIdRef = useRef(-1); // For temporary client-side IDs

  const { data: existingQuiz, isLoading: loadingQuiz } = useQuiz(
    quizId ? parseInt(quizId) : 0
  );
  const createQuizMutation = useCreateQuiz();
  const updateQuizMutation = useUpdateQuiz();
  const generateAiMutation = useGenerateAiQuestions();
  const createGameMutation = useCreateGame(); // Instantiate the create game hook

  const [aiTopic, setAiTopic] = useState<string>("");
  const [aiCount, setAiCount] = useState<number>(5);
  const [aiDifficulty, setAiDifficulty] = useState<"easy" | "medium" | "hard">("medium");
  const [hostName, setHostName] = useState<string>("");

  // State now uses the clean, local format for questions
  const [draftQuiz, setDraftQuiz] = useState<LocalQuizState>({
    id: 0,
    quizName: "",
    questions: [],
    createdByUserId: user?.id ?? 0,
    creatorUsername: user?.name ?? "",
  });

  useEffect(() => {
    if (isEditMode && existingQuiz) {
      // Transform fetched data into local format ONCE
      setDraftQuiz({
        ...existingQuiz,
        questions: apiToLocalQuestions(existingQuiz.questions),
      });
      nextIdRef.current = -1;
    }
  }, [isEditMode, existingQuiz]);

  const updateQuizState = (updater: (current: LocalQuizState) => LocalQuizState) => {
    setDraftQuiz((prev) => updater(prev));
  };

  // Actions are now refactored to work with the `Question[]` and `Answer[]` types
  const actions = {
    setGameName: (value: string) => {
      updateQuizState((current) => ({ ...current, quizName: value }));
    },
    setHostName: (value: string) => setHostName(value),
    addQuestion: () => {
      updateQuizState((current) => {
        const newQuestion: Question = {
          id: (nextIdRef.current--).toString(),
          text: "",
          answers: [],
        };
        return { ...current, questions: [...current.questions, newQuestion] };
      });
    },
    deleteQuestion: (questionId: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.filter((q) => q.id !== questionId),
      }));
    },
    updateQuestionText: (questionId: string, text: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === questionId ? { ...q, text } : q
        ),
      }));
    },
    addAnswer: (questionId: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) => {
          if (q.id === questionId && q.answers.length < 4) {
            const newAnswer: Answer = {
              id: `${questionId}-${q.answers.length}`,
              text: "",
              isCorrect: q.answers.length === 0, // First answer is correct by default
            };
            return { ...q, answers: [...q.answers, newAnswer] };
          }
          return q;
        }),
      }));
    },
    deleteAnswer: (questionId: string, answerId: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) => {
          if (q.id === questionId) {
            const newAnswers = q.answers.filter((a) => a.id !== answerId);
            // If the deleted answer was correct, make the first one correct
            if (!newAnswers.some(a => a.isCorrect) && newAnswers.length > 0) {
              newAnswers[0].isCorrect = true;
            }
            return { ...q, answers: newAnswers };
          }
          return q;
        }),
      }));
    },
    updateAnswerText: (questionId: string, answerId: string, text: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === questionId
            ? {
                ...q,
                answers: q.answers.map((a) =>
                  a.id === answerId ? { ...a, text } : a
                ),
              }
            : q
        ),
      }));
    },
    toggleCorrect: (questionId: string, answerId: string) => {
      updateQuizState((current) => ({
        ...current,
        questions: current.questions.map((q) =>
          q.id === questionId
            ? {
                ...q,
                answers: q.answers.map((a) => ({
                  ...a,
                  isCorrect: a.id === answerId,
                })),
              }
            : q
        ),
      }));
    },
  };

  const handleGenerateAi = async () => {
    const topic = aiTopic.trim();
    if (!topic) {
      toast.error("Please enter a topic for AI generation");
      return;
    }
    if (aiCount < 1 || aiCount > 50) {
      toast.error("Number of questions must be between 1 and 50");
      return;
    }

    try {
      const aiApiQuestions = await generateAiMutation.mutateAsync({
        topic,
        numberOfQuestions: aiCount,
        difficulty: aiDifficulty,
      });

      if (!aiApiQuestions || aiApiQuestions.length === 0) {
        toast("No questions generated. Try a different topic or difficulty.");
        return;
      }

      // Convert the API response into the local format before adding to state
      const newLocalQuestions = apiToLocalQuestions(aiApiQuestions.map(q => ({ ...q, id: nextIdRef.current-- })));

      updateQuizState((current) => ({
        ...current,
        questions: [...current.questions, ...newLocalQuestions],
      }));

      toast.success(`Added ${aiApiQuestions.length} AI-generated question(s).`);
    } catch (err) {
      const error = err as { response?: { data?: string }; message?: string }
      const msg = error?.response?.data ?? error?.message ?? "Failed to generate with AI";
      toast.error(typeof msg === "string" ? msg : "Failed to generate with AI");
    }
  };

  const validateForm = () => {
    if (!draftQuiz) return false;
    const { questions, quizName } = draftQuiz;

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
    for (const q of questions) {
      if (!q.text.trim()) {
        toast.error(`Question "${q.id}" cannot be empty.`);
        return false;
      }
      if (q.answers.length !== 4) {
        toast.error(`Question "${q.text}" must have exactly 4 answers.`);
        return false;
      }
      if (!q.answers.some(a => a.isCorrect)) {
        toast.error(`Question "${q.text}" must have one correct answer.`);
        return false;
      }
      for (const a of q.answers) {
        if (!a.text.trim()) {
          toast.error(`Answers for "${q.text}" cannot be empty.`);
          return false;
        }
      }
    }
    return true;
  };

  const handleSave = async () => {
    if (!validateForm() || !draftQuiz) return;

    // Convert local state back to API format before submitting
    const apiQuestions = localToApiQuestions(draftQuiz.questions);

    try {
      if (isEditMode && existingQuiz) {
        const questionsForApi = apiQuestions.map((q) => ({
          ...q,
          id: q.id < 0 ? 0 : q.id,
        }));
        await updateQuizMutation.mutateAsync({
          quizId: existingQuiz.id,
          quiz: { quizName: draftQuiz.quizName, questions: questionsForApi },
          currentUserId: user?.id ?? 0,
        });
        toast.success("Quiz updated successfully!");
        navigate("/edit-game");
      } else {
        // --- THIS IS THE UPDATED LOGIC ---
        const questionsForApi = apiQuestions.map((q) => ({ ...q, id: 0 }));
        
        // Step 1: Create the quiz
        const newQuiz = await createQuizMutation.mutateAsync({
          quiz: { quizName: draftQuiz.quizName, questions: questionsForApi },
          creatorUserId: user?.id ?? 0,
        });

        toast.success("Quiz created successfully! Creating game lobby...");

        // Step 2: Create a game session, which will navigate to the lobby on success
        createGameMutation.mutate(newQuiz.id);
      }
    } catch (err) {
      console.error(`Failed to ${isEditMode ? "update" : "create"} quiz:`, err);
      toast.error(`Failed to ${isEditMode ? "update" : "create"} quiz. Please try again.`);
    }
  };

  if (isEditMode && loadingQuiz) {
    return (
      <section className="min-h-screen px-4 py-16 flex items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin" />
      </section>
    );
  }

  if (!draftQuiz) return null;

  const isSaving = isEditMode
    ? updateQuizMutation.isPending
    : createQuizMutation.isPending || createGameMutation.isPending; // Update loading state

  return (
    <section className="flex justify-center items-center min-h-[calc(100vh-5rem)] px-4 py-2">
      <div className="w-full max-w-3xl">
        <QuizHeader isEditMode={isEditMode} onBack={() => navigate("/edit-game")} />
        <QuizDetailsCard
          isEditMode={isEditMode}
          gameName={draftQuiz.quizName}
          hostName={hostName}
          creatorUsername={draftQuiz.creatorUsername}
          onGameNameChange={actions.setGameName}
          onHostNameChange={actions.setHostName}
        />
        <AiGeneratorCard
          topic={aiTopic}
          count={aiCount}
          difficulty={aiDifficulty}
          isGenerating={generateAiMutation.isPending}
          onTopicChange={setAiTopic}
          onCountChange={setAiCount}
          onDifficultyChange={setAiDifficulty}
          onGenerate={handleGenerateAi}
        />
        <QuestionsList
          questions={draftQuiz.questions}
          onAddQuestion={actions.addQuestion}
          onDeleteQuestion={actions.deleteQuestion}
          onUpdateQuestionText={actions.updateQuestionText}
          onAddAnswer={actions.addAnswer}
          onDeleteAnswer={actions.deleteAnswer}
          onUpdateAnswerText={actions.updateAnswerText}
          onToggleCorrect={actions.toggleCorrect}
        />
        <div className="flex justify-end gap-3">
          <Button variant="outline" onClick={() => navigate(isEditMode ? "/edit-game" : "/")}>
            Cancel
          </Button>
          <Button onClick={handleSave} disabled={isSaving}>
            {isSaving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {isSaving
              ? isEditMode ? "Saving..." : "Creating..."
              : isEditMode ? "Save Changes" : "Create & Go to Lobby"}
          </Button>
        </div>
      </div>
    </section>
  );
}