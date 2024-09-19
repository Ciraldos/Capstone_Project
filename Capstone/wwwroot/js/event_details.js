
//############## GIF ##############
document.addEventListener('DOMContentLoaded', function () {
    // Toggle GIF section
    const addGifSectionBtn = document.getElementById('addGifSectionBtn');
    if (addGifSectionBtn) {
        addGifSectionBtn.addEventListener('click', function () {
            const gifSection = document.getElementById('gifSection');
            const gifSearch = document.getElementById('gifSearch');
            const gifResults = document.getElementById('gifResults');
            const selectedGifContainer = document.getElementById('selectedGifContainer');

            if (gifSection) {
                gifSection.style.display = (gifSection.style.display === 'none' || gifSection.style.display === '') ? 'block' : 'none';
                gifSearch.value = ''; // Clear the search input
                gifResults.innerHTML = ''; // Clear the search results
                selectedGifContainer.style.display = 'none'; // Hide the selected GIF container
            }
        });
    }

    // Search GIFs
    const gifSearch = document.getElementById('gifSearch');
    if (gifSearch) {
        gifSearch.addEventListener('input', function () {
            const query = this.value.trim();
            const gifResults = document.getElementById('gifResults');

            if (query === '') {
                gifResults.innerHTML = ''; // Clear results if query is empty
                return;
            }

            searchGifs(query).then(gifs => {
                gifResults.innerHTML = ''; // Clear previous results
                gifs.forEach(gif => {
                    const imgElement = document.createElement('img');
                    imgElement.src = gif.images.fixed_height.url;
                    imgElement.className = 'img-thumbnail m-2';
                    imgElement.style.cursor = 'pointer';
                    imgElement.addEventListener('click', function () {
                        document.getElementById('selectedGif').src = gif.images.fixed_height.url;
                        document.getElementById('hiddenGifUrl').value = gif.images.fixed_height.url;
                        document.getElementById('selectedGifContainer').style.display = 'block'; // Show the selected GIF container
                        gifResults.innerHTML = ''; // Clear the GIF search results
                    });
                    gifResults.appendChild(imgElement);
                });
            }).catch(error => {
                console.error('Error fetching GIFs:', error);
                alert('Failed to fetch GIFs. Please try again.');
            });
        });
    }

    // Function to search GIFs
    function searchGifs(query) {
        const url = `https://api.giphy.com/v1/gifs/search?api_key=${giphyApiKey}&q=${query}&limit=9`;
        return fetch(url)
            .then(response => response.json())
            .then(data => data.data);
    }






    //############## Recensioni ##############

    // Review Form validation and submission
    document.querySelectorAll('[id^="toggleEditBtn-"]').forEach(button => {
        button.addEventListener('click', function () {
            const reviewId = this.id.split('-')[1]; // Extract the reviewId from the button ID
            toggleEditMode(reviewId);
        });
    });
    window.toggleEditMode = function (reviewId) {
        const reviewContentDiv = document.getElementById('review-content-' + reviewId);
        const editFormDiv = document.getElementById('edit-review-form-' + reviewId);

        if (editFormDiv && reviewContentDiv) {
            if (editFormDiv.classList.contains('d-none')) {
                // Mostra il modulo di modifica
                editFormDiv.classList.remove('d-none');
                reviewContentDiv.classList.add('d-none');

                // Riempie i campi del modulo con i valori della recensione
                document.getElementById('Title-' + reviewId).value = reviewContentDiv.getAttribute('data-title');
                document.getElementById('Description-' + reviewId).value = reviewContentDiv.getAttribute('data-description');
                document.getElementById('Rating-' + reviewId).value = reviewContentDiv.getAttribute('data-rating');
            } else {
                // Nascondi il modulo di modifica
                editFormDiv.classList.add('d-none');
                reviewContentDiv.classList.remove('d-none');
            }
        } else {
            console.error(`Element(s) with ID(s) "review-content-${reviewId}" or "edit-review-form-${reviewId}" not found.`);
        }
    }

    window.submitEditForm = function (reviewId) {
        const form = document.querySelector(`#edit-review-form-${reviewId} form`);
        if (form) {
            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.getElementById('review-title-' + reviewId).textContent = 'Titolo: ' + data.review.title;
                        document.getElementById('review-description-' + reviewId).textContent = 'Contenuto: ' + data.review.description;

                        // Genera le stelle dinamicamente
                        const ratingElement = document.getElementById('review-rating-' + reviewId);
                        ratingElement.innerHTML = 'Valutazione: '; // resetta il contenuto
                        for (let i = 0; i < data.review.rating; i++) {
                            const star = document.createElement('i');
                            star.classList.add('fa', 'fa-star', 'text-orange');
                            ratingElement.appendChild(star);
                        }

                        toggleEditMode(reviewId); // Torna alla modalità di visualizzazione
                    } else {
                        alert('Errore nell\'aggiornamento della recensione: ' + data.error);
                    }
                })
                .catch(error => {
                    console.error('Errore:', error);
                    alert('Si è verificato un errore durante il tentativo di aggiornare la recensione.');
                });

            return false; // Evita il submit tradizionale del form
        }
    }
    window.previewImage = function (reviewId) {
        const inputFile = document.getElementById('imageFiles-' + reviewId);
        const previewImg = document.getElementById('previewImg-' + reviewId);
        const reviewSectionImg = document.getElementById('reviewSectionImg-' + reviewId); // Immagine nella sezione "Review Image"

        if (inputFile.files && inputFile.files.length > 0) {
            const reader = new FileReader();

            reader.onload = function (e) {
                previewImg.src = e.target.result;  // Aggiorna solo l'anteprima
                reviewSectionImg.src = e.target.result;  // Aggiorna solo l'immagine temporaneamente
            };

            // Leggi il primo file caricato dall'utente
            reader.readAsDataURL(inputFile.files[0]);
        }
    }
});

// Funzione per eliminare una recensione
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".deleteReviewBtn").forEach(button => {
        button.addEventListener("click", function (event) {
            const reviewId = this.getAttribute("data-review-id");
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            if (confirm("Sei sicuro di voler eliminare questa recensione?")) {
                fetch(`/Event/DeleteReview?reviewId=${reviewId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token
                    }
                })
                    .then(response => {
                        if (response.ok) {
                            // Rimuovi la recensione dal DOM
                            document.getElementById(`review-content-${reviewId}`).remove();

                        } else {
                            alert('Si è verificato un errore durante l\'eliminazione della recensione.');
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        alert('Si è verificato un errore durante l\'eliminazione della recensione.');
                    });
            }
        });
    });
});

// Funzione per mostrare/nascondere il modulo di recensione
$(document).ready(function () {
    function toggleForm() {
        var form = $('#reviewForm');
        var btn = $('#reviewBtn');

        if (form.is(':visible')) {
            form.hide();
            btn.html('Aggiungi una recensione ' + '<i class="bi bi-pencil-square">');
        } else {
            form.show();
            btn.text('Nascondi');
        }
    }

    // Aggiungi l'evento di click al pulsante
    $('#reviewBtn').click(toggleForm);
});





// ############## Google Maps ##############

function initMap() {
    const defaultLocation = { lat: 41.9028, lng: 12.4964 }; // Default to Rome

    map = new google.maps.Map(document.getElementById("map"), {
        center: defaultLocation,
        zoom: 13,
    });

    marker = new google.maps.Marker({
        map: map,
        draggable: true,
        position: defaultLocation,
    });

    const geocoder = new google.maps.Geocoder();
    const address = eventAddress;

    if (address) {
        geocoder.geocode({ address: address }, function (results, status) {
            if (status === 'OK') {
                map.setCenter(results[0].geometry.location);
                map.setZoom(15);
                marker.setPosition(results[0].geometry.location);
            } else {
                alert("Geocode was not successful for the following reason: " + status);
            }
        });
    } else {
        marker.setPosition(defaultLocation);
    }
}





// ############## Commenti ##############

// Funzione per mostrare/nascondere il form per aggiungere un commento
$(document).ready(function () {
    function toggleCommentForm() {
        var form = $('#commentForm');
        var btn = $('#commentBtn');

        if (form.is(':visible')) {
            form.hide();
            btn.html('Aggiungi un commento ' + '<i class="bi bi-pencil-square">');
        } else {
            form.show();
            btn.text('Nascondi');
        }
    }

    // Aggiungi l'evento di click al pulsante
    $('#commentBtn').click(toggleCommentForm);
});


// Seleziona tutti i bottoni di modifica dei commenti e scatena la funziona per mostrare il form di modifica
document.addEventListener('DOMContentLoaded', function () {
    const editButtons = document.querySelectorAll('.editCommentBtn');

    editButtons.forEach(button => {
        button.addEventListener('click', function () {
            const commentId = this.getAttribute('data-comment-id');
            toggleEditCommentMode(commentId); // Mostra il form di modifica del commento
        });
    });
});

document.querySelectorAll('[id^="toggleEditCommentBtn-"]').forEach(button => {
    button.addEventListener('click', function () {
        const reviewId = this.id.split('-')[1]; // Extract the reviewId from the button ID
        toggleEditCommentMode(reviewId);
    });
});

// Funzione per mostrare/nascondere il form di modifica del commento

function toggleEditCommentMode(commentId) {
    const commentContentDiv = document.getElementById('comment-content-' + commentId);
    const editFormDiv = document.getElementById('edit-comment-form-' + commentId);

    if (editFormDiv && commentContentDiv) {
        if (editFormDiv.classList.contains('d-none')) {
            // Mostra il modulo di modifica
            editFormDiv.classList.remove('d-none');

            // Riempie i campi del modulo con i valori del commento
            document.getElementById('Description-' + commentId).value = commentContentDiv.getAttribute('data-description');
        } else {
            // Nascondi il modulo di modifica
            editFormDiv.classList.add('d-none');
        }
    } else {
        console.error(`Element(s) with ID(s) "comment-content-${commentId}" or "edit-comment-form-${commentId}" not found.`);
    }
}

// Funzione per inviare il form di modifica del commento
window.submitEditCommentForm = function (commentId) {
    const form = document.querySelector(`#edit-comment-form-${commentId} form`);
    if (form) {
        const formData = new FormData(form);

        fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(response => response.json())
            .then(data => {
                console.log('Server Response:', data); // Aggiungi il log per debugging
                if (data.success) {
                    document.getElementById('comment-description-' + commentId).textContent = data.comment.description;

                    toggleEditCommentMode(commentId); // Torna alla modalità di visualizzazione
                } else {
                    alert('Errore nell\'aggiornamento del commento: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Errore durante il fetch:', error);
                alert('Si è verificato un errore durante il tentativo di aggiornare il commento.');
            });

        return false; // Evita il submit tradizionale del form
    }
}


// Eliminazione commento con fetch API
document.addEventListener('DOMContentLoaded', function () {
    const deleteButtons = document.querySelectorAll('.deleteCommentBtn');

    deleteButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            const commentId = this.getAttribute('data-comment-id');

            if (confirm('Sei sicuro di voler eliminare questo commento?')) {
                // Ottieni il valore del token antifrode
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                fetch(`/Comment/DeleteComment?commentId=${commentId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token  // Aggiungi il token agli headers
                    }
                })
                    .then(response => {
                        if (response.ok) {
                            const commentElement = document.getElementById(`comment-content-${commentId}`).closest('.main-comment, .reply-comment');
                            commentElement.remove();
                        } else {
                            alert('Si è verificato un errore durante l\'eliminazione del commento.');
                        }
                    })
                    .catch(error => {
                        console.error('Errore nella richiesta:', error);
                        alert('Si è verificato un errore.');
                    });
            }
        });
    });
});



// Funzione per aggiornare il numero di like ad un commento e lo stato del bottone
document.addEventListener('DOMContentLoaded', function () {
    function updateLikeCount(commentId) {
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

    // Funzione per aggiungere o rimuovere un like ad un commento
    function toggleLike(commentId) {
        fetch(`/Comment/ToggleLike/${commentId}`, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
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
                console.error('Errore durante l\'aggiornamento dei like:', error);
            });
    }

    // Aggiorna il numero di like e lo stato del bottone al caricamento della pagina
    document.querySelectorAll('.like-button').forEach(button => {
        const commentId = button.getAttribute('data-comment-id');
        updateLikeCount(commentId);

        button.addEventListener('click', function () {
            toggleLike(commentId);
        });
    });

    // Funzione per mostrare il nome dei like associati di un commento
    const likeModal = document.getElementById('likeModal');
    likeModal.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget; // Bottone che ha aperto il modale
        const commentId = button.getAttribute('data-comment-id'); // Ottieni l'ID del commento

        // Recupera la lista dei like associati al commento
        fetch(`/Comment/getcommentlikes?commentId=${commentId}`)
            .then(response => response.json())
            .then(data => {
                const likeList = document.getElementById('likeList');
                likeList.innerHTML = ''; // Resetta la lista

                // Aggiungi ogni utente alla lista del modale
                data.likes.forEach(user => {
                    const li = document.createElement('li');
                    li.classList.add('list-group-item', 'text-white', 'bg-transparent', 'border-0');
                    li.textContent = user.username; // Accedi alla proprietà 'username'
                    likeList.appendChild(li);
                });

                console.log('Total likes:', data.likeCount);
            })
            .catch(error => {
                console.error('Errore durante il recupero dei like:', error);
            });
    });
});

// Funzione per nascondere il form di risposta ad un commento
document.addEventListener("DOMContentLoaded", function () {
    // Aggiungi l'evento click a tutti i bottoni con la classe 'cancel-reply'
    document.querySelectorAll('.cancel-reply').forEach(button => {
        button.addEventListener('click', function () {
            // Recupera l'ID del commento dal data attribute
            const commentId = this.getAttribute('data-comment-id');
            // Seleziona il form di risposta corrispondente e nascondilo
            const replyForm = document.getElementById(`replyForm-${commentId}`);
            if (replyForm) {
                replyForm.style.display = 'none';
            }
        });
    });
});


// Funzione per mostrare form di risposta ad un commento
document.querySelectorAll('.reply-button').forEach(button => {
    button.addEventListener('click', function () {
        const commentId = this.getAttribute('data-comment-id');
        const replyForm = document.getElementById('replyForm-' + commentId);

        if (replyForm) {
            replyForm.style.display = (replyForm.style.display === 'none' || replyForm.style.display === '') ? 'block' : 'none';
        }
    });
});