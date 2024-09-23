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

        // Aggiungi la classe "selected" se il checkbox è già selezionato al caricamento
        if (checkbox.checked) {
            const label = document.querySelector(`label[for="${checkbox.id}"]`);
            label.classList.add('selected');
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('imgInput');
    const imgPreview = document.getElementById('imgPreview');

    fileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();

            reader.onload = function (e) {
                imgPreview.src = e.target.result;
                imgPreview.classList.remove('d-none');
            }

            reader.readAsDataURL(file);
        } else {
            imgPreview.src = "#";
            imgPreview.classList.add('d-none');
        }
    });
});