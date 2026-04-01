/**
 * Base student assignment properties.
 */
export interface StudentAssignmentBase {
  /**
   * Gets or sets the user ID of the student assigned to this assignment.
   */
  studentId: string;
  tabSwitchCount: number;
  isLocked: boolean;
}

/**
 * Student assignment creation data transfer object.
 */
export interface StudentAssignmentCreate extends StudentAssignmentBase {
  // Inherits examTakerId from base
}
