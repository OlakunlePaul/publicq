export interface User {
  /**
   * Unique identifier for the user.
   */
  id: string;

  /**
   * Email address of the user.
   */
  email?: string;

  /**
   * Optional: Full name of the user.
   */
  fullName?: string;

  /**
   * Optional: Date of birth of the user.
   */
  dateOfBirth?: string; // ISO date string

  /**
   * Optional: Student admission number.
   */
  admissionNumber?: string;

  /**
   * Indicates if the user has valid credentials.
   */
  hasCredential: boolean;

  /**
   * Roles assigned to the user.
   */
  roles?: string[];

  /**
   * Current class name (if enrolled).
   */
  className?: string;

  /**
   * Current session name (if enrolled).
   */
  sessionName?: string;

  /**
   * Current term name (if enrolled).
   */
  termName?: string;
}