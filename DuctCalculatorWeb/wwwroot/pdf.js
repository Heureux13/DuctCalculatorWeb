window.downloadPdf = (title, subtitle, sections, filename, orientation = 'portrait') => {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ orientation });

    doc.setFontSize(16);
    doc.text(title, 14, 16);
    doc.setFontSize(10);
    doc.text(subtitle, 14, 23);

    const subtitleLines = subtitle.split('\n').length;
    let y = 23 + subtitleLines * 5 + 4;

    for (const s of sections) {
        doc.setFontSize(12);
        doc.text(s.title, 14, y);
        doc.autoTable({
            startY: y + 3,
            head: [s.head],
            body: s.rows,
            theme: 'grid',
            styles: { fontSize: 9 },
            headStyles: { fillColor: [60, 60, 60] }
        });
        y = doc.lastAutoTable.finalY + 10;
    }

    doc.save(filename);
};

window.downloadNestedPdf = (title, subtitle, head, branches, filename, legend) => {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ orientation: 'landscape' });

    const pageW = doc.internal.pageSize.getWidth();
    const pageH = doc.internal.pageSize.getHeight();
    const top = 12;
    const bottom = pageH - 12;
    const left = 10;
    const right = pageW - 10;

    const orange = [253, 126, 20];
    const blue = [13, 110, 253];
    const green = [25, 135, 84];

    const open = [];
    let y;

    const rect = (b, endY) => {
        doc.setDrawColor(...b.color);
        doc.setLineWidth(0.6);
        doc.rect(left + b.inset, b.segStart, right - left - 2 * b.inset, endY - b.segStart);
    };

    const newPage = () => {
        open.forEach(b => rect(b, bottom));
        doc.addPage();
        y = top;
        open.forEach(b => { b.segStart = top; });
    };

    const ensure = (space) => {
        if (y + space > bottom) newPage();
    };

    const openBox = (label, name, total, color, inset, space) => {
        ensure(space);
        const box = { color, inset, segStart: y };
        open.push(box);

        doc.setFont(undefined, 'bold');
        doc.setFontSize(7);
        doc.setTextColor(...color);
        doc.text(`${label}: ${name || ''}`, left + inset + 3, y + 6);
        doc.setFont(undefined, 'normal');
        doc.setTextColor(0);
        y += 10;
        box.total = total;
    };

    const closeBox = () => {
        const box = open.pop();
        y += 2;
        doc.setFont(undefined, 'bold');
        doc.setFontSize(10);
        doc.setTextColor(...box.color);
        doc.text(`Total: ${box.total.toFixed(2)} CFM`, right - box.inset - 3, y + 4, { align: 'right' });
        doc.setFont(undefined, 'normal');
        doc.setTextColor(0);
        y += 7;
        rect(box, y);
        y += 3;
    };

    doc.setFontSize(16);
    doc.text(title, left, 16);
    doc.setFontSize(10);
    const lines = subtitle.split('\n');
    lines.forEach((line, li) => {
        const fields = line.split('|');
        const colW = (right - left) / fields.length;
        fields.forEach((f, fi) => doc.text(f.trim(), left + fi * colW, 23 + li * 6));
    });
    y = 23 + lines.length * 6 + 2;

    for (const branch of branches) {
        openBox('Branch', branch.name, branch.total, orange, 0, 40);

        for (const run of branch.runs) {
            openBox('Run', run.name, run.total, blue, 4, 34);

            for (const sec of run.sections) {
                openBox('Section', sec.name, sec.total, green, 8, 28);

                const startPage = doc.internal.getCurrentPageInfo().pageNumber;

                doc.autoTable({
                    startY: y,
                    head: [head],
                    body: sec.rows,
                    theme: 'grid',
                    styles: { fontSize: 7, cellPadding: 1.2, halign: 'center' },
                    headStyles: { fillColor: [60, 60, 60] },
                    margin: { left: left + 11, right: pageW - right + 11, top: top, bottom: pageH - bottom }
                });

                const endPage = doc.internal.getCurrentPageInfo().pageNumber;

                if (endPage > startPage) {
                    for (let p = startPage; p < endPage; p++) {
                        doc.setPage(p);
                        open.forEach(b => { rect(b, bottom); b.segStart = top; });
                    }
                    doc.setPage(endPage);
                }

                y = doc.lastAutoTable.finalY + 2;
                closeBox();
            }

            closeBox();
        }

        closeBox();
    }

    if (legend && legend.length) {
        doc.setFontSize(8);
        const width = right - left;
        const wrapped = legend.map(l =>
            l.startsWith('#') ? { head: true, lines: [l.slice(1)] }
                              : { head: false, lines: doc.splitTextToSize(l, width - 4) });
        const needed = wrapped.reduce((n, w) => n + w.lines.length * 3.6, 0) + 4;
        ensure(Math.min(needed, 60));

        wrapped.forEach(w => {
            const h = w.lines.length * 3.6;
            if (y + h > bottom) newPage();
            doc.setFont(undefined, w.head ? 'bold' : 'normal');
            doc.text(w.lines, left, y + 3);
            y += h + (w.head ? 0.5 : 0);
        });
        doc.setFont(undefined, 'normal');
    }

    doc.save(filename);
};
