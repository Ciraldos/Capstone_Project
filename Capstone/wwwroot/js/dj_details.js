document.addEventListener('DOMContentLoaded', () => {
    const scrollableDiv = document.getElementById('scrollableDiv');
    const relatedDiv = document.getElementById('scrollableRelatedDiv');

    let isScrolling;
    const scrollSpeed = 2; // Aumenta questo valore per incrementare la quantità di scroll

    function handleScroll(e, div) {
        // Verifica se il div contiene almeno 3 elementi <a>
        const links = div.querySelectorAll('a');
        if (links.length > 3) {
            e.preventDefault(); // Previeni lo scroll della pagina

            // Calcola il nuovo valore di scroll con un fattore di scala
            const scrollAmount = e.deltaY * scrollSpeed;

            // Funzione per scorrere in modo fluido
            function smoothScroll() {
                div.scrollLeft += scrollAmount;

                // Interrompi l'animazione se non ci sono più eventi di scorrimento
                if (isScrolling) {
                    requestAnimationFrame(smoothScroll);
                }
            }

            // Inizia lo scroll fluido
            isScrolling = true;
            smoothScroll();

            // Fermare l'animazione dopo un breve periodo se non ci sono più eventi di scorrimento
            clearTimeout(isScrolling);
            isScrolling = setTimeout(() => {
                isScrolling = false;
            }, 50); // 50 ms di ritardo prima di fermare l'animazione
        }
    }

    scrollableDiv.addEventListener('wheel', (e) => handleScroll(e, scrollableDiv));
    relatedDiv.addEventListener('wheel', (e) => handleScroll(e, relatedDiv));
});
