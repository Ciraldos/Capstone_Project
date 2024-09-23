document.addEventListener('DOMContentLoaded', function () {
    const checkboxes = document.querySelectorAll('#genresCheckboxes input[type="checkbox"]');

    // Aggiungi la classe 'selected' agli checkbox già selezionati
    checkboxes.forEach(checkbox => {
        const label = document.querySelector(`label[for="${checkbox.id}"]`);
        if (checkbox.checked) {
            label.classList.add('selected');  // Aggiunge la classe se il checkbox è selezionato
        }

        // Aggiungi un event listener per il cambiamento
        checkbox.addEventListener('change', function () {
            // Cambia la classe del bottone
            if (this.checked) {
                label.classList.add('selected');
            } else {
                label.classList.remove('selected');
            }
        });
    });
});
