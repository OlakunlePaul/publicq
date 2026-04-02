import { useState, useRef, useCallback } from 'react';
import mammoth from 'mammoth';
import { v4 as uuidv4 } from 'uuid';
import { QuestionCreateDto, PossibleAnswerCreateDto } from '../../models/assessment-modules-create';
import { QuestionType } from '../../models/question-types';
import { 
  FileText, 
  Upload, 
  Image as ImageIcon, 
  Trash2, 
  CheckCircle, 
  RefreshCcw, 
  X, 
  Plus, 
  Check,
  AlertCircle
} from 'lucide-react';

interface ParsedQuestion {
  id: string;
  text: string;
  type: QuestionType;
  answers: { text: string; isCorrect: boolean }[];
  selected: boolean;
  images: File[];
  originalNumber?: string;
}

interface Props {
  moduleId: string;
  moduleVersionId: string;
  startingOrder: number;
  onImport: (questions: QuestionCreateDto[]) => void;
  onClose: () => void;
}

/**
 * REFINED LINEAR GREEDY PARSER
 * Optimized for complex math questions and same-line options.
 */
function parseQuestionsFromHtml(html: string, imageMap: Record<string, File>): ParsedQuestion[] {
  const parser = new DOMParser();
  const doc = parser.parseFromString(html, 'text/html');
  
  const questions: ParsedQuestion[] = [];
  let currentQuestion: ParsedQuestion | null = null;
  let pendingContextText = '';
  let pendingContextImages: File[] = [];

  // 1. Get structural blocks while preventing duplication
  const allBlocks: Element[] = Array.from(doc.querySelectorAll('p, li, tr, td, h1, h2, h3, h4, h5, h6'));
  const blocks = allBlocks.filter(b => !b.parentElement?.closest('p, li, tr, td, h1, h2, h3, h4, h5, h6'));
  
  // 2. Precise Markers for Questions and Options
  // Supports "1. ", "1)", "a) ", "A. ", "(A) "
  const splitRegex = /(?=\s+(?:[a-eA-E][.)\]]|\d+[.)])\s+)/;

  for (const block of blocks) {
    const imagesInBlock = Array.from(block.querySelectorAll('img')).map(img => {
      const src = img.getAttribute('src');
      return src && imageMap[src] ? imageMap[src] : null;
    }).filter((f): f is File => f !== null);

    const blockText = block.textContent?.trim() || '';
    if (!blockText && imagesInBlock.length === 0) continue;

    // Check if block contains any structural markers
    const hasAnyMarker = /^(?:\d+[.)]|[a-eA-E][.)\]])/.test(blockText) || blockText.match(/\s+(?:[a-eA-E][.)\]]|\d+[.)])\s+/);

    if (!hasAnyMarker) {
      if (currentQuestion) {
        currentQuestion.text += '\n' + blockText;
        currentQuestion.images.push(...imagesInBlock);
      } else {
        pendingContextText += (pendingContextText ? '\n' : '') + blockText;
        pendingContextImages.push(...imagesInBlock);
      }
      continue;
    }

    // Split greedy block into segments (Question or Options)
    const segments = blockText.split(splitRegex).map(s => s.trim()).filter(s => s.length > 0);

    for (const segment of segments) {
      const questionMatch = segment.match(/^(\d+)[.)]\s*([\s\S]*)/);
      const optionMatch = segment.match(/^([a-eA-E])[.)\]]\s*([\s\S]*)/);

      if (questionMatch) {
        if (currentQuestion) questions.push(finalizeQuestion(currentQuestion));

        const questionText = questionMatch[2].trim();
        currentQuestion = {
          id: uuidv4(),
          text: (pendingContextText ? pendingContextText + '\n\n' : '') + questionText,
          type: QuestionType.SingleChoice,
          answers: [],
          selected: true,
          images: [...pendingContextImages, ...imagesInBlock],
          originalNumber: questionMatch[1]
        };
        pendingContextText = '';
        pendingContextImages = [];
        imagesInBlock.length = 0; // consumed
      } else if (optionMatch && currentQuestion) {
        let answerText = optionMatch[2].trim();
        let isCorrect = false;

        // Detection for correct answer markers (* or (correct))
        if (answerText.endsWith('*') || answerText.endsWith('**')) {
          isCorrect = true;
          answerText = answerText.replace(/\*+$/, '').trim();
        }
        if (/\(correct\)/i.test(answerText)) {
          isCorrect = true;
          answerText = answerText.replace(/\s*\(correct\)\s*/i, '').trim();
        }
        currentQuestion.answers.push({ text: answerText, isCorrect });
      } else if (currentQuestion) {
        // Greedy fallback: append to current context
        if (currentQuestion.answers.length > 0) {
          currentQuestion.answers[currentQuestion.answers.length - 1].text += ' ' + segment;
        } else {
          currentQuestion.text += '\n' + segment;
        }
      }
    }
  }

  if (currentQuestion) questions.push(finalizeQuestion(currentQuestion));
  return questions;
}

function finalizeQuestion(q: ParsedQuestion): ParsedQuestion {
  if (q.answers.length === 0) {
    q.type = QuestionType.FreeText;
  } else {
    if (!q.answers.some(a => a.isCorrect)) q.answers[0].isCorrect = true;
    const correctCount = q.answers.filter(a => a.isCorrect).length;
    q.type = correctCount > 1 ? QuestionType.MultipleChoice : QuestionType.SingleChoice;
  }
  return q;
}

export const WordImportModal = ({ moduleId, moduleVersionId, startingOrder, onImport, onClose }: Props) => {
  const [step, setStep] = useState<'upload' | 'preview'>('upload');
  const [questions, setQuestions] = useState<ParsedQuestion[]>([]);
  const [isParsing, setIsParsing] = useState(false);
  const [parseError, setParseError] = useState<string | null>(null);
  const [isImporting, setIsImporting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const manualImageInputRef = useRef<HTMLInputElement>(null);
  const [activeQIndex, setActiveQIndex] = useState<number | null>(null);

  const handleFileUpload = useCallback(async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!file.name.endsWith('.docx')) {
      setParseError('Please select a valid .docx (Word) file.');
      return;
    }

    setIsParsing(true);
    setParseError(null);

    try {
      const arrayBuffer = await file.arrayBuffer();
      const imageMap: Record<string, File> = {};

      const options = {
        convertImage: mammoth.images.imgElement((image) => {
          return image.read().then(async (imageBuffer) => {
            const fileId = `img-${uuidv4()}`;
            const blob = new Blob([imageBuffer as any], { type: image.contentType });
            imageMap[fileId] = new File([blob], `${fileId}.png`, { type: image.contentType });
            return { src: fileId };
          });
        })
      };

      const result = await mammoth.convertToHtml({ arrayBuffer }, options);
      const parsed = parseQuestionsFromHtml(result.value, imageMap);

      if (parsed.length === 0) {
        setParseError('No questions identified. Ensure they start with "1. ", "2. ", etc.');
        setIsParsing(false);
        return;
      }

      setQuestions(parsed);
      setStep('preview');
    } catch (err) {
      console.error(err);
      setParseError('Failed to parse document. Check the file format.');
    } finally {
      setIsParsing(false);
    }
  }, []);

  const handleManualImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file && activeQIndex !== null) {
      setQuestions(prev => prev.map((q, i) => i === activeQIndex ? { ...q, images: [...q.images, file] } : q));
    }
    setActiveQIndex(null);
    if (e.target) e.target.value = '';
  };

  const removeImage = (qIdx: number, imgIdx: number) => {
    setQuestions(prev => prev.map((q, i) => i === qIdx ? { ...q, images: q.images.filter((_, idx) => idx !== imgIdx) } : q));
  };

  const updateText = (idx: number, val: string) => {
    setQuestions(prev => prev.map((q, i) => i === idx ? { ...q, text: val } : q));
  };

  const toggleSelection = (idx: number) => {
    setQuestions(prev => prev.map((q, i) => i === idx ? { ...q, selected: !q.selected } : q));
  };

  const executeImport = async () => {
    setIsImporting(true);
    const selected = questions.filter(q => q.selected);
    onImport(selected.map((q, i) => ({
      internalId: q.id,
      order: startingOrder + i,
      moduleId,
      moduleVersionId,
      text: q.text,
      staticFileIds: [],
      extractedFiles: q.images,
      type: q.type,
      answers: q.answers.map((a, ai) => ({
        text: a.text,
        order: ai,
        staticFileIds: [],
        isCorrect: a.isCorrect,
      })),
    })));
    setIsImporting(false);
  };

  const selectedCount = questions.filter(q => q.selected).length;

  return (
    <div style={styles.overlay} onClick={onClose}>
      <div style={styles.container} onClick={e => e.stopPropagation()}>
        <div style={styles.header}>
          <div style={styles.headerLead}>
            <div style={styles.iconBox}><FileText size={20} color="#3182ce" /></div>
            <h2 style={styles.title}>{step === 'upload' ? 'Word Document Import' : `Review Questions (${selectedCount})`}</h2>
          </div>
          <button style={styles.closeBtn} onClick={onClose}><X size={20} /></button>
        </div>

        {step === 'upload' ? (
          <div style={styles.body}>
            <div style={styles.dropzone} onClick={() => fileInputRef.current?.click()}>
              <div style={styles.uploadIcon}><Upload size={40} color="#4a5568" /></div>
              <p style={styles.dropTitle}>Select Question Document</p>
              <p style={styles.dropSub}>Questions should be numbered (1.) with options (a.)</p>
              <input ref={fileInputRef} type="file" accept=".docx" onChange={handleFileUpload} style={{ display: 'none' }} />
            </div>
            {isParsing && <div style={styles.loader}><RefreshCcw className="animateSpin" size={18} /> Analyzing questions...</div>}
            {parseError && <div style={styles.error}><AlertCircle size={16} /> {parseError}</div>}
          </div>
        ) : (
          <div style={styles.previewArea}>
            <input ref={manualImageInputRef} type="file" accept="image/*" style={{ display: 'none' }} onChange={handleManualImage} />
            <div style={styles.toolbar}>
              <p style={styles.toolHint}>You can edit question text or add images manually below.</p>
              <div style={styles.toolActions}>
                <button style={styles.miniBtn} onClick={() => setQuestions(q => q.map(it => ({ ...it, selected: true })))}>All</button>
                <button style={styles.miniBtn} onClick={() => setQuestions(q => q.map(it => ({ ...it, selected: false })))}>None</button>
              </div>
            </div>

            <div style={styles.scrollList}>
              {questions.map((q, idx) => (
                <div key={q.id} style={{ ...styles.card, opacity: q.selected ? 1 : 0.6, borderColor: q.selected ? '#e2e8f0' : '#f7fafc' }}>
                  <div style={styles.cardHeader}>
                    <div style={styles.qIndex}>
                      <input type="checkbox" checked={q.selected} onChange={() => toggleSelection(idx)} style={styles.checkbox} />
                      <span style={styles.qNum}>Question {idx + 1}</span>
                    </div>
                    <button style={styles.addImgBtn} onClick={() => { setActiveQIndex(idx); manualImageInputRef.current?.click(); }}>
                      <ImageIcon size={14} /> Add Graph
                    </button>
                  </div>
                  
                  <textarea 
                    style={styles.textEdit} 
                    value={q.text} 
                    onChange={e => updateText(idx, e.target.value)}
                    rows={Math.max(2, q.text.split('\n').length)}
                  />

                  {q.images.length > 0 && (
                    <div style={styles.imageGrid}>
                      {q.images.map((img, iidx) => (
                        <div key={iidx} style={styles.imgWrap}>
                          <img src={URL.createObjectURL(img)} alt="q-img" style={styles.img} />
                          <button style={styles.imgDel} onClick={() => removeImage(idx, iidx)}><Trash2 size={12} /></button>
                        </div>
                      ))}
                    </div>
                  )}

                  <div style={styles.answerGrid}>
                    {q.answers.map((a, aidx) => (
                      <div key={aidx} style={{...styles.ansRow, background: a.isCorrect ? '#f0fff4' : '#f8fafc', color: a.isCorrect ? '#2f855a' : '#4a5568'}}>
                        <div style={styles.ansToken}>{String.fromCharCode(65 + aidx)}</div>
                        <span style={styles.ansText}>{a.text}</span>
                        {a.isCorrect && <Check size={14} style={{marginLeft: 'auto'}} />}
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>

            <div style={styles.footer}>
              <button style={styles.backBtn} onClick={() => { setStep('upload'); setQuestions([]); }}>Back</button>
              <button style={styles.importBtn} onClick={executeImport} disabled={selectedCount === 0 || isImporting}>
                {isImporting ? 'Importing...' : `Confirm Import (${selectedCount})`}
              </button>
            </div>
          </div>
        )}
      </div>
      <style>{`
        @keyframes animateSpin {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
        .animateSpin {
          animation: animateSpin 1s linear infinite;
        }
      `}</style>
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  overlay: { position: 'fixed', inset: 0, background: 'rgba(26, 32, 44, 0.75)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000, padding: '1rem', backdropFilter: 'blur(4px)' },
  container: { background: '#fff', borderRadius: '1.25rem', width: '100%', maxWidth: '850px', maxHeight: '90vh', display: 'flex', flexDirection: 'column', boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.25)', overflow: 'hidden' },
  header: { padding: '1.5rem 2rem', borderBottom: '1px solid #edf2f7', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: '#fff' },
  headerLead: { display: 'flex', alignItems: 'center', gap: '1rem' },
  iconBox: { width: '40px', height: '40px', background: '#ebf8ff', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center' },
  title: { margin: 0, fontSize: '1.25rem', fontWeight: 800, color: '#1a202c' },
  closeBtn: { background: 'none', border: 'none', cursor: 'pointer', color: '#a0aec0', padding: '0.5rem' },
  body: { padding: '4rem 3rem' },
  dropzone: { border: '2px dashed #e2e8f0', borderRadius: '24px', padding: '5rem 2rem', textAlign: 'center', cursor: 'pointer', transition: 'all 0.2s', background: '#fafcff' },
  uploadIcon: { marginBottom: '1.5rem', display: 'flex', justifyContent: 'center' },
  dropTitle: { fontSize: '1.1rem', fontWeight: 700, color: '#2d3748', margin: '0 0 0.5rem' },
  dropSub: { fontSize: '0.875rem', color: '#718096', margin: 0 },
  loader: { textAlign: 'center', marginTop: '2rem', color: '#3182ce', fontWeight: 600, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' },
  error: { marginTop: '1.5rem', padding: '1rem', background: '#fff5f5', color: '#c53030', borderRadius: '12px', border: '1px solid #feb2b2', fontSize: '0.9rem', display: 'flex', alignItems: 'center', gap: '0.5rem' },
  previewArea: { flex: 1, overflow: 'hidden', display: 'flex', flexDirection: 'column' },
  toolbar: { padding: '1rem 2rem', background: '#f7fafc', borderBottom: '1px solid #edf2f7', display: 'flex', justifyContent: 'space-between', alignItems: 'center' },
  toolHint: { fontSize: '0.85rem', color: '#718096', margin: 0 },
  toolActions: { display: 'flex', gap: '0.5rem' },
  miniBtn: { padding: '0.4rem 0.8rem', fontSize: '0.75rem', fontWeight: 700, borderRadius: '8px', border: '1px solid #e2e8f0', background: '#fff', cursor: 'pointer', color: '#4a5568' },
  scrollList: { flex: 1, overflowY: 'auto', padding: '2rem', background: '#fff' },
  card: { border: '1px solid #edf2f7', borderRadius: '1.25rem', padding: '1.5rem', marginBottom: '1.5rem', background: '#fff', transition: 'all 0.2s' },
  cardHeader: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' },
  qIndex: { display: 'flex', alignItems: 'center', gap: '0.75rem' },
  checkbox: { width: '18px', height: '18px', borderRadius: '4px', cursor: 'pointer' },
  qNum: { fontWeight: 800, color: '#1a202c', fontSize: '1rem' },
  addImgBtn: { padding: '0.4rem 0.9rem', fontSize: '0.75rem', fontWeight: 700, background: '#f0f7ff', color: '#2b6cb0', border: 'none', borderRadius: '10px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '0.5rem' },
  textEdit: { width: '100%', padding: '1.25rem', borderRadius: '16px', border: '1px solid #e2e8f0', fontSize: '1rem', lineHeight: '1.7', fontFamily: 'inherit', resize: 'vertical', background: '#fcfcfc', color: '#2d3748', outline: 'none' },
  imageGrid: { display: 'flex', flexWrap: 'wrap', gap: '1rem', padding: '1rem 0' },
  imgWrap: { position: 'relative', width: '120px', height: '120px', borderRadius: '14px', border: '1px solid #edf2f7', overflow: 'hidden', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)' },
  img: { width: '100%', height: '100%', objectFit: 'cover' },
  imgDel: { position: 'absolute', top: '6px', right: '6px', background: 'rgba(255,255,255,0.9)', border: 'none', borderRadius: '8px', width: '24px', height: '24px', cursor: 'pointer', color: '#e53e3e', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' },
  answerGrid: { display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '0.75rem', marginTop: '1rem' },
  ansRow: { padding: '1rem', borderRadius: '12px', display: 'flex', alignItems: 'center', gap: '0.75rem', fontSize: '0.9rem', border: '1px solid transparent' },
  ansToken: { width: '24px', height: '24px', borderRadius: '6px', background: 'rgba(0,0,0,0.05)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 800, fontSize: '0.75rem' },
  ansText: { fontWeight: 500 },
  footer: { padding: '1.5rem 2rem', borderTop: '1px solid #edf2f7', display: 'flex', justifyContent: 'space-between', background: '#fff' },
  backBtn: { padding: '0.75rem 1.75rem', borderRadius: '14px', border: '1px solid #e2e8f0', background: '#fff', fontWeight: 700, cursor: 'pointer', color: '#4a5568' },
  importBtn: { padding: '0.75rem 2.5rem', borderRadius: '14px', border: 'none', background: '#3182ce', color: '#fff', fontWeight: 800, cursor: 'pointer', boxShadow: '0 10px 15px -3px rgba(49, 130, 206, 0.4)' }
};
