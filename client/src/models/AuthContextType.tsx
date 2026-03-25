import { LoginRequest } from "./loginRequest";
import { UserRole } from "./UserRole";

export interface AuthContextType {
  token: string | null;
  saveToken: (token: string) => void;
  login: (credentials: LoginRequest) => Promise<void>;
  loginStudent: (admissionId: string) => Promise<void>;
  logout: () => void;
  userRoles: UserRole[];
  isAuthenticated: boolean;
}