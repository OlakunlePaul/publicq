using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PublicQ.Application.Interfaces;
using PublicQ.Application.Models;
using PublicQ.Domain.Enums;
using PublicQ.Domain.Models;
using PublicQ.Infrastructure.Options;
using PublicQ.Infrastructure.Persistence;
using PublicQ.Infrastructure.Persistence.Entities;
using PublicQ.Infrastructure.Persistence.Entities.Academic;
using PublicQ.Shared;
using PublicQ.Shared.Enums;

namespace PublicQ.Infrastructure.Services;

/// <summary>
/// User service implementation.
/// </summary>
/// <param name="userManager"></param>
/// <param name="logger"></param>
public class UserService(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    ApplicationDbContext dbContext,
    IMessageService messageService,
    IAssignmentService assignmentService,
    IEmailSender<ApplicationUser> identityEmailSender,
    IOptionsMonitor<UserServiceOptions> userServiceOptions,
    IOptionsMonitor<PasswordPolicyOptions> passwordPolicyOptions,
    ILogger<UserService> logger,
    IUserConfigurationProvider userConfigurationProvider) : IUserService
{
    /// <summary>
    /// Default user role for new users.
    /// </summary>
    private UserRole DefaultUserRole => UserRole.Student;
    
    /// <summary>
    /// Maximum attempts to generate a unique ID for student.
    /// </summary>
    private const int IdGenerationMaxAttempts = 10;
    
    /// <summary>
    /// Key name for student with no exams.
    /// </summary>
    private const string StudentWithNoExamsKeyName = "empty";

    /// <summary>
    /// <see cref="IUserService.LoginUserAsync"/>
    /// </summary>
    public async Task<Response<string, GenericOperationStatuses>> LoginUserAsync(string userId, string password)
    {
        Guard.AgainstNull(userId, nameof(userId));
        Guard.AgainstNullOrWhiteSpace(password, nameof(password));

        var user = await userManager.FindByNameAsync(userId);

        if (user == null)
        {
            logger.LogDebug("User '{UserId}' not found", userId);
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Unauthorized,
                "User or password is incorrect.");
        }

        var passwordCheckResult = await userManager.CheckPasswordAsync(user, password);

        if (!passwordCheckResult)
        {
            logger.LogInformation("Invalid password for user '{UserId}'", userId);
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Unauthorized,
                "User or password is incorrect.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var tokenResponse = tokenService.IssueToken(
            user.Id,
            user.Email!, 
            user.FullName, roles);

        if (tokenResponse.IsFailed)
        {
            logger.LogError("Token generation failed for user {UserId}: {Errors}", userId, tokenResponse.Errors);
            return Response<string, GenericOperationStatuses>.Failure(
                tokenResponse.Status,
                tokenResponse.Message,
                tokenResponse.Errors);
        }

        return Response<string, GenericOperationStatuses>
            .Success(tokenResponse.Data!, tokenResponse.Status, tokenResponse.Message);
    }

    /// <summary>
    /// <see cref="IUserService.SelfServiceUserRegistartionReturnTokenAsync"/>
    /// </summary>
    public async Task<Response<Token, GenericOperationStatuses>> SelfServiceUserRegistartionReturnTokenAsync(
        MailAddress email,
        string fullName,
        string password,
        DateTime? dateOfBirth,
        string? admissionNumber,
        Guid? classLevelId,
        CancellationToken cancellationToken)
    {
        if (!userServiceOptions.CurrentValue.SelfServiceRegistrationEnabled)
        {
            return Response<Token, GenericOperationStatuses>.Success(
                null!,
                GenericOperationStatuses.NotAllowed,
                "Self-service registration is disabled.");
        }
        
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNullOrWhiteSpace(password, nameof(password));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));

        var createdUser = await SelfServiceUserRegistrationAsync(
            email, 
            fullName, 
            password, 
            dateOfBirth,
            admissionNumber,
            baseUrl: default, // Not needed as user registers themselves with a password
            classLevelId,
            cancellationToken);

        if (createdUser.IsFailed)
        {
            return Response<Token, GenericOperationStatuses>.Failure(
                createdUser.Status,
                createdUser.Message,
                createdUser.Errors);
        }
        
        // This should never be null here as we just created the user
        var user = await userManager.FindByEmailAsync(email.Address);
        
        var tokenResponse = tokenService.IssueToken(
                user!.Id,
                email.Address,
                user.FullName,
                [DefaultUserRole.ToString()]);

        if (tokenResponse.IsFailed)
        {
            logger.LogError("Token generation failed for {Username}: {Errors}", email, tokenResponse.Errors);
            return Response<Token, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Token generation failed.",
                tokenResponse.Errors);
        }

        var token = new Token
        {
            AccessToken = tokenResponse.Data!
        };

        logger.LogInformation("User {Username} registered successfully", email);
        return Response<Token, GenericOperationStatuses>.Success(
            token,
            GenericOperationStatuses.Completed,
            $"User {email} registered successfully.");
    }

    /// <inheritdoc cref="IUserService.SelfServiceUserRegistrationAsync"/>
    public async Task<Response<GenericOperationStatuses>> SelfServiceUserRegistrationAsync(
        MailAddress email, 
        string fullName, 
        string? password,
        DateTime? dateOfBirth,
        string? admissionNumber,
        string? baseUrl,
        Guid? classLevelId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Request to register user received.");

        if (!userServiceOptions.CurrentValue.SelfServiceRegistrationEnabled)
        {
            return Response<GenericOperationStatuses>.Success(
                GenericOperationStatuses.NotAllowed,
                "Self-service registration is disabled.");
        }
        
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        
        return await RegisterIdentityUserAsync(
            email, 
            fullName, 
            password, 
            dateOfBirth, 
            admissionNumber,
            baseUrl, 
            classLevelId,
            cancellationToken);
    }

    /// <inheritdoc cref="IUserService.RegisterIdentityUserByAdminAsync"/>
    public async Task<Response<GenericOperationStatuses>> RegisterIdentityUserByAdminAsync(
        MailAddress email, 
        string fullName, 
        string? password, 
        DateTime? dateOfBirth,
        string? admissionNumber,
        string? baseUrl,
        Guid? classLevelId,
        CancellationToken cancellationToken)
    {
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        
        return await RegisterIdentityUserAsync(
            email, 
            fullName, 
            password, 
            dateOfBirth, 
            admissionNumber,
            baseUrl,
            classLevelId,
            cancellationToken);
    }

    /// <summary>
    /// Registers a new identity user.
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="fullName">Full name</param>
    /// <param name="password">Optional: password</param>
    /// <param name="dateOfBirth">Optional: Date of birth</param>
    /// <param name="baseUrl">Optional: Base URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    async Task<Response<GenericOperationStatuses>> RegisterIdentityUserAsync(
        MailAddress email, 
        string fullName, 
        string? password, 
        DateTime? dateOfBirth,
        string? admissionNumber,
        string? baseUrl,
        Guid? classLevelId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(baseUrl))
        {
            logger.LogDebug("Either password or createPasswordUrl must be provided.");
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                "Either password or createPasswordUrl must be provided.");
        }
        
        if (string.IsNullOrWhiteSpace(admissionNumber))
        {
            admissionNumber = await GenerateAdmissionNumberAsync(cancellationToken);
        }
        
        var emailAddressUpper = email.Address.ToUpper();

        var studentHasThisEmail = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(e => e.NormalizedEmail == emailAddressUpper, cancellationToken);

        if (studentHasThisEmail)
        {
            logger.LogInformation("Cannot register user. A student with email {Username} already exists.", 
                email);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                $"Cannot register user. A student with email '{email}' already exists.");
        }
        
        var user = new ApplicationUser
        {
            Email = email.Address,
            FullName = fullName,
            UserName = emailAddressUpper,
            NormalizedEmail = emailAddressUpper,
            NormalizedUserName = emailAddressUpper,
            DateOfBirth = dateOfBirth?.ToUniversalTime(),
            AdmissionNumber = admissionNumber,
            CreatedAtUtc = DateTime.UtcNow,
        };

        IdentityResult identityResult;
        if (!string.IsNullOrWhiteSpace(password))
        {
            var passwordValidationResponse = ValidatePassword(password);

            if (passwordValidationResponse.IsFailed)
            {
                var errors = string.Join(", ", passwordValidationResponse.Errors);
                logger.LogDebug("Password does not meet the security requirements. Errors: {Errors}", 
                    errors);
                return Response<GenericOperationStatuses>.Failure(
                    GenericOperationStatuses.BadRequest,
                    $"Password does not meet the security requirements. Validation errors: '{errors}'.");
            }
            
            identityResult = await userManager.CreateAsync(user, password);
        }
        else
        {
            identityResult = await userManager.CreateAsync(user);
        }
        
        if (!identityResult.Succeeded)
        {
            logger.LogError("User registration failed for {Username}: {Errors}", email, identityResult.Errors);
            var errors = identityResult.Errors
                .Select(e => e.Description)
                .ToList();

            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "User registration failed.",
                errors);
        }
        
        SendMessageRequest messageRequest;
        // If password is not provided, we skip sending the link
        if (string.IsNullOrWhiteSpace(password))
        {
            var createPasswordUrl = await GenerateResetLinkAsync(
                $"{baseUrl}/{Constants.FrontEndResetPasswordPath}", 
                user);
            messageRequest = new SendMessageRequest
            {
                TemplateId = Constants.DefaultWelcomeMessageWithCreatePasswordTemplateId,
                Recipients = [email.Address],
                Placeholders = new Dictionary<string, string>
                {
                    { "User", email.Address.Split("@")[0] },
                    { "BaseUrl", baseUrl! },
                    { "CreatePasswordUrl", createPasswordUrl }
                }
            };
        }
        else
        {
            messageRequest = new SendMessageRequest
            {
                TemplateId = Constants.DefaultWelcomeMessageTemplateId,
                Recipients = [email.Address],
                Placeholders = new Dictionary<string, string>
                {
                    { "User", email.Address.Split("@")[0] }
                }
            };
        }
        
        try
        {
            await NotifyUserAsync(email, messageRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send welcome message to {Username}. Registration will continue.", email);
        }
        
        if (classLevelId.HasValue)
        {
            await EnrollStudentInClassAsync(user.Id, classLevelId.Value, cancellationToken);
        }
        
        var assignDefaultRoleResult = await userManager.AddToRoleAsync(user, DefaultUserRole.ToString());
        if (!assignDefaultRoleResult.Succeeded)
        {
            logger.LogError(
                "User registration failed for {Username}: {Errors}.  User was created without default assigned role",
                email,
                assignDefaultRoleResult.Errors);

            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Registration failed. User was created without default assigned role.",
                assignDefaultRoleResult.Errors.Select(e => e.Description).ToList());
        }
        
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
            $"User {email} registered successfully.");
    }

    /// <inheritdoc cref="IUserService.RegisterStudentAsync"/>
    public async Task<Response<User, GenericOperationStatuses>> RegisterStudentAsync(
        MailAddress? email,
        string? id,
        DateTime? dateOfBirth,
        string fullName,
        string? admissionNumber,
        Guid? classLevelId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Request to register student received.");
        Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));

        // Removed mandatory date of birth check as per user request

        if (string.IsNullOrWhiteSpace(admissionNumber))
        {
            admissionNumber = await GenerateAdmissionNumberAsync(cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            string? generatedId = null;
            for (var attempt = 1; attempt <= IdGenerationMaxAttempts; attempt++)
            {
                var candidate = StudentIdGenerator.Generate(); // ensure generator returns uppercase
                var exists = await CheckIfStudentExistsAsync(email, candidate, cancellationToken);
                if (!exists)
                {
                    generatedId = candidate;
                    break;
                }
            }

            if (generatedId is null)
            {
                logger.LogError("Unable to generate a unique ID for the student after {MaxAttempts} attempts.",
                    IdGenerationMaxAttempts);
                return Response<User, GenericOperationStatuses>.Failure(
                    GenericOperationStatuses.Failed,
                    $"Unable to generate a unique ID for the student after '{IdGenerationMaxAttempts}' attempts."); 
            }
            
            id = generatedId;
        }
        else
        {
            id = id.Trim().ToUpperInvariant();
            var studentExists = await CheckIfStudentExistsAsync(email, id, cancellationToken);
            if (studentExists)
            {
                logger.LogWarning("Student already exists with given ID or Email");
                return Response<User, GenericOperationStatuses>.Failure(
                    GenericOperationStatuses.Conflict,
                    "Student already exists with given ID or Email");
            }
        }
        
        if (email is not null)
        {
            var userWithThisEmail = await userManager.FindByEmailAsync(email.Address);
            if (userWithThisEmail != null)
            {
                logger.LogWarning("Cannot register student. A user with email {Email} already exists.", 
                    email);
                return Response<User, GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                    $"Cannot register student. A user with email {email} already exists.");
            }
        }
        var student = new StudentEntity
        {
            Id = id,
            Email = email?.Address,
            NormalizedEmail = email?.Address.ToUpperInvariant(),
            FullName = fullName,
            DateOfBirth = dateOfBirth?.ToUniversalTime(),
            AdmissionNumber = admissionNumber,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        await dbContext.Students.AddAsync(student, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        if (classLevelId.HasValue)
        {
            await EnrollStudentInClassAsync(student.Id, classLevelId.Value, cancellationToken);
        }
        
        logger.LogInformation("Student registered successfully. ID: {Id}.", id);
        
        return Response<User, GenericOperationStatuses>.Success(
            student.ConvertToUser(),
            GenericOperationStatuses.Completed,
            $"Student registered successfully. ID: '{id}'.");
    }
    
    /// <summary>
    /// <see cref="IUserService.ResetIdentityUserPasswordByAdminAsync"/>
    /// </summary>
    public async Task<Response<GenericOperationStatuses>> ResetIdentityUserPasswordByAdminAsync(
        MailAddress email, 
        string password,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Request to reset password for exam taker received.");
        
        Guard.AgainstNullOrWhiteSpace(password, nameof(password));
        Guard.AgainstNull(email, nameof(email));
        
        var validatePasswordResponse = ValidatePassword(password);
        if (validatePasswordResponse.IsFailed)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                "Password does not meet the security requirements.",
                validatePasswordResponse.Errors);
        }

        var user = await userManager.FindByEmailAsync(email.Address);
        if (user == null)
        {
            logger.LogWarning("Unable to reset the password. User {Username} not found", email);
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"Unable to reset the password.User {email.Address} not found.");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);

        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to reset password for {Username}: {Errors}",
                email.Address,
                result.Errors);
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Failed to reset password for {email.Address}.",
                result.Errors.Select(e => e.Description).ToList());
        }

        logger.LogInformation("Password reset successfully for {Username}",
            email);

        var messageRequest = new SendMessageRequest
        {
            TemplateId = Constants.DefaultPasswordResetMessageTemplateId,
            Recipients = [email.Address],
            Placeholders = new Dictionary<string, string>
            {
                { "User", email.Address.Split("@")[0] },
                { "Password", password }
            }
        };
        
        // TODO: Consider removing password from the email notification for security reasons
        // and instead provide a link to set the password via a secure page.
        try
        {
            await NotifyUserAsync(email, messageRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send password reset message to {Username}. Reset will continue.", email);
        }

        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed,
            $"Password reset successfully for {email}.");
    }

    /// <summary>
    /// <see cref="IUserService"/>
    /// </summary>
    public async Task<Response<GenericOperationStatuses>> DeleteUserAsync(
        string userId,
        CancellationToken cancellation)
    {
        Guard.AgainstNullOrWhiteSpace(userId, nameof(userId));

        logger.LogInformation("Deleting user '{UserId}'", userId);
        var identityUser = await userManager.FindByIdAsync(userId);

        if (identityUser != null)
        {
            if (string.Equals(identityUser.Email, Constants.DefaultAdminEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogWarning(
                    "Attempt to delete the default admin user '{UserId}' not allowed. You cannot remove default admin account",
                    userId);
                return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed,
                    $"You cannot remove the default admin account '{userId}'.");
            }
            
            var result = await userManager.DeleteAsync(identityUser);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to delete user '{UserId}'. Errors: '{Errors}'", userId, result.Errors);
                return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Failed,
                    $"Failed to delete user with ID '{userId}'.",
                    result.Errors.Select(e => e.Description).ToList());
            }

            logger.LogInformation("User '{UserId}' deleted successfully", userId);

            return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
                $"User with ID '{userId}' deleted successfully.");
        }
        
        var student = await dbContext.Students
            .FirstOrDefaultAsync(e => e.Id == userId, cancellation);
        
        // If we reach here, identityUser and student are both null
        if (student == null)
        {
            logger.LogWarning("No user with {ID} ID found", userId);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"No user with '{userId}' ID found.");
        }
        
        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(cancellation);
        
        logger.LogInformation("Student with '{ID}' deleted successfully", userId);
        
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
            $"User with '{userId}' ID deleted successfully.");
    }

    /// <summary>
    /// <see cref="IUserService.AssignIdentityRoleAsync"/>
    /// </summary>
    public async Task<Response<GenericOperationStatuses>> AssignIdentityRoleAsync(
        string userId,
        UserRole roleName)
    {
        Guard.AgainstNullOrWhiteSpace(userId, nameof(userId));

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            logger.LogWarning(
                "Unable to assign role '{RoleName}' to user '{UserId}' Fetching user from database returned no result.",
                roleName,
                userId);

            return Response<GenericOperationStatuses>
                .Failure(GenericOperationStatuses.NotFound, $"User '{userId}' not found.");
        }

        if (await userManager.IsInRoleAsync(user, roleName.ToString()))
        {
            logger.LogInformation("User '{UserId}' already has the role '{RoleName}'.",
                userId,
                roleName);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                $"User '{userId}' already has the role '{roleName}'.");
        }

        var result = await userManager.AddToRoleAsync(user, roleName.ToString());

        if (!result.Succeeded)
        {
            logger.LogError("Failed to assign role '{RoleName}' to user '{UserId}': '{Errors}'",
                roleName,
                user,
                result.Errors);

            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Failed to assign role '{roleName}' to user '{userId}'.",
                result.Errors.Select(e => e.Description).ToList());
        }

        return Response<GenericOperationStatuses>
            .Success(GenericOperationStatuses.Completed,
                $"Role '{roleName}' assigned to user '{userId}' successfully.");
    }

    /// <summary>
    /// <see cref="IUserService.UnassignIdentityRoleAsync"/>
    /// </summary>
    public async Task<Response<GenericOperationStatuses>> UnassignIdentityRoleAsync(
        string userId,
        UserRole roleName)
    {
        Guard.AgainstNullOrWhiteSpace(userId, nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning(
                "Unable to unassign role '{RoleName}' from user '{UserId}' Fetching user from database returned no result.",
                roleName,
                userId);

            return Response<GenericOperationStatuses>
                .Failure(GenericOperationStatuses.NotFound, $"User '{userId}' not found.");
        }

        if (!await userManager.IsInRoleAsync(user, roleName.ToString()))
        {
            logger.LogInformation("User '{UserId}' does not have the role '{RoleName}'.",
                userId,
                roleName);
            return Response<GenericOperationStatuses>.Failure(GenericOperationStatuses.Conflict,
                $"User '{userId}' does not have the role '{roleName}'.");
        }

        var result = await userManager.RemoveFromRoleAsync(user, roleName.ToString());

        if (!result.Succeeded)
        {
            logger.LogError("Failed to unassign role '{RoleName}' from user '{UserId}': '{Errors}'",
                roleName,
                userId,
                result.Errors);

            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Failed to unassign role '{roleName}' from user '{userId}'.",
                result.Errors.Select(e => e.Description).ToList());
        }

        return Response<GenericOperationStatuses>
            .Success(GenericOperationStatuses.Completed,
                $"Role '{roleName}' unassigned from user '{userId}' successfully.");
    }

    /// <summary>
    /// <see cref="IUserService.GetUsersAsync"/>
    /// </summary>
    public async Task<Response<PaginatedResponse<User>, GenericOperationStatuses>> GetUsersAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? currentUserId = null,
        bool isSuperAdmin = true,
        UserRole? role = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Move to a repository pattern
        if (pageNumber < 1)
        {
            logger.LogDebug("Page number is less than 1 in the request. Setting to 1.");
            pageNumber = 1;
        }

        pageSize = Math.Min(pageSize, userServiceOptions.CurrentValue.MaxPageSize);

        // Project both sets to a common shape and UNION ALL, then order & page once.
        var usersQ = dbContext.Users.AsQueryable();

        if (!isSuperAdmin)
        {
            // Managers see themselves, teachers, students, and parents
            usersQ = usersQ.Where(u =>
                u.Id == currentUserId ||
                dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                    (dbContext.Roles.Any(r => r.Id == ur.RoleId && 
                        (r.Name == UserRolesNames.Teacher || 
                         r.Name == UserRolesNames.Student || 
                         r.Name == UserRolesNames.Parent)))));
        }

        // Apply explicit role filter if provided
        if (role != null)
        {
            var roleName = role.Value.ToString();
            usersQ = usersQ.Where(u =>
                dbContext.UserRoles.Any(ur => ur.UserId == u.Id &&
                    dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == roleName)));
        }

        var projectedUsersQ = usersQ
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FullName,
                u.DateOfBirth,
                u.CreatedAtUtc,
                AdmissionNumber = (string?)null,
                HasCredential = true
            });

        // Only include Students if no specific role is requested 
        // OR if the requested role is STUDENT
        IQueryable<StudentEntity>? studentsSourceQ = null;
        if (role == null || role == UserRole.Student)
        {
            studentsSourceQ = dbContext.Students.AsNoTracking();
        }

        var studentsQ = studentsSourceQ?
            .Select(e => new
            {
                e.Id,
                Email = e.Email,
                FullName = e.FullName,
                DateOfBirth = e.DateOfBirth,
                CreatedAtUtc = e.CreatedAtUtc,
                AdmissionNumber = e.AdmissionNumber,
                HasCredential = false
            });

        var unifiedQ = projectedUsersQ;
        if (studentsQ != null)
        {
            unifiedQ = unifiedQ.Concat(studentsQ);
        }

        var totalCount = await unifiedQ.LongCountAsync(cancellationToken);

        var pageItems = await unifiedQ
            .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(x => new User
            {
                Id = x.Id,
                Email = x.Email!,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                AdmissionNumber = x.AdmissionNumber,
                HasCredential = x.HasCredential
            })
            .ToListAsync(cancellationToken);

        var page = new PaginatedResponse<User>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
        page.Data.AddRange(pageItems);

        await PopulateUserRolesAsync(page.Data, cancellationToken);
        await PopulateUserEnrollmentAsync(page.Data, cancellationToken);
        
        var message = pageItems.Count == 0 ? "No users found." : "Users retrieved successfully.";
        return Response<PaginatedResponse<User>, GenericOperationStatuses>.Success(
            page, 
            GenericOperationStatuses.Completed, 
            message);
    }

    /// <summary>
    /// Populates roles for a list of users.
    /// </summary>
    private async Task PopulateUserRolesAsync(List<User> users, CancellationToken cancellationToken)
    {
        var identityUserIds = users
            .Where(u => u.HasCredential)
            .Select(u => u.Id)
            .ToList();

        if (identityUserIds.Count > 0)
        {
            var rolesLookup = await dbContext.UserRoles
                .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => identityUserIds.Contains(x.UserId))
                .ToListAsync(cancellationToken);

            foreach (var user in users.Where(u => u.HasCredential))
            {
                var userRoles = rolesLookup
                    .Where(x => x.UserId == user.Id)
                    .Select(x => x.Name!)
                    .ToList();
                
                // Add roles to the list
                foreach (var role in userRoles)
                {
                    if (!user.Roles.Contains(role))
                    {
                        user.Roles.Add(role);
                    }
                }
            }
        }

        // For Students (HasCredential = false), ensure they have the student role
        foreach (var user in users.Where(u => !u.HasCredential))
        {
            if (!user.Roles.Contains(UserRolesNames.Student))
            {
                user.Roles.Add(UserRolesNames.Student);
            }
        }
    }

    /// <summary>
    /// Populates enrollment information (Class, Session, Term) for a list of users.
    /// </summary>
    private async Task PopulateUserEnrollmentAsync(List<User> users, CancellationToken cancellationToken)
    {
        var userIds = users.Select(u => u.Id).ToList();
        
        var enrollments = await dbContext.StudentAssessments
            .AsNoTracking()
            .Include(sa => sa.ClassLevel)
            .Include(sa => sa.Session)
            .Include(sa => sa.Term)
            .Where(sa => userIds.Contains(sa.StudentId))
            .OrderByDescending(sa => sa.CreatedAt)
            .ToListAsync(cancellationToken);

        var enrollmentLookup = enrollments
            .GroupBy(sa => sa.StudentId)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var user in users)
        {
            if (enrollmentLookup.TryGetValue(user.Id, out var enrollment))
            {
                user.ClassName = enrollment.ClassLevel?.Name;
                user.SessionName = enrollment.Session?.Name;
                user.TermName = enrollment.Term?.Name;
                
                if (enrollment.ClassLevel != null && !string.IsNullOrEmpty(enrollment.ClassLevel.SectionOrArm))
                {
                    user.ClassName += $" ({enrollment.ClassLevel.SectionOrArm})";
                }
            }
        }
    }

    /// <inheritdoc cref="IUserService.GetStudentByIdAsync"/>
    public async Task<Response<User, GenericOperationStatuses>> GetStudentByIdAsync(
        string id, 
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Get student by id request received.");
        Guard.AgainstNullOrWhiteSpace(id, nameof(id));
        
        var studentId = await dbContext
            .Students
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.ToUpperInvariant(), cancellationToken);

        if (studentId == null)
        {
            logger.LogDebug("No student found with ID '{Id}'", id);
            return Response<User, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"No student found with ID '{id}'.");
        }
        
        var user = new User
        {
            Id = studentId.Id,
            Email = studentId.Email!,
            FullName = studentId.FullName,
            DateOfBirth = studentId.DateOfBirth,
            HasCredential = false
        };
        
        logger.LogDebug("Student with ID '{Id}' retrieved successfully.", id);
        return Response<User, GenericOperationStatuses>.Success(
            user,
            GenericOperationStatuses.Completed,
            $"Student with ID '{id}' retrieved successfully.");
    }

    /// <summary>
    /// <see cref="IUserService.GetUsersByFilter"/>
    /// </summary>
    /// TODO: Combine SearchUsersByEmailAsync and GetUsersAsync into a single method with optional email parameter
    /// TODO: Should we use Specification pattern here?
    public async Task<Response<PaginatedResponse<User>, GenericOperationStatuses>> GetUsersByFilter(
        string? email,
        string? id,
        int pageNumber = 1,
        int pageSize = 10,
        string? currentUserId = null,
        bool isSuperAdmin = true,
        UserRole? role = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            logger.LogWarning("Page number is less than 1 in the request. Setting to 1.");
            pageNumber = 1;
        }

        var maxPage = userServiceOptions.CurrentValue.MaxPageSize;
        pageSize = Math.Min(pageSize, maxPage);

        var queryIdentityUsers = dbContext.Users.AsNoTracking();

        if (!isSuperAdmin)
        {
            // Managers see themselves, teachers, students, and parents
            queryIdentityUsers = queryIdentityUsers.Where(u =>
                u.Id == currentUserId ||
                dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                    (dbContext.Roles.Any(r => r.Id == ur.RoleId && 
                        (r.Name == UserRolesNames.Teacher || 
                         r.Name == UserRolesNames.Student || 
                         r.Name == UserRolesNames.Parent)))));
        }

        // Apply explicit role filter if provided
        if (role != null)
        {
            var roleName = role.Value.ToString();
            queryIdentityUsers = queryIdentityUsers.Where(u =>
                dbContext.UserRoles.Any(ur => ur.UserId == u.Id &&
                    dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == roleName)));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            queryIdentityUsers = queryIdentityUsers.Where(u => u.Email != null && u.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            queryIdentityUsers = queryIdentityUsers.Where(u => u.Id.Contains(id));
        }

        var projectedUsersQ = queryIdentityUsers
            .Select(u => new
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                DateOfBirth = u.DateOfBirth,
                CreatedAtUtc = u.CreatedAtUtc,
                AdmissionNumber = u.AdmissionNumber,
                HasCredential = true
            });

        // Only include Students if no specific role is requested 
        // OR if the requested role is STUDENT
        var unifiedQ = projectedUsersQ;

        if (role == null || role == UserRole.Student)
        {
            var queryStudents = dbContext.Students.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var norm = email!.ToUpperInvariant();
                var pattern = $"%{norm.EscapeLike()}%";
                queryStudents = queryStudents.Where(e => EF.Functions.Like(e.NormalizedEmail, pattern));
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                var idPattern = $"%{id}%";
                queryStudents = queryStudents.Where(u => EF.Functions.Like(u.Id, idPattern));
            }

            unifiedQ = unifiedQ.Concat(queryStudents.Select(et => new
            {
                Id = et.Id,
                Email = et.Email,
                FullName = et.FullName,
                DateOfBirth = et.DateOfBirth,
                CreatedAtUtc = et.CreatedAtUtc,
                AdmissionNumber = et.AdmissionNumber,
                HasCredential = false
            }));
        }

        var totalCount = await unifiedQ.LongCountAsync(cancellationToken);

        var pageItems = await unifiedQ
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new User
            {
                Id = x.Id,
                Email = x.Email!,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                AdmissionNumber = x.AdmissionNumber,
                HasCredential = x.HasCredential
            })
            .ToListAsync(cancellationToken);

        await PopulateUserRolesAsync(pageItems, cancellationToken);
        await PopulateUserEnrollmentAsync(pageItems, cancellationToken);

        var response = new PaginatedResponse<User>
        {
            Data = pageItems,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return Response<PaginatedResponse<User>, GenericOperationStatuses>.Success(response, GenericOperationStatuses.Completed);
    }

    /// <inheritdoc cref="IUserService.GetUsersByIdAsync"/>
    public async Task<Response<IList<User>, GenericOperationStatuses>> GetUsersByIdAsync(
        HashSet<string> userIds, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Get user by ID request received.");
        
        if (userIds.Any(string.IsNullOrWhiteSpace))
        {
            logger.LogDebug("One or more user IDs are empty.");
            return Response<IList<User>, GenericOperationStatuses>
                .Failure(GenericOperationStatuses.BadRequest, "One or more user IDs are empty.");
        }
        
        var identityUsers = await dbContext.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new User
            {
                Id = u.Id,
                Email = u.Email!,
                FullName = u.FullName,
                DateOfBirth = u.DateOfBirth,
                HasCredential = true
            })
            .ToListAsync(cancellationToken);
        
        var examTakers = await dbContext.Students
            .AsNoTracking()
            .Where(e => userIds.Contains(e.Id))
            .Select(e => new User
            {
                Id = e.Id,
                Email = e.Email,
                FullName = e.FullName,
                DateOfBirth = e.DateOfBirth,
                HasCredential = false
            })
            .ToListAsync(cancellationToken);
        
        var totalUsers = identityUsers.Count + examTakers.Count;
        
        if (totalUsers == 0)
        {
            logger.LogInformation("No user with {UserId} ID is found in the database.", userIds);
            return Response<IList<User>, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"No user with {userIds} ID is found in the database.");
        }
        
        if (totalUsers != userIds.Count)
        {
            var foundUserIds = identityUsers.Select(u => u.Id).ToHashSet();
            var foundUserIdsStr = string.Join(", ", foundUserIds);
            var notFoundUserIdsStr = string.Join(", ", userIds.Except(foundUserIds));
            
            logger.LogWarning("Some users were not found. Requested IDs: {RequestedIds}, Found IDs: {FoundIds}, Not Found IDs: {NotFoundIds}",
                userIds, foundUserIdsStr, notFoundUserIdsStr);
            
            return Response<IList<User>, GenericOperationStatuses>.Success(identityUsers,GenericOperationStatuses.PartiallyCompleted,
                $"Some users were not found. Requested IDs: '{string.Join(", ", userIds)}', Found IDs: '{foundUserIdsStr}', Not Found IDs: '{notFoundUserIdsStr}'.");
        }
        
        var allUsers = identityUsers.ToList();
        await PopulateUserRolesAsync(allUsers, cancellationToken);
        
        return Response<IList<User>, GenericOperationStatuses>.Success(allUsers,GenericOperationStatuses.Completed);
    }

    /// <inheritdoc cref="IUserService.GetUserCountAsync"/>
    public async Task<Response<long, GenericOperationStatuses>> GetUserCountAsync(
        UserRole? role = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving total user count from the database. Role filter: {Role}", role);

        long identityUsersCount = 0;
        long studentsCount = 0;

        if (role == null)
        {
            identityUsersCount = await dbContext.Users.LongCountAsync(cancellationToken);
            studentsCount = await dbContext.Students.LongCountAsync(cancellationToken);
        }
        else
        {
            var roleName = role.Value.ToString();
            identityUsersCount = await dbContext.UserRoles
                .Where(ur => dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == roleName))
                .Select(ur => ur.UserId)
                .Distinct()
                .LongCountAsync(cancellationToken);

            if (role == UserRole.Student)
            {
                studentsCount = await dbContext.Students.LongCountAsync(cancellationToken);
            }
        }

        var totalUsers = identityUsersCount + studentsCount;
        
        logger.LogDebug("Retrieved total user count from the database: {Count}", totalUsers);

        return Response<long, GenericOperationStatuses>.Success(totalUsers, GenericOperationStatuses.Completed);
    }

    /// <inheritdoc cref="IUserService.GetUserRolesAsync"/>
    public async Task<Response<IList<UserRole>, GenericOperationStatuses>> GetUserRolesAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNullOrWhiteSpace(userId, nameof(userId));
        logger.LogDebug("Retrieving user roles from the database.");
        
        var roles = await userManager.GetRolesAsync(new ApplicationUser { Id = userId });
        
        if (roles.Count == 0)
        {
            logger.LogDebug("No roles found for user with ID '{UserId}'", userId);
            return Response<IList<UserRole>, GenericOperationStatuses>.Failure(GenericOperationStatuses.NotFound,
                $"No roles found for user with ID '{userId}'.");
        }

        var parsedRoles = roles.Select(r =>
        {
            if (!Enum.TryParse<UserRole>(r, out var role))
            {
                // This should never happen if roles are managed correctly
                logger.LogError("Failed to parse role '{Role}' for user '{UserId}'", 
                    r, 
                    userId);
            }
            
            return role;
        }).ToList();
        
        logger.LogDebug("Retrieved roles from the database: {Roles}", parsedRoles);
        
        return Response<IList<UserRole>, GenericOperationStatuses>.Success(
            parsedRoles, 
            GenericOperationStatuses.Completed);
    }

    /// <inheritdoc cref="IUserService.ImportStudents"/>
    public async Task<Response<IList<User>, GenericOperationStatuses>> ImportStudents(
        IList<StudentImportDto> students,
        string importedByUser,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Import students request received.");
        Guard.AgainstNull(students, nameof(students));
        
        var validationResponse = await ValidateStudentBeforeImportAsync(students, cancellationToken);
        
        if (validationResponse.IsFailed)
        {
            return Response<IList<User>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Conflict,
                validationResponse.Message,
                validationResponse.Errors);
        }
        
        logger.LogDebug("Creating new students.");
        var errorMessages = new List<string>();

        var groupedUsersWithAssignments = new Dictionary<Guid, IList<User>>();
        var usersWithNoAssignment = new List<User>();
        
        foreach (var student in students)
        {
            var email = student.Email is not null ? 
                new MailAddress(student.Email) : 
                null;
            
            var userResponse = await RegisterStudentAsync(
                email,
                student.Id,
                student.DateOfBirth,
                student.Name, 
                student.AdmissionNumber,
                null,
                cancellationToken);

            if (userResponse.IsSuccess)
            {
                if (student.AssignmentId.HasValue && 
                    groupedUsersWithAssignments.TryGetValue(student.AssignmentId.Value, out var users))
                {
                    users.Add(userResponse.Data!);
                }
                else if (student.AssignmentId.HasValue)
                {
                    groupedUsersWithAssignments[student.AssignmentId.Value] = new List<User> { userResponse.Data! };
                }
                else
                {
                    usersWithNoAssignment.Add(userResponse.Data!);
                }
            }
            // We need to keep processing other users even if one fails
            // and report all errors at the end
            else
            {
                errorMessages.Add(userResponse.Message);
            }
        }

        foreach (var group in groupedUsersWithAssignments)
        {
            var userIds = group
                .Value
                .Select(u => u.Id)
                .ToHashSet();
            
            var assignStudentsResult = await assignmentService.AddStudentsAsync(
                group.Key, 
                userIds, 
                importedByUser, 
                cancellationToken);

            if (assignStudentsResult.IsFailed)
            {
                errorMessages.Add($"Unable to assign students to assignment '{group.Key}' ID. Errors:  {string.Join(",", assignStudentsResult.Errors)}");
            }
        }

        // Combine all created users
        var createdUsers = groupedUsersWithAssignments.Select(g => g.Value)
            .SelectMany(u => u)
            .Concat(usersWithNoAssignment)
            .ToList();
        
        if (errorMessages.Count > 0)
        {
            var errorMessagesString = string.Join("; ", errorMessages);
            logger.LogWarning("Import students completed with some errors. {ErrorMessages}", errorMessagesString);
            return Response<IList<User>, GenericOperationStatuses>.Success(
                createdUsers,
                GenericOperationStatuses.PartiallyCompleted,
                $"Import students completed with some errors. Error messages: '{errorMessagesString}'");
        }
        
        return Response<IList<User>, GenericOperationStatuses>.Success(
            createdUsers,
            GenericOperationStatuses.Completed,
            "Import students completed successfully.");
    }

    /// <inheritdoc cref="IUserService.ForgetPasswordAsync"/>
    public async Task<Response<GenericOperationStatuses>> ForgetPasswordAsync(
        MailAddress email,
        string url,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Forget password request received.");
        Guard.AgainstNull(email, nameof(email));
        
        var user = await userManager.FindByEmailAsync(email.Address);
        if (user == null)
        {
            logger.LogWarning("No user found with email '{Email}'", email);
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"No user found with email '{email.Address}'.");
        }
        
        var resetLink = await GenerateResetLinkAsync(url, user);

        await identityEmailSender.SendPasswordResetLinkAsync(user, user.Email!, resetLink);
        
        return Response<GenericOperationStatuses>.Success(GenericOperationStatuses.Completed,
            $"Password reset link sent to '{email.Address}' if the user exists.");
    }

    /// <inheritdoc cref="IUserService.ValidateResetPasswordToken"/>
    public async Task<Response<GenericOperationStatuses>> ValidateResetPasswordToken(
        MailAddress email, 
        string token, 
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email.Address);
        if (user == null)
        {
            logger.LogInformation("Given Token is invalid. No user found with email '{Email}'", email);
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Token is invalid try to request a new one.");
        }
        
        var isValid = await userManager.VerifyUserTokenAsync(
            user, 
            userManager.Options.Tokens.PasswordResetTokenProvider, 
            UserManager<ApplicationUser>.ResetPasswordTokenPurpose, 
            token);

        if (isValid)
        {
            logger.LogInformation("Given Token is valid for '{Email}'", email);
            return Response<GenericOperationStatuses>.Success(
                GenericOperationStatuses.Completed,
                "Token is valid.");
        }
        
        logger.LogInformation("Given Token is invalid for '{Email}'", email);
        return Response<GenericOperationStatuses>.Failure(
            GenericOperationStatuses.Failed,
            "Token is invalid try to request a new one.");
    }

    /// <inheritdoc cref="IUserService.ResetPasswordAsync"/>
    public async Task<Response<string, GenericOperationStatuses>> ResetPasswordAsync(
        MailAddress email, 
        string token, 
        string newPassword, 
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Reset password request received.");
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNullOrWhiteSpace(token, nameof(token));
        Guard.AgainstNullOrWhiteSpace(newPassword, nameof(newPassword));
        
        var validatePasswordResponse = ValidatePassword(newPassword);
        if (validatePasswordResponse.IsFailed)
        {
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                "Password does not meet the security requirements.",
                validatePasswordResponse.Errors);
        }
        
        var user = await userManager.FindByEmailAsync(email.Address);
        if (user == null)
        {
            logger.LogWarning("No user found with email '{Email}'", email);
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.NotFound,
                $"No user found with email '{email.Address}'.");
        }
        
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to reset password for '{Email}': {Errors}",
                email,
                result.Errors);
            return Response<string, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                $"Failed to reset password for '{email.Address}'.",
                result.Errors.Select(e => e.Description).ToList());
        }
        
        logger.LogInformation("Password reset successfully for '{Email}'",
            email);
        
        var roles = await userManager.GetRolesAsync(user);

        // We do not check if token generation succeeded, as failure is unlikely here
        // and we want to let the user know that password reset was successful regardless.
        var tokenResponse = tokenService.IssueToken(
            user.Id,
            user.Email!, 
            user.FullName, roles);
        
        // Return success response with the new token or empty string if token generation failed
        return Response<string, GenericOperationStatuses>.Success(
            tokenResponse.Data ?? string.Empty,
            GenericOperationStatuses.Completed,
            $"Password reset successfully for '{email.Address}'.");
    }

    /// <summary>
    /// Validates students before import. Checks for existing IDs or Emails and verifies assignments.
    /// </summary>
    /// <param name="students">Arrays of <see cref="StudentImportDto"/></param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns <see cref="GenericOperationStatuses"/> wrapped into <see cref="Response{TStatus}"/></returns>
    private async Task<Response<GenericOperationStatuses>> ValidateStudentBeforeImportAsync(
        IList<StudentImportDto> students,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Checking for existing users with the same IDs or Emails.");

        if (students.Count > userServiceOptions.CurrentValue.MaxImportSize)
        {
            logger.LogInformation("Max import size exceeded. Max: {MaxImportSize}, Provided: {ProvidedSize}",
                userServiceOptions.CurrentValue.MaxImportSize,
                students.Count);
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.BadRequest,
                $"Max import size exceeded. Max: '{userServiceOptions.CurrentValue.MaxImportSize}', Provided: '{students.Count}'");
        }
        
        // Extract IDs and emails for efficient querying
        var studentIds = students.Select(e => e.Id).ToList();
        var studentIdsUppercase = students.Select(e => e.Id.ToUpperInvariant()).ToList();
        var studentEmails = students
            .Where(e => !string.IsNullOrWhiteSpace(e.Email))
            .Select(e => e.Email!)
            .ToList();
        
        var existingUsers = await dbContext
            .Users
            .AsNoTracking()
            .Where(u => studentIds.Contains(u.Id)
                || (u.Email != null && studentEmails.Contains(u.Email)))
            .ToListAsync(cancellationToken);
            
        var existingStudentsInDb = await dbContext.Students
            .Where(e => studentIdsUppercase.Contains(e.Id)
                || (e.Email != null && studentEmails.Contains(e.Email)))
            .ToListAsync(cancellationToken);
        
        var errorMessages = new List<string>();
        if (existingUsers.Count > 0 || existingStudentsInDb.Count > 0)
        {
            var existingIds = existingUsers.Select(u => u.Id)
                .Concat(existingStudentsInDb.Select(e => e.Id))
                .Distinct()
                .ToList();
            var existingEmails = existingUsers.Select(u => u.Email)
                .Concat(existingStudentsInDb.Select(e => e.Email))
                .Where(e => e != null)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();
            
            if (existingIds.Count > 0)
            {
                errorMessages.Add($"The following IDs already exist: {string.Join(", ", existingIds)}");
            }
            if (existingEmails.Count > 0)
            {
                errorMessages.Add($"The following Emails already exist: {string.Join(", ", existingEmails)}");
            }
            
            var errorMessage = string.Join("; ", errorMessages);
            logger.LogWarning("Import students failed due to existing IDs or Emails. {ErrorMessage}", errorMessage);
            
            return Response<IList<User>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Conflict,
                "Import students failed due to existing IDs or Emails.",
                errorMessages);
        }
        logger.LogDebug("ID and Email verification completed.");

        logger.LogDebug("Checking for assignments linked to the students.");
        var assignmentIds = students
            .Where(e => e.AssignmentId != null)
            .Select(e => e.AssignmentId!.Value)
            .Distinct()
            .ToList();

        if (assignmentIds.Count > 0)
        {
            var existingAssignments = await dbContext.Assignments
                .AsNoTracking()
                .Where(a => assignmentIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            if (existingAssignments.Count != assignmentIds.Count)
            {
                var missingAssignmentIds = assignmentIds
                    .Except(existingAssignments.Select(a => a.Id))
                    .ToList();
                
                logger.LogWarning("Import students failed due to missing assignments. Missing Assignment IDs: {MissingAssignmentIds}",
                    string.Join(", ", missingAssignmentIds));
                
                errorMessages.Add($"The following Assignment IDs do not exist: {string.Join(", ", missingAssignmentIds)}");
            }
        }

        logger.LogDebug("Assignment verification completed.");
        
        if (errorMessages.Count > 0)
        {
            return Response<IList<User>, GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Import students failed due to validation issues, check errors for more details.",
                errorMessages);
        }
        
        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed,
            "Validation completed successfully.");
    }
    
    /// <summary>
    /// Notifies the user via email with a message request.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="messageRequest"><see cref="SendMessageRequest"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task NotifyUserAsync(
        MailAddress email,
        SendMessageRequest messageRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var messageSendResponse = await messageService.SendAsync(messageRequest, cancellationToken);
            if (messageSendResponse.IsFailed)
            {
                logger.LogWarning("Failed to send message to {Username}: {Errors}",
                    email,
                    messageSendResponse.Errors);
            }
            else if (messageSendResponse.Status == GenericOperationStatuses.FeatureIsNotAvailable)
            {
                logger.LogDebug("Message send feature is not available. Skipping sending message to {Username}",
                    email);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "An error occurred while sending a message to {Username}: {Message}", 
                email, ex.Message);
        }
    }
    
    /// <summary>
    /// Validates the password against the defined security policies.
    /// </summary>
    /// <param name="password">Password to check</param>
    private Response<GenericOperationStatuses> ValidatePassword(string password)
    {
        var errors = new List<string>();
        
        if (password.Length < passwordPolicyOptions.CurrentValue.RequiredLength)
        {
            errors.Add($"Password must be at least {passwordPolicyOptions.CurrentValue.RequiredLength} characters long");
        }
        
        if (passwordPolicyOptions.CurrentValue.RequireDigit && !password.Any(char.IsDigit))
        {
            errors.Add("Password must contain at least one digit");
        }
        
        if (passwordPolicyOptions.CurrentValue.RequireLowercase && !password.Any(char.IsLower))
        {
            errors.Add("Password must contain at least one lowercase letter");
        }
        
        if (passwordPolicyOptions.CurrentValue.RequireUppercase && !password.Any(char.IsUpper))
        {
            errors.Add("Password must contain at least one uppercase letter");
        }
        
        if (passwordPolicyOptions.CurrentValue.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
        {
            errors.Add("Password must contain at least one non-alphanumeric character");
        }
        
        if (errors.Count > 0)
        {
            return Response<GenericOperationStatuses>.Failure(
                GenericOperationStatuses.Failed,
                "Password does not meet the security requirements.",
                errors);
        }
        
        return Response<GenericOperationStatuses>.Success(
            GenericOperationStatuses.Completed,
            "Password meets the security requirements.");
    }
    
    /// <summary>
    /// Returns true if a student with the given email or ID already exists.
    /// </summary>
    /// <param name="email">Optional: Email address</param>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task<bool> CheckIfStudentExistsAsync(
        MailAddress? email, 
        string id, 
        CancellationToken cancellationToken)
    {
        var hasEmail = email is not null;
        
        return await dbContext.Students
            .AsNoTracking()
            .AnyAsync(e => (hasEmail && e.NormalizedEmail == email!.Address.ToUpperInvariant()) 
                             || e.Id == id.ToUpperInvariant(), cancellationToken);
    }
    
    /// <summary>
    /// Generates a password reset link for the user.
    /// </summary>
    /// <param name="url">Base URL</param>
    /// <param name="user"><see cref="ApplicationUser"/></param>
    /// <returns>Returns a link to reset password</returns>
    private async Task<string> GenerateResetLinkAsync(string url, ApplicationUser user)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var encodedEmail = Uri.EscapeDataString(user.Email!);
        
        var resetLink = $"{url}?email={encodedEmail}&token={encodedToken}";
        return resetLink;
    }

    /// <summary>
    /// Auto-generates an admission number based on system configuration.
    /// </summary>
    private async Task<string> GenerateAdmissionNumberAsync(CancellationToken cancellationToken)
    {
        var configResult = await userConfigurationProvider.GetConfigurationAsync<AdmissionNumberConfiguration>(
            UserConfigTypes.AdmissionNumber, cancellationToken);
            
        var format = "EN-{YYYY}-{0000}";
        var sequence = 0;
            
        if (configResult.IsSuccess && configResult.Data != null)
        {
            format = configResult.Data.Format;
            sequence = configResult.Data.LastSequenceNumber;
        }
        
        sequence++;
        
        if (configResult.IsSuccess)
        {
            var configData = configResult.Data;
            if (configData == null)
            {
                // Create new configuration if missing
                configData = new AdmissionNumberConfiguration
                {
                    Format = format,
                    LastSequenceNumber = sequence
                };
            }
            else
            {
                configData.LastSequenceNumber = sequence;
            }
            
            await userConfigurationProvider.SetConfigurationAsync(configData, cancellationToken);
        }

        var year = DateTime.UtcNow.Year.ToString();
        var numStr = sequence.ToString("D4"); 
        
        return format.Replace("{YYYY}", year).Replace("{0000}", numStr);
    }

    private async Task EnrollStudentInClassAsync(string studentId, Guid classLevelId, CancellationToken cancellationToken)
    {
        var activeSession = await dbContext.Sessions.FirstOrDefaultAsync(s => s.IsActive, cancellationToken);
        if (activeSession == null)
        {
            logger.LogWarning("Cannot enroll student {StudentId}: No active session found.", studentId);
            return;
        }

        var activeTerm = await dbContext.Terms.FirstOrDefaultAsync(t => t.IsActive && t.SessionId == activeSession.Id, cancellationToken);
        if (activeTerm == null)
        {
            logger.LogWarning("Cannot enroll student {StudentId}: No active term found for session {SessionId}.", studentId, activeSession.Id);
            return;
        }

        // Check if enrollment already exists to avoid duplicates
        var exists = await dbContext.StudentAssessments.AnyAsync(a => 
            a.StudentId == studentId && 
            a.SessionId == activeSession.Id && 
            a.TermId == activeTerm.Id, cancellationToken);

        if (!exists)
        {
            var assessment = new StudentAssessmentEntity
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                SessionId = activeSession.Id,
                TermId = activeTerm.Id,
                ClassLevelId = classLevelId,
                Status = ModerationStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.StudentAssessments.Add(assessment);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Student {StudentId} enrolled in Class {ClassLevelId} for {Session}/{Term}", 
                studentId, classLevelId, activeSession.Name, activeTerm.Name);
        }
    }
}
