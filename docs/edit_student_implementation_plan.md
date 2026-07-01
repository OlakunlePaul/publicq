# Implementation Plan: Edit Student/User Feature

This document outlines the proposed implementation plan for adding an "Edit User" feature to the ExamNova platform. This feature will allow administrators to edit student and user details directly from the frontend UI.

## Objective
Enable administrators to update user and student profiles (e.g., correcting misspellings in the "Full Name" field or updating email addresses/class assignments).

## Backend Changes (C# ASP.NET Core)
1. **Create Data Transfer Objects (DTOs)**
   - Create `UserUpdateRequest` to hold the fields that can be edited (e.g., `FullName`, `DateOfBirth`, `ClassLevelId`).

2. **Update Service Interface (`IUserService`)**
   - Add a method signature: `Task<Response<User, GenericOperationStatuses>> UpdateUserAsync(string userId, UserUpdateRequest request, CancellationToken cancellationToken);`

3. **Implement Service Logic (`UserService.cs`)**
   - Implement `UpdateUserAsync`:
     - Fetch the user/student by `userId`.
     - Update properties directly on the entity (`ApplicationUser` or `StudentEntity`).
     - Save changes to the database.
     - Return the updated user object.

4. **Add Controller Endpoint (`UsersController.cs`)**
   - Create an `[HttpPut("{userId}")]` endpoint.
   - Authorize using `Constants.ManagersPolicy` or `Constants.AdminsPolicy`.
   - Call the `UpdateUserAsync` service and return the result.

## Frontend Changes (React)
1. **Update API Client (`userService.ts`)**
   - Add an `updateUser(userId: string, data: any)` function that performs an HTTP PUT request to `/api/users/{userId}`.

2. **Create Edit Component (`UserManagement.tsx`)**
   - Build an `EditUserModal` component (similar to `CreateUserModal`).
   - The modal should populate with the selected user's current data.
   - Upon submission, it will call `userService.updateUser`.

3. **Update Table Component (`UserTable.tsx`)**
   - Ensure the `onUserAction` prop supports the `edit` action.
   - In `UserManagement.tsx`, capture the `edit` action to open the new `EditUserModal`.

## Deployment
1. Commit the C# backend changes and deploy to the backend hosting provider (Railway).
2. Commit the React frontend changes and deploy to the frontend hosting provider (Vercel).

## Future Considerations
- Ensure email updates (if allowed) properly handle uniqueness constraints.
- Maintain auditing/logging for profile updates if required.
