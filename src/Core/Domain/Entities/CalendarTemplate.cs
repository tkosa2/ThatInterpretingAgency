using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarTemplate : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string TemplateType { get; private set; } = string.Empty; // 'Weekly', 'Monthly', 'Custom'
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<CalendarTemplateRule> Rules { get; private set; } = new List<CalendarTemplateRule>();

    private CalendarTemplate() { }

    public static CalendarTemplate Create(string userId, string name, string templateType, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(templateType))
            throw new ArgumentException("Template type cannot be empty", nameof(templateType));

        return new CalendarTemplate
        {
            UserId = userId.Trim(),
            Name = name.Trim(),
            TemplateType = templateType.Trim(),
            Description = description?.Trim(),
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }

    public void AddRule(CalendarTemplateRule rule)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        Rules.Add(rule);
        UpdateTimestamp();
    }

    public void RemoveRule(Guid ruleId)
    {
        var rule = Rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule != null)
        {
            Rules.Remove(rule);
            UpdateTimestamp();
        }
    }

    // Business logic methods
    public bool IsWeekly => TemplateType.Equals("Weekly", StringComparison.OrdinalIgnoreCase);
    public bool IsMonthly => TemplateType.Equals("Monthly", StringComparison.OrdinalIgnoreCase);
    public bool IsCustom => TemplateType.Equals("Custom", StringComparison.OrdinalIgnoreCase);

    public bool HasRules => Rules.Any();


}
