/**
 * Individual user report details
 */
export interface IndividualUserReport {
  /**
   * The unique identifier for this exam taker assignment.
   * A GUID that uniquely identifies this student-assignment relationship.
   */
  id: string;

  /**
   * The identifier of the exam taker (student) assigned to this assignment.
   * References the student who must complete the assigned modules.
   * This is typically the user ID from the identity system (e.g., ASP.NET Core Identity).
   * The same student can have multiple student assignment records for different assignments.
   */
  studentId: string;

  /**
   * Exam taker's display name at the time of assignment.
   * This redundantly stores the name to preserve historical accuracy,
   * even if the user's name changes later or in case of user deletion in the identity system.
   */
  studentDisplayName: string;

  /**
   * Number of times the student switched tabs or minimized the browser during the exam.
   */
  tabSwitchCount: number;

  /**
   * The timestamp of the last recorded browser focus loss.
   */
  lastTabSwitchAtUtc?: string;
}