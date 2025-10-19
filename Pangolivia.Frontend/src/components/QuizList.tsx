import { useQuizzes, useDeleteQuiz } from '@/hooks/useQuizzes';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Trash2, Edit, Eye } from 'lucide-react';

interface QuizListProps {
  onEdit?: (quizId: number) => void;
  onView?: (quizId: number) => void;
}

export function QuizList({ onEdit, onView }: QuizListProps) {
  const { data: quizzes, isLoading, error } = useQuizzes();
  const deleteQuiz = useDeleteQuiz();

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this quiz?')) {
      try {
        await deleteQuiz.mutateAsync({ id, currentUserId: 1 }); // Using hardcoded userId for now
      } catch (err) {
        console.error('Failed to delete quiz:', err);
      }
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <p className="text-muted-foreground">Loading quizzes...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center p-8">
        <p className="text-destructive">Error loading quizzes: {error.message}</p>
      </div>
    );
  }

  if (!quizzes || quizzes.length === 0) {
    return (
      <div className="flex items-center justify-center p-8">
        <p className="text-muted-foreground">No quizzes found</p>
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {quizzes.map((quiz) => (
        <Card key={quiz.id} className="hover:shadow-lg transition-shadow">
          <CardHeader>
            <CardTitle>{quiz.quizName}</CardTitle>
            <CardDescription>
              By {quiz.creatorUsername} • {quiz.questionCount} questions
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex gap-2">
              {onView && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onView(quiz.id)}
                  className="flex-1"
                >
                  <Eye className="mr-2 h-4 w-4" />
                  View
                </Button>
              )}
              {onEdit && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onEdit(quiz.id)}
                  className="flex-1"
                >
                  <Edit className="mr-2 h-4 w-4" />
                  Edit
                </Button>
              )}
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleDelete(quiz.id)}
                disabled={deleteQuiz.isPending}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
