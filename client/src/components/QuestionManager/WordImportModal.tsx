import { useState, useRef, useCallback } from 'react';
import mammoth from 'mammoth';
import { v4 as uuidv4 } from 'uuid';
import { QuestionCreateDto, PossibleAnswerCreateDto } from '../../models/assessment-modules-create';
import { QuestionType } from '../../models/question-types';

interface ParsedQuestion {
  text: string;
  type: QuestionType;
  answers: { text: string; isCorrect: boolean }[];
  selected: boolean;
}

interface Props {
  moduleId: string;
  moduleVersionId: string;
  startingOrder: number;
  onImport: (questions: QuestionCreateDto[]) => void;
  onClose: () => void;
}

/**
 * Parses raw text extracted from a Word document into structured questions.
 * Supports patterns like:
 *   1. Question text
 *      a) Option A
 *      b) Option B *
 *      c) Option C
 * 
 * The correct answer is marked with an asterisk (*) at the end.
 * If no options are found, the question defaults to FreeText type.
 */
function parseQuestionsFromText(rawText: string): ParsedQuestion[] {
  // Split the raw text into non-empty lines
  const rawLines = rawText.split('\n').map(l => l.trim()).filter(l => l.length > 0);
  const questions: ParsedQuestion[] = [];

  let currentQuestion: ParsedQuestion | null = null;

  for (const rawLine of rawLines) {
    // Check if the line contains inline options (e.g., "1. Question a) Opt A b) Opt B")
    // We split the line by looking ahead for a space followed by a letter, a punctuation (. or ) or ]), and either a space or end of string.
    const parts = rawLine.split(/(?=\s+[a-zA-Z][.)\]](?:\s+|$))/).map(p => p.trim()).filter(p => p.length > 0);

    for (const line of parts) {
      // Match option pattern: starts with a letter followed by . or ) or ]
      const optionMatch = line.match(/^[a-zA-Z][.)\]]\s*(.+)/);

      if (optionMatch) {
        if (!currentQuestion) continue; // Ignore loose options before any question text
        
        let answerText = optionMatch[1].trim();
        let isCorrect = false;

        // Check for correct answer marker (trailing asterisk, or wrapped in ** **)
        if (answerText.endsWith('*') || answerText.endsWith('**')) {
          isCorrect = true;
          answerText = answerText.replace(/\*+$/, '').trim();
        }
        // Also check for "correct" or "(correct)" marker
        if (/\(correct\)/i.test(answerText)) {
          isCorrect = true;
          answerText = answerText.replace(/\s*\(correct\)\s*/i, '').trim();
        }

        currentQuestion.answers.push({ text: answerText, isCorrect });
      } else {
        // It's not an option. It's either a new question or a continuation.
        // Match optional manual numbering at the beginning of the line to strip it
        const numberMatch = line.match(/^\d+[.)]\s+(.+)/);
        const textContent = numberMatch ? numberMatch[1].trim() : line.trim();

        // If the line explicitly starts with a number, OR if the current question already had options,
        // it means we are starting a NEW question. (Because options always come after the question text).
        if (numberMatch || (currentQuestion && currentQuestion.answers.length > 0)) {
          if (currentQuestion) {
            finalizeQuestion(currentQuestion);
            questions.push(currentQuestion);
          }
          currentQuestion = {
            text: textContent,
            type: QuestionType.SingleChoice,
            answers: [],
            selected: true,
          };
        } else if (!currentQuestion) {
          // First line of the document
          currentQuestion = {
            text: textContent,
            type: QuestionType.SingleChoice,
            answers: [],
            selected: true,
          };
        } else {
          // Continuation of current multi-line question text
          currentQuestion.text += ' ' + textContent;
        }
      }
    }
  }

  // Push the last question
  if (currentQuestion) {
    finalizeQuestion(currentQuestion);
    questions.push(currentQuestion);
  }

  return questions;
}

function finalizeQuestion(q: ParsedQuestion) {
  if (q.answers.length === 0) {
    q.type = QuestionType.FreeText;
  } else {
    // If no answer is marked correct, default the first one
    if (!q.answers.some(a => a.isCorrect)) {
      q.answers[0].isCorrect = true;
    }
    // If multiple answers are marked correct, it's MultipleChoice
    const correctCount = q.answers.filter(a => a.isCorrect).length;
    q.type = correctCount > 1 ? QuestionType.MultipleChoice : QuestionType.SingleChoice;
  }
}

export const WordImportModal = ({ moduleId, moduleVersionId, startingOrder, onImport, onClose }: Props) => {
  const [step, setStep] = useState<'upload' | 'preview'>('upload');
  const [parsedQuestions, setParsedQuestions] = useState<ParsedQuestion[]>([]);
  const [isParsing, setIsParsing] = useState(false);
  const [parseError, setParseError] = useState<string | null>(null);
  const [isImporting, setIsImporting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = useCallback(async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!file.name.endsWith('.docx')) {
      setParseError('Please select a .docx file.');
      return;
    }

    setIsParsing(true);
    setParseError(null);

    try {
      const arrayBuffer = await file.arrayBuffer();
      const result = await mammoth.extractRawText({ arrayBuffer });
      const questions = parseQuestionsFromText(result.value);

      if (questions.length === 0) {
        setParseError('No questions found in the document. Please ensure your questions follow a numbered format (e.g., "1. Question text").');
        setIsParsing(false);
        return;
      }

      setParsedQuestions(questions);
      setStep('preview');
    } catch (err) {
      setParseError('Failed to parse the document. Please ensure it is a valid .docx file.');
    } finally {
      setIsParsing(false);
    }
  }, []);

  const toggleQuestionSelection = (index: number) => {
    setParsedQuestions(prev => prev.map((q, i) =>
      i === index ? { ...q, selected: !q.selected } : q
    ));
  };

  const selectAll = () => setParsedQuestions(prev => prev.map(q => ({ ...q, selected: true })));
  const deselectAll = () => setParsedQuestions(prev => prev.map(q => ({ ...q, selected: false })));

  const handleImport = useCallback(async () => {
    setIsImporting(true);
    const selectedQuestions = parsedQuestions.filter(q => q.selected);

    const questionsToCreate: QuestionCreateDto[] = selectedQuestions.map((q, index) => ({
      internalId: uuidv4(),
      order: startingOrder + index,
      moduleId,
      moduleVersionId,
      text: q.text,
      staticFileIds: [],
      type: q.type,
      answers: q.answers.map((a, aIndex): PossibleAnswerCreateDto => ({
        text: a.text,
        order: aIndex,
        staticFileIds: [],
        isCorrect: a.isCorrect,
      })),
    }));

    onImport(questionsToCreate);
    setIsImporting(false);
  }, [parsedQuestions, startingOrder, moduleId, moduleVersionId, onImport]);

  const selectedCount = parsedQuestions.filter(q => q.selected).length;

  const questionTypeLabel = (type: QuestionType) => {
    switch (type) {
      case QuestionType.SingleChoice: return 'Single Choice';
      case QuestionType.MultipleChoice: return 'Multiple Choice';
      case QuestionType.FreeText: return 'Essay / Free Text';
      default: return 'Unknown';
    }
  };

  return (
    <div style={modalStyles.overlay} onClick={onClose}>
      <div style={modalStyles.modal} onClick={e => e.stopPropagation()}>
        <div style={modalStyles.header}>
          <h2 style={modalStyles.headerTitle}>
            {step === 'upload' ? '📄 Import Questions from Word' : `📋 Preview (${selectedCount} selected)`}
          </h2>
          <button onClick={onClose} style={modalStyles.closeButton}>✕</button>
        </div>

        {step === 'upload' && (
          <div style={modalStyles.uploadArea}>
            <div
              style={modalStyles.dropZone}
              onClick={() => fileInputRef.current?.click()}
            >
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📁</div>
              <p style={{ fontSize: '1.1rem', fontWeight: 600, color: '#2c3e50', marginBottom: '0.5rem' }}>
                Click to select a Word Document
              </p>
              <p style={{ fontSize: '0.875rem', color: '#6c757d' }}>
                Supports .docx files. Questions should be numbered (1. 2. 3.) with lettered options (a. b. c.)
              </p>
              <p style={{ fontSize: '0.8rem', color: '#95a5a6', marginTop: '0.5rem' }}>
                Mark correct answers with an asterisk (*) at the end of the option
              </p>
              <input
                ref={fileInputRef}
                type="file"
                accept=".docx"
                onChange={handleFileSelect}
                style={{ display: 'none' }}
              />
            </div>

            {isParsing && (
              <div style={{ textAlign: 'center', padding: '1rem', color: '#3498db' }}>
                <div style={{ fontSize: '1.5rem', marginBottom: '0.5rem' }}>⏳</div>
                Parsing document...
              </div>
            )}

            {parseError && (
              <div style={modalStyles.errorBox}>
                {parseError}
              </div>
            )}
          </div>
        )}

        {step === 'preview' && (
          <div style={modalStyles.previewArea}>
            <div style={modalStyles.selectionBar}>
              <span style={{ fontSize: '0.875rem', color: '#6c757d' }}>
                {parsedQuestions.length} questions found
              </span>
              <div>
                <button onClick={selectAll} style={modalStyles.selectionButton}>Select All</button>
                <button onClick={deselectAll} style={{ ...modalStyles.selectionButton, marginLeft: '0.5rem' }}>Deselect All</button>
              </div>
            </div>

            <div style={modalStyles.questionsList}>
              {parsedQuestions.map((q, index) => (
                <div
                  key={index}
                  style={{
                    ...modalStyles.questionItem,
                    opacity: q.selected ? 1 : 0.5,
                    borderColor: q.selected ? '#3498db' : '#e1e8ed',
                  }}
                >
                  <div style={modalStyles.questionHeader}>
                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer', flex: 1 }}>
                      <input
                        type="checkbox"
                        checked={q.selected}
                        onChange={() => toggleQuestionSelection(index)}
                        style={{ width: '18px', height: '18px', cursor: 'pointer' }}
                      />
                      <span style={{ fontWeight: 600, color: '#2c3e50' }}>Q{index + 1}.</span>
                      <span style={{ color: '#34495e' }}>{q.text}</span>
                    </label>
                    <span style={modalStyles.typeBadge}>{questionTypeLabel(q.type)}</span>
                  </div>
                  {q.answers.length > 0 && (
                    <div style={modalStyles.answersList}>
                      {q.answers.map((a, aIdx) => (
                        <div key={aIdx} style={{
                          ...modalStyles.answerItem,
                          backgroundColor: a.isCorrect ? '#d4edda' : '#f8f9fa',
                          borderColor: a.isCorrect ? '#28a745' : '#dee2e6',
                        }}>
                          <span style={{ fontWeight: 500 }}>
                            {String.fromCharCode(65 + aIdx)}.
                          </span>
                          {' '}{a.text}
                          {a.isCorrect && <span style={{ color: '#28a745', fontWeight: 700, marginLeft: '0.5rem' }}>✓</span>}
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>

            <div style={modalStyles.footer}>
              <button onClick={() => { setStep('upload'); setParsedQuestions([]); }} style={modalStyles.backButton}>
                ← Back
              </button>
              <button
                onClick={handleImport}
                disabled={selectedCount === 0 || isImporting}
                style={{
                  ...modalStyles.importButton,
                  opacity: selectedCount === 0 || isImporting ? 0.5 : 1,
                }}
              >
                {isImporting ? 'Importing...' : `Import ${selectedCount} Question${selectedCount !== 1 ? 's' : ''}`}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

const modalStyles: Record<string, React.CSSProperties> = {
  overlay: {
    position: 'fixed',
    top: 0, left: 0, right: 0, bottom: 0,
    backgroundColor: 'rgba(0,0,0,0.5)',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000,
    padding: '1rem',
  },
  modal: {
    backgroundColor: '#fff',
    borderRadius: '12px',
    width: '100%',
    maxWidth: '750px',
    maxHeight: '85vh',
    display: 'flex',
    flexDirection: 'column',
    boxShadow: '0 20px 60px rgba(0,0,0,0.2)',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '1.25rem 1.5rem',
    borderBottom: '1px solid #e1e8ed',
  },
  headerTitle: {
    margin: 0,
    fontSize: '1.25rem',
    fontWeight: 700,
    color: '#2c3e50',
  },
  closeButton: {
    background: 'none',
    border: 'none',
    fontSize: '1.25rem',
    cursor: 'pointer',
    color: '#6c757d',
    padding: '0.25rem 0.5rem',
    borderRadius: '4px',
  },
  uploadArea: {
    padding: '2rem 1.5rem',
  },
  dropZone: {
    border: '2px dashed #3498db',
    borderRadius: '12px',
    padding: '2.5rem 2rem',
    textAlign: 'center' as const,
    cursor: 'pointer',
    transition: 'all 0.2s',
    backgroundColor: '#f8fbff',
  },
  errorBox: {
    marginTop: '1rem',
    padding: '0.75rem 1rem',
    backgroundColor: '#f8d7da',
    color: '#721c24',
    borderRadius: '8px',
    fontSize: '0.875rem',
  },
  previewArea: {
    flex: 1,
    overflow: 'hidden',
    display: 'flex',
    flexDirection: 'column' as const,
  },
  selectionBar: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '0.75rem 1.5rem',
    borderBottom: '1px solid #e1e8ed',
    backgroundColor: '#f8f9fa',
  },
  selectionButton: {
    padding: '0.25rem 0.75rem',
    fontSize: '0.8rem',
    border: '1px solid #d1d5db',
    borderRadius: '4px',
    backgroundColor: '#fff',
    cursor: 'pointer',
    color: '#374151',
  },
  questionsList: {
    flex: 1,
    overflowY: 'auto' as const,
    padding: '1rem 1.5rem',
  },
  questionItem: {
    border: '1px solid #e1e8ed',
    borderRadius: '8px',
    padding: '1rem',
    marginBottom: '0.75rem',
    transition: 'all 0.2s',
  },
  questionHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    gap: '0.75rem',
  },
  typeBadge: {
    fontSize: '0.7rem',
    fontWeight: 600,
    padding: '0.2rem 0.5rem',
    borderRadius: '4px',
    backgroundColor: '#e8f4fd',
    color: '#2980b9',
    whiteSpace: 'nowrap' as const,
  },
  answersList: {
    marginTop: '0.75rem',
    marginLeft: '2rem',
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '0.25rem',
  },
  answerItem: {
    padding: '0.375rem 0.75rem',
    borderRadius: '4px',
    fontSize: '0.875rem',
    border: '1px solid',
  },
  footer: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '1rem 1.5rem',
    borderTop: '1px solid #e1e8ed',
  },
  backButton: {
    padding: '0.5rem 1rem',
    border: '1px solid #d1d5db',
    borderRadius: '8px',
    backgroundColor: '#fff',
    cursor: 'pointer',
    fontSize: '0.875rem',
    color: '#374151',
  },
  importButton: {
    padding: '0.5rem 1.5rem',
    border: 'none',
    borderRadius: '8px',
    backgroundColor: '#28a745',
    color: '#fff',
    cursor: 'pointer',
    fontSize: '0.875rem',
    fontWeight: 600,
  },
};
