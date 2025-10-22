import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Trash2, Edit, Loader2 } from "lucide-react";
import { useQuizzesByUser, useDeleteQuiz } from "@/hooks/useQuizzes";
import { useAuth } from '@/hooks/useAuth';
import { toast } from "sonner";

// const CURRENT_USER_ID = 1;

export default function EditGamePage() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const { data: allQuizzes, isLoading: loadingQuizzes } = useQuizzesByUser(user?.id ?? 0);
  const deleteQuiz = useDeleteQuiz();

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [quizToDelete, setQuizToDelete] = useState<number | null>(null);

  const handleSelectGame = (quizId: number) => {
    navigate(`/quiz/edit/${quizId}`);
  };

  const handleDeleteGame = (quizId: number) => {
    setQuizToDelete(quizId);
    setDeleteDialogOpen(true);
  };

  const confirmDeleteGame = async () => {
    if (!quizToDelete || !user) return;
    if (!quizToDelete) return;

    try {
      await deleteQuiz.mutateAsync({
        id: quizToDelete,
        currentUserId: user?.id,
      });

      // Clean up related data
      localStorage.removeItem(`players_${quizToDelete}`);
      localStorage.removeItem(`game_${quizToDelete}_started`);

      toast.success("Quiz deleted successfully!");
      setDeleteDialogOpen(false);
      setQuizToDelete(null);
    } catch (error) {
      console.error("Failed to delete quiz:", error);
      toast.error("Failed to delete quiz. Please try again.");
    }
  };

  if (loadingQuizzes) {
    return (
      <section className="min-h-[calc(100vh-5rem)] px-4 py-2 flex items-center justify-center">
        <div className="w-full max-w-3xl">
          <Loader2 className="h-8 w-8 animate-spin" />
        </div>
      </section>
    );
  }

  return (
    <section className="min-h-[calc(100vh-5rem)] px-4 py-2 flex items-center justify-center">
      <div className="w-full max-w-3xl">
        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold">Edit Quizzes</h1>
          <p className="text-muted-foreground">
            Select a quiz to edit or delete
          </p>
        </div>

        {!allQuizzes || allQuizzes.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <p className="text-muted-foreground mb-4">
                No quizzes created yet.
              </p>
              <Button onClick={() => navigate("/create-game")}>
                Create Your First Quiz
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            {allQuizzes.map((quiz) => (
              <Card key={quiz.id}>
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div>
                      <CardTitle className="text-xl">
                        {quiz.quizName}
                      </CardTitle>
                      <CardDescription className="mt-1">
                        By {quiz.creatorUsername} • {quiz.questionCount}{" "}
                        questions
                      </CardDescription>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleSelectGame(quiz.id)}
                      >
                        <Edit className="mr-2 h-4 w-4" />
                        Edit
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleDeleteGame(quiz.id)}
                        disabled={deleteQuiz.isPending}
                      >
                        {deleteQuiz.isPending ? (
                          <Loader2 className="h-4 w-4 animate-spin" />
                        ) : (
                          <Trash2 className="h-4 w-4 text-red-600" />
                        )}
                      </Button>
                    </div>
                  </div>
                </CardHeader>
              </Card>
            ))}
          </div>
        )}
      </div>

      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the
              quiz and all associated data.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDeleteGame}
              className="bg-red-600 hover:bg-red-700"
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </section>
  );
}
