using System.Runtime.Serialization;
using PublicQ.Shared.Enums;

namespace PublicQ.Application.Models;

/// <summary>
/// Defines the school's branding configuration (Name, Address, Logo).
/// </summary>
public class SchoolBrandingConfiguration : IConfigurationModel
{
    [IgnoreDataMember]
    public UserConfigTypes UserConfigType => UserConfigTypes.SchoolBranding;

    public string SchoolName { get; set; } = "Day & Boarding School";
    
    public string SchoolAddress { get; set; } = "123 Education Avenue, Knowledge City.";
    
    public string SchoolPhone { get; set; } = "0800-SCHOOL-123";
    
    public string? SchoolLogoUrl { get; set; }
}
