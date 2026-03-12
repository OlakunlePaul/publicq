using System.Runtime.Serialization;
using PublicQ.Shared.Enums;

namespace PublicQ.Application.Models;

/// <summary>
/// Admission number configuration settings.
/// </summary>
public class AdmissionNumberConfiguration : IConfigurationModel
{
    /// <summary>
    /// Configuration type of the model.
    /// </summary>
    [IgnoreDataMember]
    public UserConfigTypes UserConfigType => UserConfigTypes.AdmissionNumber;

    /// <summary>
    /// Format string for auto-generating admission numbers.
    /// E.g., EN-{YYYY}-{0000}
    /// </summary>
    public string Format { get; set; } = "EN-{YYYY}-{0000}";
    
    /// <summary>
    /// The last assigned sequential number (to manage the {0000} part).
    /// </summary>
    public int LastSequenceNumber { get; set; } = 0;
}
