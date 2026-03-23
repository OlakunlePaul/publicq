import React, { useState, useEffect, useCallback } from 'react';
import { sessionService } from '../../services/sessionService';
import { QuestionResponse } from '../../models/question-response';
import styles from './ManualMarkingModal.module.css';

interface ManualMarkingModalProps {
  isOpen: boolean;
  studentId: string;
  assignmentId: string;
  moduleId: string;
  moduleTitle: string;
  studentName: string;
  onClose: () => void;
}

const ManualMarkingModal: React.FC<ManualMarkingModalProps> = ({
  isOpen,
  studentId,
  assignmentId,
  moduleId,
  moduleTitle,
  studentName,
  onClose,
}) => {
  const [responses, setResponses] = useState<QuestionResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [updatingIds, setUpdatingIds] = useState<Set<string>>(new Set());

  const loadResponses = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const result = await sessionService.getModuleProgress(studentId, assignmentId, moduleId);
      const allResponses = result.data?.questionResponses || [];
      // Filter to only FreeText responses
      const essayResponses = allResponses.filter(
        (r) => r.questionType === 'FreeText' || r.questionType === '2'
      );
      setResponses(essayResponses);
    } catch (err: any) {
      setError('Failed to load responses: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  }, [studentId, assignmentId, moduleId]);

  useEffect(() => {
    if (isOpen) {
      loadResponses();
    }
  }, [isOpen, loadResponses]);

  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) onClose();
    };
    if (isOpen) {
      document.addEventListener('keydown', handleEsc);
      return () => document.removeEventListener('keydown', handleEsc);
    }
  }, [isOpen, onClose]);

  const handleMark = async (responseId: string, isCorrect: boolean) => {
    setUpdatingIds((prev) => new Set(prev).add(responseId));
    try {
      await sessionService.updateQuestionResponseMark(responseId, isCorrect);
      setResponses((prev) =>
        prev.map((r) => (r.id === responseId ? { ...r, isCorrect } : r))
      );
    } catch (err: any) {
      setError('Failed to update mark: ' + (err.response?.data?.message || err.message));
    } finally {
      setUpdatingIds((prev) => {
        const next = new Set(prev);
        next.delete(responseId);
        return next;
      });
    }
  };

  if (!isOpen) return null;

  const markedCount = responses.filter((r) => r.isCorrect !== null && r.isCorrect !== undefined).length;

  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.header}>
          <div>
            <h3 className={styles.title}>Review Essay Answers</h3>
            <p className={styles.subtitle}>
              {studentName} — {moduleTitle}
            </p>
          </div>
          <button className={styles.closeBtn} onClick={onClose}>
            ×
          </button>
        </div>

        <div className={styles.body}>
          {error && <div className={styles.error}>{error}</div>}

          {loading ? (
            <div className={styles.loading}>Loading essay responses…</div>
          ) : responses.length === 0 ? (
            <div className={styles.noEssays}>
              No essay questions found for this module.
            </div>
          ) : (
            responses.map((response, idx) => {
              const isUpdating = updatingIds.has(response.id);
              const questionText = response.question?.text || `Question ${idx + 1}`;
              const markStatus =
                response.isCorrect === true
                  ? 'correct'
                  : response.isCorrect === false
                  ? 'incorrect'
                  : 'pending';

              return (
                <div key={response.id} className={styles.questionCard}>
                  <div className={styles.questionHeader}>
                    <span className={styles.questionNumber}>
                      Essay {idx + 1} of {responses.length}
                    </span>
                    <span
                      className={`${styles.markBadge} ${
                        markStatus === 'correct'
                          ? styles.markCorrect
                          : markStatus === 'incorrect'
                          ? styles.markIncorrect
                          : styles.markPending
                      }`}
                    >
                      {markStatus === 'correct'
                        ? '✓ Correct'
                        : markStatus === 'incorrect'
                        ? '✗ Incorrect'
                        : '⏳ Pending'}
                    </span>
                  </div>

                  <div className={styles.questionText}>{questionText}</div>

                  <div className={styles.answerSection}>
                    <div className={styles.answerLabel}>Student's Answer</div>
                    <div
                      className={`${styles.answerText} ${
                        !response.textResponse ? styles.emptyAnswer : ''
                      }`}
                    >
                      {response.textResponse || 'No answer provided'}
                    </div>
                  </div>

                  <div className={styles.actions}>
                    <button
                      className={`${styles.markBtn} ${styles.markBtnCorrect} ${
                        response.isCorrect === true ? styles.markBtnCorrectActive : ''
                      }`}
                      onClick={() => handleMark(response.id, true)}
                      disabled={isUpdating}
                    >
                      ✓ {isUpdating ? 'Saving…' : 'Correct'}
                    </button>
                    <button
                      className={`${styles.markBtn} ${styles.markBtnIncorrect} ${
                        response.isCorrect === false ? styles.markBtnIncorrectActive : ''
                      }`}
                      onClick={() => handleMark(response.id, false)}
                      disabled={isUpdating}
                    >
                      ✗ {isUpdating ? 'Saving…' : 'Incorrect'}
                    </button>
                  </div>
                </div>
              );
            })
          )}
        </div>

        <div className={styles.footer}>
          <span className={styles.summary}>
            <span className={styles.summaryCount}>{markedCount}</span> of{' '}
            <span className={styles.summaryCount}>{responses.length}</span> marked
          </span>
          <button className={styles.doneBtn} onClick={onClose}>
            Done
          </button>
        </div>
      </div>
    </div>
  );
};

export default ManualMarkingModal;
