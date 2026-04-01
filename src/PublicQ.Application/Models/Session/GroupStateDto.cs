using PublicQ.Application.Models.Group;

namespace PublicQ.Application.Models.Session;

/// <summary>
/// Contains necessary information to present the group state.
/// </summary>
public class GroupStateDto : GroupBaseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Group member entities
    /// </summary>
    public HashSet<GroupMemberStateDto> GroupMembers { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the assignment is locked due to anti-cheat violations.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets the number of tab switches recorded for this assignment.
    /// </summary>
    public int TabSwitchCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tab switches allowed.
    /// </summary>
    public int MaxTabSwitches { get; set; }
}