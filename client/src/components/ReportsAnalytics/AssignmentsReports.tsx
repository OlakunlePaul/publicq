import React, { useState, useEffect } from 'react';
import { reportingService } from '../../services/reportingService';
import { AssignmentSummaryReport } from '../../models/reporting';
import { formatDateToLocal } from '../../utils/dateUtils';
import AssignmentFullReport from '../AssignmentFullReport/AssignmentFullReport';
import cssStyles from './ReportsSummary.module.css';

const AssignmentsReports: React.FC = () => {
  const [assignments, setAssignments] = useState<AssignmentSummaryReport[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedAssignment, setSelectedAssignment] = useState<AssignmentSummaryReport | null>(null);
  const [showFullReport, setShowFullReport] = useState(false);
  const [loadingFullReportAssignmentId, setLoadingFullReportAssignmentId] = useState<string | null>(null);

  const pageSize = 10;

  const fetchAssignments = async (page: number = 1) => {
    try {
      setLoading(true);
      setError('');
      const response = await reportingService.getAllAssignmentsReport(page, pageSize);
      if (response.isSuccess) {
        setAssignments(response.data?.data || []);
        setCurrentPage(page);
        setTotalPages(response.data?.totalPages || 1);
      } else {
        setError('Failed to load assignment reports.');
      }
    } catch (err: any) {
      if (err.response?.status !== 404) {
        setError('An error occurred while loading reports.');
      }
      setAssignments([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAssignments(1);
  }, []);

  const handlePageChange = (page: number) => {
    if (page >= 1 && page <= totalPages) {
      fetchAssignments(page);
    }
  };

  const handleViewFullReport = (assignment: AssignmentSummaryReport) => {
    setLoadingFullReportAssignmentId(assignment.assignmentId);
    setTimeout(() => {
      setSelectedAssignment(assignment);
      setShowFullReport(true);
      setLoadingFullReportAssignmentId(null);
    }, 100);
  };

  if (selectedAssignment && showFullReport) {
    return (
      <AssignmentFullReport
        assignmentId={selectedAssignment.assignmentId}
        assignmentTitle={selectedAssignment.assignmentTitle}
        onBack={() => {
          setShowFullReport(false);
          setSelectedAssignment(null);
        }}
      />
    );
  }

  return (
    <div className={cssStyles.container}>
      <div className={cssStyles.header}>
        <h2 className={cssStyles.title}>
          <img src="/images/icons/clipboard.svg" alt="" style={{width: '24px', height: '24px', marginRight: '10px'}} />
          Exam Assignments Reports
        </h2>
        <button 
          onClick={() => fetchAssignments(currentPage)}
          className={cssStyles.refreshButton}
          disabled={loading}
        >
          {loading ? 'Loading...' : 'Refresh'}
        </button>
      </div>

      {error && <div className={cssStyles.errorContainer}><p className={cssStyles.errorText}>{error}</p></div>}

      {loading && assignments.length === 0 ? (
        <div className={cssStyles.loadingContainer}><p className={cssStyles.loadingText}>Loading reports...</p></div>
      ) : (
        <>
          <div className={cssStyles.tableContainer}>
            <table className={cssStyles.table}>
              <thead className={cssStyles.thead}>
                <tr>
                  <th className={cssStyles.th}>Assignment</th>
                  <th className={cssStyles.th}>Exam Takers</th>
                  <th className={cssStyles.th}>Completion Rate</th>
                  <th className={cssStyles.th}>Avg Score</th>
                  <th className={cssStyles.th}>Status</th>
                  <th className={cssStyles.th}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {assignments.map((assignment) => (
                  <tr key={assignment.assignmentId} className={cssStyles.tr}>
                    <td className={cssStyles.td}>
                      <strong>{assignment.assignmentTitle}</strong>
                      <div className={cssStyles.assignmentDates}>
                        {formatDateToLocal(assignment.startDateUtc)}
                      </div>
                    </td>
                    <td className={cssStyles.td}>
                      {assignment.totalStudents} total ({assignment.completedStudents} done)
                    </td>
                    <td className={cssStyles.td}>
                      <div className={cssStyles.progressBar}>
                        <div className={cssStyles.progressFill} style={{width: `${assignment.completionRate}%`}} />
                      </div>
                      <span style={{fontSize: '11px'}}>{assignment.completionRate.toFixed(1)}%</span>
                    </td>
                    <td className={cssStyles.td}>
                      {assignment.averageScore != null ? `${assignment.averageScore.toFixed(1)}%` : 'N/A'}
                    </td>
                    <td className={cssStyles.td}>
                      <span className={cssStyles.statusBadge} style={{
                        backgroundColor: assignment.isActive ? '#dcfce7' : '#f3f4f6',
                        color: assignment.isActive ? '#166534' : '#4b5563'
                      }}>
                        {assignment.isActive ? 'Active' : 'Ended'}
                      </span>
                    </td>
                    <td className={cssStyles.td}>
                      <button 
                        onClick={() => handleViewFullReport(assignment)}
                        className={cssStyles.fullReportButton}
                        disabled={loadingFullReportAssignmentId === assignment.assignmentId}
                      >
                        {loadingFullReportAssignmentId === assignment.assignmentId ? '...' : 'View Full Report'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {totalPages > 1 && (
            <div className={cssStyles.pagination}>
              <button disabled={currentPage === 1} onClick={() => handlePageChange(currentPage - 1)}>Prev</button>
              <span>Page {currentPage} of {totalPages}</span>
              <button disabled={currentPage === totalPages} onClick={() => handlePageChange(currentPage + 1)}>Next</button>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default AssignmentsReports;
