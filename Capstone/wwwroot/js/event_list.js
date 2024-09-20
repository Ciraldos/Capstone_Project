document.addEventListener('DOMContentLoaded', function () {
    const genres = document.querySelectorAll('.filter-genre');
    const events = document.querySelectorAll('.related-event');
    const cancelFilterBtn = document.getElementById('cancelFilterBtn');
    const noResultsMessage = document.getElementById('noResultsMessage');

    // Funzione per filtrare gli eventi in base al genere
    genres.forEach(genre => {
        genre.addEventListener('click', function () {
            const selectedGenre = this.getAttribute('data-genre');
            let foundEvent = false;

            // Rimuove lo stile attivo da tutti i bottoni
            genres.forEach(g => g.classList.remove('active-genre'));

            // Aggiunge la classe attiva al bottone cliccato
            this.classList.add('active-genre');

            // Nasconde tutti gli eventi
            events.forEach(event => {
                event.style.setProperty('display', 'none', 'important');
            });

            // Mostra solo gli eventi che corrispondono al genere selezionato
            events.forEach(event => {
                const eventGenres = event.getAttribute('data-genres').split(',');

                if (eventGenres.includes(selectedGenre)) {
                    event.style.setProperty('display', 'flex', 'important');
                    foundEvent = true; // Imposta che almeno un evento è stato trovato
                }
            });

            // Mostra il messaggio "Nessun artista trovato" se non ci sono eventi
            if (!foundEvent) {
                noResultsMessage.classList.remove('d-none');
            } else {
                noResultsMessage.classList.add('d-none');
            }
        });
    });

    // Funzione per resettare i filtri e mostrare tutti gli eventi
    cancelFilterBtn.addEventListener('click', function () {
        // Rimuove lo stile attivo da tutti i bottoni
        genres.forEach(g => g.classList.remove('active-genre'));

        // Nasconde il messaggio "Nessun artista trovato"
        noResultsMessage.classList.add('d-none');

        // Mostra tutti gli eventi
        events.forEach(event => {
            event.style.setProperty('display', 'flex', 'important');
        });
    });
});
