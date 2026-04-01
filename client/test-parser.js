const QuestionType = {
  SingleChoice: 'SingleChoice',
  MultipleChoice: 'MultipleChoice',
  FreeText: 'FreeText'
};

function finalizeQuestion(q) {
  if (q.answers.length === 0) {
    q.type = QuestionType.FreeText;
  } else {
    if (!q.answers.some(a => a.isCorrect)) {
      q.answers[0].isCorrect = true;
    }
    const correctCount = q.answers.filter(a => a.isCorrect).length;
    q.type = correctCount > 1 ? QuestionType.MultipleChoice : QuestionType.SingleChoice;
  }
}

function parseQuestionsFromText(rawText) {
  const rawLines = rawText.split('\n').map(l => l.trim()).filter(l => l.length > 0);
  const questions = [];
  let currentQuestion = null;

  for (const rawLine of rawLines) {
    const parts = rawLine.split(/(?=\s+[a-zA-Z][.)\]](?:\s+|$))/).map(p => p.trim()).filter(p => p.length > 0);

    for (const line of parts) {
      const optionMatch = line.match(/^[a-zA-Z][.)\]]\s*(.+)/);

      if (optionMatch) {
         if (!currentQuestion) {
             // Ignore options before any question is found
             continue;
         }
         let answerText = optionMatch[1].trim();
         let isCorrect = false;
         if (answerText.endsWith('*') || answerText.endsWith('**')) {
           isCorrect = true;
           answerText = answerText.replace(/\*+$/, '').trim();
         }
         if (/\(correct\)/i.test(answerText)) {
           isCorrect = true;
           answerText = answerText.replace(/\s*\(correct\)\s*/i, '').trim();
         }
         currentQuestion.answers.push({ text: answerText, isCorrect });
      } else {
         const numberMatch = line.match(/^\d+[.)]\s+(.+)/);

         if (numberMatch || (currentQuestion && currentQuestion.answers.length > 0)) {
           if (currentQuestion) {
             finalizeQuestion(currentQuestion);
             questions.push(currentQuestion);
           }
           currentQuestion = {
             text: numberMatch ? numberMatch[1].trim() : line.trim(),
             type: QuestionType.SingleChoice,
             answers: [],
             selected: true
           };
         } else if (!currentQuestion) {
           currentQuestion = {
             text: line.trim(),
             type: QuestionType.SingleChoice,
             answers: [],
             selected: true
           };
         } else {
           currentQuestion.text += ' ' + line.trim();
         }
      }
    }
  }

  if (currentQuestion) {
    finalizeQuestion(currentQuestion);
    questions.push(currentQuestion);
  }

  return questions;
}

// Test case 1: Manual numbering (what my previous code expected)
const text1 = `
1. What is 1+1? a) 1 b) 2*
2. What is 2+2? a) 3 b) 4*
`;
console.log("Test 1 (Manual):", parseQuestionsFromText(text1).length === 2 ? "PASS" : "FAIL");

// Test case 2: Automatic numbering stripped by Mammoth
const text2 = `
What is 1+1? a) 1 b) 2*
What is 2+2? a) 3 b) 4*
`;
console.log("Test 2 (Auto stripped):", parseQuestionsFromText(text2).length === 2 ? "PASS" : "FAIL");

// Test case 3: Multi-line question with auto numbering
const text3 = `
Solve the following equation.
It is very hard.
a) 1
b) 2*
What is the square root of 9?
a) 2
b) 3*
`;
console.log("Test 3 (Multi-line auto):", parseQuestionsFromText(text3)[0].text === "Solve the following equation. It is very hard." ? "PASS" : "FAIL");
