

document.addEventListener('DOMContentLoaded', function () {
    const checkboxes = document.querySelectorAll('#genresCheckboxes input[type="checkbox"]');

    checkboxes.forEach(checkbox => {
        // Aggiungi un event listener a ciascun checkbox
        checkbox.addEventListener('change', function () {
            const label = document.querySelector(`label[for="${this.id}"]`);

            // Controlla se il checkbox è selezionato e cambia la classe del bottone
            if (this.checked) {
                label.classList.add('selected');  // Aggiunge la classe per il colore selezionato
            } else {
                label.classList.remove('selected');  // Rimuove la classe se deselezionato
            }
        });
    });
});