
// Script per cambiare icona hamburger
document.addEventListener("DOMContentLoaded", function () {
    const navbarToggler = document.querySelector(".navbar-toggler");
    const toggleIcon = document.getElementById("toggleIcon");

    // Gestisci il cambio di icona al click del pulsante
    navbarToggler.addEventListener("click", function () {
        if (navbarToggler.classList.contains("collapsed")) {
            // Navbar chiusa: mostra l'icona freccia in giù
            toggleIcon.innerHTML = `<i class="bi bi-chevron-bar-down fs-2"></i>`;
        } else {
            // Navbar aperta: mostra l'icona freccia in su
            toggleIcon.innerHTML = `<i class="bi bi-chevron-bar-up fs-2"></i>`;
        }
    });

    // Imposta l'icona freccia in giù inizialmente
    toggleIcon.innerHTML = `<i class="bi bi-chevron-bar-down fs-2"></i>`;
});




    let prevScrollPos = window.pageYOffset; // Posizione iniziale dello scroll
const navbar = document.getElementById("navbar");

    window.onscroll = function() {
        let currentScrollPos = window.pageYOffset;

        if (prevScrollPos > currentScrollPos) {
        // Quando si scorre verso l'alto, la navbar ricompare
            navbar.style.top = "0";
        } else {
        // Quando si scorre verso il basso, la navbar si nasconde
            navbar.style.top = "-300px"; // Altezza della navbar o simile
        }

    prevScrollPos = currentScrollPos;
}


//Funzione per la ricerca asincrona di eventi e artisti
document.getElementById('searchbarInput').addEventListener('input', function () {
    var query = this.value;
    var resultsContainer = document.getElementById('searchbarResults');

    if (query.length > 2) {
        fetch(window.location.origin + '/Home/Search?name=' + encodeURIComponent(query))
            .then(response => response.json())
            .then(data => {
                console.log(data);
                resultsContainer.innerHTML = '';

                let foundResults = false;

                if (data && Array.isArray(data.events)) {
                    if (data.events.length > 0) {
                        foundResults = true;
                        var eventHeader = document.createElement('h5');
                        eventHeader.textContent = 'Eventi';
                        eventHeader.className = 'text-light ps-2';
                        resultsContainer.appendChild(eventHeader);

                        data.events.forEach(event => {
                            var listItem = document.createElement('a');
                            listItem.href = '/Event/Details/' + event.eventId;
                            listItem.className = 'result-item d-flex justify-content-center align-items-start p-3 rounded-1 my-2 text-decoration-none';

                            var image = document.createElement('img');
                            image.src = event.imageUrl;
                            image.alt = event.name;
                            image.className = 'result-imageevent me-3';

                            var details = document.createElement('div');

                            var title = document.createElement('h6');
                            title.textContent = event.name;
                            title.className = 'text-light mb-1';

                            var dateRange = document.createElement('p');
                            var startDate = new Date(event.dateFrom);
                            var endDate = new Date(event.dateTo);
                            dateRange.textContent = `${startDate.getDate().toString().padStart(2, '0')}/${(startDate.getMonth() + 1).toString().padStart(2, '0')} - ${endDate.getDate().toString().padStart(2, '0')}/${(endDate.getMonth() + 1).toString().padStart(2, '0')}`;
                            dateRange.className = 'myText small mb-1';

                            var location = document.createElement('p');
                            location.textContent = event.location;
                            location.className = 'myText small mb-1';

                            details.appendChild(title);
                            details.appendChild(dateRange);
                            details.appendChild(location);

                            listItem.appendChild(image);
                            listItem.appendChild(details);

                            resultsContainer.appendChild(listItem);
                        });
                    }
                }

                if (data && Array.isArray(data.dJs)) {
                    if (data.dJs.length > 0) {
                        foundResults = true;
                        var djHeader = document.createElement('h5');
                        djHeader.textContent = 'Artisti';
                        djHeader.className = 'text-light mt-2 ps-2';
                        resultsContainer.appendChild(djHeader);

                        data.dJs.forEach(dj => {
                            var listItem = document.createElement('a');
                            listItem.href = '/Dj/Details/' + dj.djId;
                            listItem.className = 'result-item d-flex align-items-center p-3 rounded-1 my-2 text-decoration-none';

                            var image = document.createElement('img');
                            image.src = dj.img;
                            image.alt = dj.artistName;
                            image.className = 'result-imagedj me-3';

                            var details = document.createElement('div');

                            var name = document.createElement('h6');
                            name.textContent = dj.artistName;
                            name.className = 'text-light mb-1';

                            details.appendChild(name);

                            listItem.appendChild(image);
                            listItem.appendChild(details);

                            resultsContainer.appendChild(listItem);
                        });
                    }
                }

                // Mostra i risultati solo se è stato trovato qualcosa
                if (foundResults) {
                    resultsContainer.classList.remove('d-none');
                    resultsContainer.classList.add('d-block');
                } else {
                    resultsContainer.classList.remove('d-block');
                    resultsContainer.classList.add('d-none');
                }
            })
            .catch(error => console.error('Errore:', error));
    } else {
        // Nasconde i risultati se la query è troppo breve
        resultsContainer.innerHTML = '';
        resultsContainer.classList.remove('d-block');
        resultsContainer.classList.add('d-none');
    }
});

document.addEventListener('DOMContentLoaded', function () {
    function getCartItemsCount(id) {
        fetch(`/Comment/getcommentlikes?commentId=${commentId}`)
            .then(response => response.json())
            .then(data => {
                const spanLike = document.getElementById('spanLike-' + commentId);
                const likeButton = document.querySelector(`.like-button[data-comment-id='${commentId}']`);
                const icon = likeButton.querySelector('i');

                if (spanLike) {
                    spanLike.textContent = data.likeCount; // Aggiorna il numero di like
                }

                // Aggiorna la classe dell'icona
                if (data.userHasLiked) {
                    icon.classList.remove('myActions');
                    icon.classList.add('text-danger');
                } else {
                    icon.classList.remove('text-danger');
                    icon.classList.add('myActions');
                }
            })
            .catch(error => {
                console.error('Errore durante il recupero dei like:', error);
            });
    }
});

function getCartItemsCount() {
    $.ajax({
        url: '/Cart/GetCartItemsCount', 
        type: 'GET',
        success: function (response) {
            if (response.success) { 
                if (response.count > 0) {
                    $('#cartItemsCount').text(response.count);
                } else {
                    $('#cartItemsCount').addClass('d-none');

                }
            } else {
                console.log("Errore: " + response.message);
            }
        },
        error: function (error) {
            console.log("Errore nella richiesta AJAX", error);
        }
    });
}

$(document).ready(function () {
    getCartItemsCount();
});