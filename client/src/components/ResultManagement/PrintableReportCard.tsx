import React, { useRef, useEffect, useState } from 'react';
import { AssessmentDetailsDto, TermDto, SessionDto } from '../../models/academic';
import { configurationService } from '../../services/configurationService';
import { SchoolBrandingConfiguration } from '../../models/school-branding-configuration';
import api from '../../api/axios';

interface PrintableReportCardProps {
  report: AssessmentDetailsDto;
  termInfo?: TermDto;
  sessionInfo?: SessionDto;
  onClose: () => void;
}

const PrintableReportCard: React.FC<PrintableReportCardProps> = ({ report, termInfo, sessionInfo, onClose }) => {
  const printRef = useRef<HTMLDivElement>(null);

  const [branding, setBranding] = useState<SchoolBrandingConfiguration | null>(null);
  const baseURL = api.defaults.baseURL?.replace(/\/api\/?$/, '') || '';

  useEffect(() => {
    // Fetch branding data
    configurationService.getSchoolBrandingConfiguration().then(res => {
        if (res.isSuccess && res.data) {
            setBranding(res.data);
        }
    }).catch(console.error);

    // Trigger print automatically after a short delay to ensure rendering
    const timer = setTimeout(() => {
      window.print();
    }, 800);
    return () => clearTimeout(timer);
  }, []);

  // Helper styles specifically for the print layout
  const tableBorder = '1px solid #000';
  
  return (
    <div style={overlayStyle}>
      <style>
        {`
          @media print {
            body * {
              visibility: hidden;
            }
            #printable-report-card, #printable-report-card * {
              visibility: visible;
            }
            #printable-report-card {
              position: absolute;
              left: 0;
              top: 0;
              width: 100%;
              padding: 10mm;
              margin: 0;
              box-sizing: border-box;
            }
            .no-print {
              display: none !important;
            }
            table {
              page-break-inside: avoid;
            }
            @page {
              size: A4;
              margin: 10mm;
            }
          }
        `}
      </style>

      <div className="no-print" style={controlsStyle}>
        <button onClick={() => window.print()} style={btnPrimary}>Print</button>
        <button onClick={onClose} style={btnSecondary}>Close</button>
      </div>

      <div id="printable-report-card" ref={printRef} style={pageStyle}>
        {/* Header Section */}
        <div style={{ textAlign: 'center', marginBottom: '20px', borderBottom: '3px solid #000', paddingBottom: '10px' }}>
          {branding?.schoolLogoUrl && (
            <img 
              src={branding.schoolLogoUrl.startsWith('http') ? branding.schoolLogoUrl : `${baseURL}/${branding.schoolLogoUrl}`} 
              alt="School Logo" 
              style={{ maxHeight: '80px', marginBottom: '10px' }} 
            />
          )}
          <div style={{ fontSize: '32px', fontWeight: 'bold', textTransform: 'uppercase', color: '#1e3a8a' }}>
            {branding?.schoolName || 'Day & Boarding School'}
          </div>
          <div style={{ fontSize: '14px', marginTop: '4px' }}>
            {branding?.schoolAddress || '123 Education Avenue, Knowledge City.'} Tel: {branding?.schoolPhone || '0800-SCHOOL-123'}
          </div>
          <div style={{ fontSize: '20px', fontWeight: 'bold', marginTop: '10px', textTransform: 'uppercase' }}>
            {termInfo?.name || 'Academic Term'} Report
          </div>
          <div style={{ fontSize: '14px', fontWeight: 'bold', marginTop: '4px' }}>
            {sessionInfo?.name || 'Academic Session'}
          </div>
        </div>

        {/* Student Info Section */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', marginBottom: '20px' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <tbody>
              <tr>
                <td style={{ ...tdInfoStyle, width: '40%' }}>Student Name:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000', fontWeight: 'bold' }}>{report.studentName}</td>
              </tr>
              <tr>
                <td style={tdInfoStyle}>Admission No:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000' }}>{report.admissionNumber || 'N/A'}</td>
              </tr>
              <tr>
                <td style={tdInfoStyle}>Class:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000' }}>{report.className || 'N/A'}</td>
              </tr>
            </tbody>
          </table>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <tbody>
              <tr>
                <td style={{ ...tdInfoStyle, width: '50%' }}>Number in Class:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000' }}>{report.numberInClass || '-'}</td>
              </tr>
              <tr>
                <td style={tdInfoStyle}>Total Marks:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000' }}>{report.totalMarksObtained} / {report.totalMarksObtainable}</td>
              </tr>
              <tr>
                <td style={tdInfoStyle}>Final Average:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000', fontWeight: 'bold' }}>{report.averageScore?.toFixed(2) || '-'}%</td>
              </tr>
              <tr>
                <td style={tdInfoStyle}>Position/Grade:</td>
                <td style={{ ...tdInfoStyle, borderBottom: '1px solid #000', fontWeight: 'bold' }}>
                  {report.positionInClass ? `${report.positionInClass} / ` : '- / '} 
                  {report.overallGrade || '-'}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        {/* Academic Performance */}
        <div style={{ marginBottom: '20px' }}>
          <div style={sectionTitleStyle}>Academic Performance</div>
          <table style={{ width: '100%', borderCollapse: 'collapse', border: tableBorder }}>
            <thead>
              <tr style={{ backgroundColor: '#f3f4f6' }}>
                <th style={thStyle}>Subject</th>
                <th style={thStyle}>Test Score (40)</th>
                <th style={thStyle}>Exam Score (60)</th>
                <th style={thStyle}>Total (100)</th>
                <th style={thStyle}>Grade</th>
                <th style={thStyle}>Remark</th>
              </tr>
            </thead>
            <tbody>
              {report.subjectScores?.map((score, idx) => {
                const test = score.testScore || 0;
                const exam = score.examScore || 0;
                const total = test + exam;
                let grade = 'F9';
                if (total >= 75) grade = 'A1';
                else if (total >= 70) grade = 'B2';
                else if (total >= 65) grade = 'B3';
                else if (total >= 60) grade = 'C4';
                else if (total >= 50) grade = 'C6';
                else if (total >= 40) grade = 'E8';

                return (
                  <tr key={idx}>
                    <td style={tdStyle}>{score.subjectName || score.subjectRemark || 'Subject'}</td>
                    <td style={{...tdStyle, textAlign: 'center'}}>{score.testScore ?? '-'}</td>
                    <td style={{...tdStyle, textAlign: 'center'}}>{score.examScore ?? '-'}</td>
                    <td style={{...tdStyle, textAlign: 'center', fontWeight: 'bold'}}>{total}</td>
                    <td style={{...tdStyle, textAlign: 'center'}}>{grade}</td>
                    <td style={tdStyle}>{score.subjectRemark || '-'}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>

        {/* Traits and Attendance Row */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '15px', marginBottom: '20px' }}>
          {/* Attendance */}
          <div>
            <div style={sectionTitleStyle}>Attendance</div>
            <table style={{ width: '100%', borderCollapse: 'collapse', border: tableBorder }}>
              <tbody>
                <tr><td style={tdTraitStyle}>Times School Opened</td><td style={{...tdTraitStyle, textAlign: 'center', width: '40px'}}>{report.timesSchoolOpened ?? '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Times Present</td><td style={{...tdTraitStyle, textAlign: 'center'}}>{report.timesPresent ?? '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Times Absent</td><td style={{...tdTraitStyle, textAlign: 'center'}}>{report.timesAbsent ?? '-'}</td></tr>
              </tbody>
            </table>
          </div>

          {/* Affective Domain */}
          <div>
            <div style={sectionTitleStyle}>Affective Domain</div>
            <table style={{ width: '100%', borderCollapse: 'collapse', border: tableBorder }}>
              <tbody>
                <tr><td style={tdTraitStyle}>Regularity</td><td style={{...tdTraitStyle, textAlign: 'center', width: '40px', fontWeight: 'bold'}}>{report.regularity || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Punctuality</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.punctuality || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Neatness</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.neatness || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Attitude in School</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.attitudeInSchool || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Social Activities</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.socialActivities || '-'}</td></tr>
              </tbody>
            </table>
          </div>

          {/* Psychomotor  */}
          <div>
            <div style={sectionTitleStyle}>Psychomotor Skills</div>
            <table style={{ width: '100%', borderCollapse: 'collapse', border: tableBorder }}>
              <tbody>
                <tr><td style={tdTraitStyle}>Indoor Games</td><td style={{...tdTraitStyle, textAlign: 'center', width: '40px', fontWeight: 'bold'}}>{report.indoorGames || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Field Games</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.fieldGames || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Track Games</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.trackGames || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Jumps</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.jumps || '-'}</td></tr>
                <tr><td style={tdTraitStyle}>Swims</td><td style={{...tdTraitStyle, textAlign: 'center', fontWeight: 'bold'}}>{report.swims || '-'}</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        {/* Comments Footer */}
        <div style={{ marginTop: '30px' }}>
          <div style={{ marginBottom: '15px' }}>
            <span style={{ fontWeight: 'bold' }}>Class Teacher's Comment: </span>
            <span style={{ fontStyle: 'italic', borderBottom: '1px dotted #000', display: 'inline-block', width: 'calc(100% - 180px)' }}>
              {report.classTeacherComment || '.........................................................................................'}
            </span>
          </div>
          <div style={{ marginBottom: '15px' }}>
            <span style={{ fontWeight: 'bold' }}>Head Teacher's Comment: </span>
            <span style={{ fontStyle: 'italic', borderBottom: '1px dotted #000', display: 'inline-block', width: 'calc(100% - 180px)' }}>
              {report.headTeacherComment || '.........................................................................................'}
            </span>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', marginTop: '20px' }}>
            <div>
              <span style={{ fontWeight: 'bold' }}>Next Term Begins: </span>
              <span style={{ fontStyle: 'normal' }}>{termInfo?.nextTermBegins ? new Date(termInfo.nextTermBegins).toLocaleDateString() : '........................'}</span>
            </div>
            {/* Additional info like next term fees could go here */}
          </div>
        </div>
      </div>
    </div>
  );
};

// Styles for the viewing overlay (non-print)
const overlayStyle: React.CSSProperties = {
  position: 'fixed',
  top: 0, left: 0, right: 0, bottom: 0,
  backgroundColor: '#f3f4f6',
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  zIndex: 1500,
  padding: '20px',
  overflowY: 'auto'
};

const controlsStyle: React.CSSProperties = {
  display: 'flex',
  gap: '12px',
  marginBottom: '20px',
  width: '100%',
  maxWidth: '210mm',
  justifyContent: 'flex-end'
};

const btnPrimary: React.CSSProperties = {
  padding: '8px 16px',
  backgroundColor: '#2563eb',
  color: '#fff',
  border: 'none',
  borderRadius: '4px',
  cursor: 'pointer',
  fontWeight: 'bold'
};

const btnSecondary: React.CSSProperties = {
  padding: '8px 16px',
  backgroundColor: '#9ca3af',
  color: '#fff',
  border: 'none',
  borderRadius: '4px',
  cursor: 'pointer',
  fontWeight: 'bold'
};

// Layout matching A4 dimensions standard
const pageStyle: React.CSSProperties = {
  backgroundColor: '#fff',
  width: '210mm', // Standard A4 width
  padding: '15mm', // Standard margins
  boxSizing: 'border-box',
  boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
  fontFamily: '"Times New Roman", Times, serif', // Report cards often use serif fonts
  color: '#000',
  position: 'relative'
};

const sectionTitleStyle: React.CSSProperties = {
  fontWeight: 'bold',
  textTransform: 'uppercase',
  backgroundColor: '#f3f4f6',
  padding: '4px 8px',
  border: '1px solid #000',
  borderBottom: 'none',
  fontSize: '12px'
};

const thStyle: React.CSSProperties = {
  border: '1px solid #000',
  padding: '8px',
  textAlign: 'left',
  fontWeight: 'bold',
  fontSize: '12px',
  textTransform: 'uppercase'
};

const tdStyle: React.CSSProperties = {
  border: '1px solid #000',
  padding: '6px 8px',
  fontSize: '13px'
};

const tdInfoStyle: React.CSSProperties = {
  padding: '4px 0',
  fontSize: '14px'
};

const tdTraitStyle: React.CSSProperties = {
  border: '1px solid #000',
  padding: '4px 8px',
  fontSize: '12px'
};

export default PrintableReportCard;
