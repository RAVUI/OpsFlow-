namespace OpsFlow.Models.DTOs.Claims;

public class ClaimDashboardResponse
{
    public int TotalIssues { get; set; }

    public int RaisedIssues { get; set; }

    public int InProgressIssues { get; set; }

    public int ReviewIssues { get; set; }

    public int ClosedIssues { get; set; }

    public int TodayIssues { get; set; }

    public int MyRaisedIssues { get; set; }

    public int MyAssignedIssues { get; set; }

    public List<DashboardClaimItem> RecentIssues { get; set; }
        = new();
}

public class DashboardClaimItem
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string RaisedByName { get; set; } = string.Empty;

    public string? AssignedEmployeeName { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}