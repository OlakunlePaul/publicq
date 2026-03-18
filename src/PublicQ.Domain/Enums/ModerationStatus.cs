namespace PublicQ.Domain.Enums;

/// <summary>
/// Represents the approval lifecycle of an academic report card or result.
/// </summary>
public enum ModerationStatus
{
    /// <summary>
    /// The score is currently being entered or edited by the subject/class teacher. Not visible to parents.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// The teacher has finalized the entry and submitted it for review by an Admin/Head Teacher. Not visible to parents.
    /// </summary>
    Moderated = 1,

    /// <summary>
    /// The Admin/Head Teacher has reviewed and approved the result. It is ready for publication.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// The result has been published to the portal and is visible to the student and linked parents.
    /// </summary>
    Published = 3
}
