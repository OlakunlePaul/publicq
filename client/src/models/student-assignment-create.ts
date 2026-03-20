import { StudentAssignmentBase } from './student-assignment-base';

/**
 * Student assignment creation data transfer object.
 */
export interface StudentAssignmentCreate extends StudentAssignmentBase {
  /**
   * Student unique identifier.
   */
  studentId: string;
}
