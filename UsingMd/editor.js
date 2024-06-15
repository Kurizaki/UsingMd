document.addEventListener('DOMContentLoaded', () => {
    function makeTableEditable(table) {
        const rows = table.rows;
        for (let i = 0; i < rows.length; i++) {
            const cells = rows[i].cells;
            for (let j = 0; j < cells.length; j++) {
                const cell = cells[j];
                cell.setAttribute('contenteditable', 'true');
                cell.addEventListener('input', updateMarkdown);
            }
        }
    }

    function addRow(table, rowIndex) {
        const newRow = table.insertRow(rowIndex + 1);
        const cellCount = table.rows[0].cells.length;
        for (let i = 0; i < cellCount; i++) {
            const newCell = newRow.insertCell(i);
            newCell.setAttribute('contenteditable', 'true');
            newCell.addEventListener('input', updateMarkdown);
        }
    }

    function addColumn(table, columnIndex) {
        for (let i = 0; i < table.rows.length; i++) {
            const newCell = table.rows[i].insertCell(columnIndex + 1);
            newCell.setAttribute('contenteditable', 'true');
            newCell.addEventListener('input', updateMarkdown);
        }
    }

    function updateMarkdown() {
        const table = document.querySelector('table');
        let markdownTable = '';
        const rows = table.rows;
        for (let i = 0; i < rows.length; i++) {
            let rowMarkdown = '| ';
            const cells = rows[i].cells;
            for (let j = 0; j < cells.length; j++) {
                rowMarkdown += cells[j].textContent + ' | ';
            }
            markdownTable += rowMarkdown + '\n';
            if (i === 0) {
                markdownTable += '|---'.repeat(cells.length) + '|\n';
            }
        }
        window.chrome.webview.postMessage(markdownTable);
    }

    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeName === 'TABLE') {
                        makeTableEditable(node);
                    }
                });
            }
        });
    });

    observer.observe(document.body, { childList: true, subtree: true });
});
