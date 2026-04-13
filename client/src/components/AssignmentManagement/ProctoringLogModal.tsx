import React, { useEffect, useState, useCallback } from 'react';
import { assignmentService } from '../../services/assignmentService';
import cssStyles from './AssignmentManagement.module.css';

interface ProctoringLogEntry {
  timestamp: string;
  level: string;
  message: string;
  userId: string;
}

interface ProctoringLogModalProps {
  isOpen: boolean;
  assignmentId: string;
  assignmentTitle: string;
  onClose: () => void;
}

export const ProctoringLogModal: React.FC<ProctoringLogModalProps> = ({ 
  isOpen, 
  assignmentId, 
  assignmentTitle, 
  onClose 
}) => {
  const [logs, setLogs] = useState<ProctoringLogEntry[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [studentIdFilter, setStudentIdFilter] = useState('');
  const pageSize = 10;

  const loadLogs = useCallback(async (page: number, studentId?: string) => {
    if (!assignmentId) return;
    setLoading(true);
    setError('');
    try {
      const response = await assignmentService.getProctoringLogs(assignmentId, page, pageSize, studentId);
      if (response.isSuccess && response.data) {
        setLogs(response.data.data || []);
        setTotalPages(response.data.totalPages || 1);
        setCurrentPage(page);
      } else {
        setError(response.message || 'Failed to load proctoring logs.');
      }
    } catch (err: any) {
      console.error('Failed to load proctoring logs:', err);
      setError(err.response?.data?.message || 'An error occurred while fetching logs.');
    } finally {
      setLoading(false);
    }
  }, [assignmentId, pageSize]);

  useEffect(() => {
    if (isOpen && assignmentId) {
      loadLogs(1, studentIdFilter);
    }
  }, [isOpen, assignmentId, loadLogs]);

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (isOpen && e.key === 'Escape') {
        e.preventDefault();
        onClose();
      }
    };
    if (isOpen) {
      document.addEventListener('keydown', handleKeyDown);
      return () => document.removeEventListener('keydown', handleKeyDown);
    }
  }, [isOpen, onClose]);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setStudentIdFilter(e.target.value);
  };

  const applyFilter = () => {
    loadLogs(1, studentIdFilter);
  };

  const getLevelBadgeStyle = (level: string) => {
    switch(level.toLowerCase()) {
      case 'critical': return { backgroundColor: '#fee2e2', color: '#dc2626', border: '1px solid #fecaca' };
      case 'warning': return { backgroundColor: '#fef3c7', color: '#d97706', border: '1px solid #fde68a' };
      case 'information':
      default: return { backgroundColor: '#e0f2fe', color: '#0284c7', border: '1px solid #bae6fd' };
    }
  };

  if (!isOpen) return null;

  return (
    <div className={cssStyles.modalOverlay} style={{ zIndex: 1000 }}>
      <div className={cssStyles.modal} style={{ maxWidth: '800px', width: '90%' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', paddingBottom: '1rem', borderBottom: '1px solid #e5e7eb' }}>
          <h3 className={cssStyles.modalTitle} style={{ margin: 0 }}>
            Proctoring Logs: <span style={{ fontWeight: 'normal', color: '#4b5563' }}>{assignmentTitle}</span>
          </h3>
          <button 
            onClick={onClose}
            style={{ background: 'none', border: 'none', fontSize: '1.5rem', cursor: 'pointer', color: '#9ca3af' }}
          >&times;</button>
        </div>

        <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1rem' }}>
          <input 
            type="text" 
            placeholder="Filter by Student ID..." 
            value={studentIdFilter}
            onChange={handleFilterChange}
            className={cssStyles.formInput}
            style={{ maxWidth: '250px' }}
            onKeyDown={(e) => e.key === 'Enter' && applyFilter()}
          />
          <button 
            onClick={applyFilter}
            className={cssStyles.modalConfirmButton}
            style={{ padding: '0.5rem 1rem' }}
          >
            Filter
          </button>
          {studentIdFilter && (
            <button 
              onClick={() => { setStudentIdFilter(''); loadLogs(1, ''); }}
              className={cssStyles.modalCancelButton}
              style={{ padding: '0.5rem 1rem' }}
            >
              Clear
            </button>
          )}
        </div>

        {error && <p className={cssStyles.formError}>{error}</p>}

        <div style={{ overflowX: 'auto', maxHeight: '400px', overflowY: 'auto' }}>
          {loading ? (
            <div style={{ padding: '2rem', textAlign: 'center', color: '#6b7280' }}>Loading logs...</div>
          ) : logs.length === 0 ? (
            <div style={{ padding: '2rem', textAlign: 'center', color: '#6b7280', backgroundColor: '#f9fafb', borderRadius: '0.375rem' }}>
              No proctoring logs found for this assignment.
            </div>
          ) : (
            <table className={cssStyles.userTable} style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead style={{ position: 'sticky', top: 0, backgroundColor: '#f3f4f6', zIndex: 1 }}>
                <tr>
                  <th style={{ padding: '0.75rem', textAlign: 'left', borderBottom: '1px solid #e5e7eb' }}>Timestamp</th>
                  <th style={{ padding: '0.75rem', textAlign: 'left', borderBottom: '1px solid #e5e7eb' }}>Level</th>
                  <th style={{ padding: '0.75rem', textAlign: 'left', borderBottom: '1px solid #e5e7eb' }}>Message</th>
                </tr>
              </thead>
              <tbody>
                {logs.map((log, index) => (
                  <tr key={index} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={{ padding: '0.75rem', whiteSpace: 'nowrap', fontSize: '0.875rem', color: '#4b5563' }}>
                      {new Date(log.timestamp).toLocaleString()}
                    </td>
                    <td style={{ padding: '0.75rem' }}>
                      <span style={{ 
                        padding: '0.25rem 0.5rem', 
                        borderRadius: '9999px', 
                        fontSize: '0.75rem', 
                        fontWeight: 600,
                        ...getLevelBadgeStyle(log.level)
                      }}>
                        {log.level.toUpperCase()}
                      </span>
                    </td>
                    <td style={{ padding: '0.75rem', fontSize: '0.875rem', lineHeight: 1.4 }}>
                      {log.message.replace('[PROCTORING] ', '')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {totalPages > 1 && (
          <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '1rem', marginTop: '1rem', paddingTop: '1rem', borderTop: '1px solid #e5e7eb' }}>
            <button 
              className={cssStyles.paginationButton}
              disabled={currentPage === 1 || loading}
              onClick={() => loadLogs(currentPage - 1, studentIdFilter)}
            >
              Previous
            </button>
            <span style={{ fontSize: '0.875rem', color: '#4b5563' }}>Page {currentPage} of {totalPages}</span>
            <button 
              className={cssStyles.paginationButton}
              disabled={currentPage === totalPages || loading}
              onClick={() => loadLogs(currentPage + 1, studentIdFilter)}
            >
              Next
            </button>
          </div>
        )}

      </div>
    </div>
  );
};
