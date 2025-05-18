document.addEventListener("DOMContentLoaded", () => {
    const rows = document.querySelectorAll("table.viewall tbody tr");
    rows.forEach(row => {
        const ratingCell = row.cells[4]; // Rating está en la columna 5 (índice 4)
        const rating = parseFloat(ratingCell.textContent);
        if (!isNaN(rating)) {
            ratingCell.textContent = rating.toFixed(1);
        }
    });
});
