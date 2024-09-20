document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById('djSearch');
    const djItems = document.querySelectorAll('.dj-item');
    const noResultsMessage = document.getElementById('noResultsMessage');

    searchInput.addEventListener('input', function () {
        const searchText = searchInput.value.toLowerCase();
        let hasVisibleItems = false;

        djItems.forEach(function (item) {
            const djName = item.querySelector('h4').textContent.toLowerCase();
            if (djName.includes(searchText)) {
                item.classList.remove('d-none'); // Mostra il DJ se il nome corrisponde
                hasVisibleItems = true; // C'è almeno un elemento visibile
            } else {
                item.classList.add('d-none'); // Nasconde il DJ se non corrisponde
            }
        });

        // Mostra o nascondi il messaggio a seconda della presenza di elementi visibili
        if (hasVisibleItems) {
            noResultsMessage.classList.add('d-none');
        } else {
            noResultsMessage.classList.remove('d-none');
        }
    });
});
