using System.Runtime.InteropServices;
using DinkToPdf;
using DinkToPdf.Contracts;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Reporting;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class ReportsService : IReportsService
{
    private readonly IConverter _converter;

    public ReportsService(IConverter converter)
    {
        _converter = converter;
        InitializeConverter();
    }

    public MemoryStream GeneratePdfReport(
        EvaluationSession session,
        List<Evaluation> evaluations,
        List<KpiHistoryRow> kpiTable,
        List<string> sessionNames,
        bool canForecast,
        decimal? forecastedWeightedScore)
    {
        var html = GenerateHtml(session, evaluations, kpiTable, sessionNames, canForecast, forecastedWeightedScore);

        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 },
            },
            Objects = {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                }
            }
        };

        var pdfBytes = _converter.Convert(doc);
        return new MemoryStream(pdfBytes);
    }

    private static void InitializeConverter()
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        string libraryPath;
        if (isWindows)
        {
            libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libwkhtmltox.dll");
        }
        else
        {
            throw new PlatformNotSupportedException("Your operating system is not supported.");
        }

        if (!File.Exists(libraryPath))
        {
            throw new FileNotFoundException($"The native library file was not found at: {libraryPath}");
        }
    }

    private static string GenerateHtml(
        EvaluationSession session,
        List<Evaluation> evaluations,
        List<KpiHistoryRow> kpiTable,
        List<string> sessionNames,
        bool canForecast,
        decimal? forecastedWeightedScore)
    {
        var employee = session.Employee!;
        var role = employee.User?.Role?.RoleName ?? "Unknown";
        var team = employee.Team?.Name ?? "No Team";
        var avatarBase64 = employee.Avatar != null ? Convert.ToBase64String(employee.Avatar) : null;

        var forecastTableHtml = "";
        if (!canForecast)
        {
            forecastTableHtml = "<div class='section'><b>Not enough data to forecast (need at least 3 sessions).</b></div>";
        }
        else
        {
            forecastTableHtml = "<div class='section'><h3 class='section-title'>KPI Performance History & Forecast</h3>";
            forecastTableHtml += "<table class='kpi-table'><thead><tr><th>KPI</th>";
            foreach (var s in sessionNames)
                forecastTableHtml += $"<th>{s}</th>";
            forecastTableHtml += "</tr></thead><tbody>";
            foreach (var row in kpiTable)
            {
                forecastTableHtml += $"<tr><td>{row.KpiName}</td>";
                for (int i = 0; i < row.Scores.Count; i++)
                {
                    var val = row.Scores[i].ToString("F2");
                    // If this is the forecast column, add arrow
                    if (canForecast && i == row.Scores.Count - 1 && row.Scores.Count > 1)
                    {
                        var prev = row.Scores[row.Scores.Count - 2];
                        var forecast = row.Scores[row.Scores.Count - 1];
                        var arrow = forecast > prev ? "<span style='color:green;font-size:16px;'>&#9650;</span>" : (forecast < prev ? "<span style='color:red;font-size:16px;'>&#9660;</span>" : "");
                        forecastTableHtml += $"<td>{val} {arrow}</td>";
                    }
                    else
                    {
                        forecastTableHtml += $"<td>{val}</td>";
                    }
                }
                forecastTableHtml += "</tr>";
            }
            forecastTableHtml += "</tbody></table>";
            if (forecastedWeightedScore.HasValue)
            {
                var lastSessionScore = session.WeightedScore ?? 0;
                var arrow = forecastedWeightedScore > lastSessionScore ? "<span style='color:green;font-size:16px;'>&#9650;</span>" : (forecastedWeightedScore < lastSessionScore ? "<span style='color:red;font-size:16px;'>&#9660;</span>" : "");
                forecastTableHtml += $"<div style='margin-top:10px;'><b>General Performance Forecast:</b> {forecastedWeightedScore.Value:F2} {arrow}</div>";
            }
            forecastTableHtml += "</div>";
        }

        var html = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{
                    font-family: 'Segoe UI', Arial, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    margin: 0;
                    padding: 20px;
                }}
                .header {{
                    text-align: center;
                    color: #2c3e50;
                    margin-bottom: 30px;
                    border-bottom: 2px solid #3498db;
                    padding-bottom: 10px;
                }}
                .employee-section {{
                    display: flex;
                    flex-direction: row;
                    gap: 30px;
                    margin-bottom: 30px;
                    background:rgb(228, 230, 231);
                    padding: 20px;
                    border-radius: 8px;
                    box-sizing: border-box;
                }}
                .avatar {{
                    width: 120px;
                    height: 120px;
                    min-width: 120px;
                    min-height: 120px;
                    border-radius: 50%;
                    object-fit: cover;
                    border: 3px solid #3498db;
                }}
                .employee-info {{
                    max-width: 300px;
                }}
                .section {{
                    margin-bottom: 30px;
                    background: white;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }}
                .section-title {{
                    color: #2c3e50;
                    border-bottom: 2px solid #3498db;
                    padding-bottom: 5px;
                    margin-bottom: 15px;
                }}
                .kpi-table {{
                    width: 100%;
                    border-collapse: collapse;
                    margin-top: 10px;
                }}
                .kpi-table th, .kpi-table td {{
                    border: 1px solid #b0b0b0;
                    padding: 6px 10px;
                    text-align: center;
                }}
                .kpi-table th {{
                    background: #e8f4f8;
                }}
                .evaluation-item {{
                    border-left: 4px solid #3498db;
                    padding-left: 15px;
                    margin-bottom: 20px;
                    page-break-inside: avoid;
                    break-inside: avoid;
                }}
                .summary-section {{
                    background: #e8f4f8;
                    padding: 20px;
                    border-radius: 8px;
                }}
                .recommended-actions {{
                    list-style-type: none;
                    padding-left: 0;
                }}
                .recommended-actions li {{
                    margin-bottom: 10px;
                    padding-left: 20px;
                    position: relative;
                }}
                .recommended-actions li:before {{
                    content: '•';
                    color: #3498db;
                    position: absolute;
                    left: 0;
                }}
            </style>
        </head>
        <body>
            <div class='header'>
                <h1>Evaluation Report: {session.Name}</h1>
            </div>
        
            <div class='employee-section'>
                <div> 
                    {(avatarBase64 != null ? $"<img src='data:image/jpeg;base64,{avatarBase64}' class='avatar' alt='Employee Avatar'>" : "")}
                </div>
                <div class='employee-info'>
                    <h2>{employee.FirstName} {employee.LastName}</h2>
                    <p><strong>Role:</strong> {role}</p>
                    <p><strong>Team:</strong> {team}</p>
                    <p><strong>Hire Date:</strong> {employee.HireDate:d}</p>
                </div>
            </div>
        
            <div class='section'>
                <h3 class='section-title'>Session Details</h3>
                <p><strong>Start Date:</strong> {FormatDateTime(session.StartDate)}</p>
                <p><strong>End Date:</strong> {FormatDateTime(session.EndDate)}</p>
                <p><strong>Finished:</strong> {FormatDateTime((DateTimeOffset)session.EvaluationFinishedDate!)}</p>
                <p><strong>Weighted Score:</strong> {session.WeightedScore!.Value:F2}</p>
            </div>
            <div class='section'>
                <h3 class='section-title'>Evaluations</h3>
                    {string.Join("", evaluations.OrderBy(e => e.RoleKpi?.KpiMetric?.Name ?? string.Empty).Select(eval =>
                    {
                        var kpiName = eval.RoleKpi?.KpiMetric?.Name ?? "Unknown KPI";
                        var description = eval.RoleKpi?.ScoreRangeDescription ?? "No description";
                        var evaluator = eval.Evaluator is not null
                            ? $"{eval.Evaluator.FirstName} {eval.Evaluator.LastName}"
                            : "Unknown";
                        var evaluatorRole = eval.Evaluator?.User?.Role?.RoleName ?? "Unknown Role";

                        return $@"
                                <div class='evaluation-item'>
                                    <h4>{kpiName}</h4>
                                    <p><strong>Score:</strong> {eval.Score} ({description})</p>
                                    <p><strong>Evaluator:</strong> {evaluator} ({evaluatorRole})</p>
                                    <p><strong>Comment:</strong> {eval.Comment}</p>
                                </div>";
                    }))}
            </div>

            {forecastTableHtml}

            <div class='summary-section'>
                <h3 class='section-title'>Summary</h3>
                {(session.Class != null ? $@"
                    <p><strong>Employee Class:</strong> {session.Class.ClassName}</p>
                    <p><strong>Description:</strong> {session.Class.Description}</p>
                    {(session.Class.RecommendedActions.Length > 0 ? $@"
                        <p><strong>Recommended Actions:</strong></p>
                        <ul class='recommended-actions'>
                            {string.Join("", session.Class.RecommendedActions.Select(action => $"<li>{action}</li>"))}
                        </ul>
                    " : "")}
                " : "")}
            </div>
        </body>
        </html>";

        return html;
    }

    private static string FormatDateTime(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss zzz");
    }
}
