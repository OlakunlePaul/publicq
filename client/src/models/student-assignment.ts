import { Assignment } from "./assignment";
import { StudentAssignmentBase } from "./student-assignment-base";
import { ModuleProgress } from "./module-progress";

/**
 * Student assignment data transfer object that represents the assignment of a specific student to a specific assignment.
 */
export interface StudentAssignment extends StudentAssignmentBase {
  /**
   * Gets or sets the unique identifier of the student assignment.
   */
  id: string;

  /**
   * Gets or sets the foreign key reference to the assignment.
   */
  assignmentId: string;

  /**
   * The assignment entity this student is assigned to.
   */
  assignment?: Assignment;

  /**
   * The collection of module progress records for this student's assignment.
   */
  moduleProgress?: ModuleProgress[];
}