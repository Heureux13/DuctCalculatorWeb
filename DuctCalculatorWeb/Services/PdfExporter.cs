using Microsoft.JSInterop;

namespace DuctCalculatorWeb.Services;

public record PdfSection(string Title, string[] Head, string[][] Rows);

public record PdfSectionBox(string Name, double Total, string[][] Rows);
public record PdfRunBox(string Name, double Total, PdfSectionBox[] Sections);
public record PdfBranchBox(string Name, double Total, PdfRunBox[] Runs);

public record PdfPhoto(string Label, string Data);

public class PdfExporter(IJSRuntime js)
{
    public async Task ExportNested(string title, string subtitle, string[] head,
        IEnumerable<PdfBranchBox> branches, string filename, string[]? legend = null,
        IEnumerable<PdfPhoto>? photos = null, bool email = false)
    {
        await js.InvokeVoidAsync("downloadNestedPdf", title, subtitle, head, branches, filename,
            legend ?? [], photos ?? [], email);
    }
    public async Task Export(string title, string subtitle, IEnumerable<PdfSection> sections,
        string filename, string orientation = "portrait")
    {
        var data = sections.Select(s => new { title = s.Title, head = s.Head, rows = s.Rows });
        await js.InvokeVoidAsync("downloadPdf", title, subtitle, data, filename, orientation);
    }
}