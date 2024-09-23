document.addEventListener("DOMContentLoaded", function () {
    // Funzione per calcolare il prezzo totale
    function calculateTotalPrice() {
        let totalPrice = 0;

        document.querySelectorAll('.event-card').forEach(function (row) {
            const quantity = parseInt(row.querySelector('.event-quantity p.fs-4').textContent.split('X ')[1]);
            // Sostituisci la virgola con un punto e poi converti in numero
            const priceText = row.querySelector('.event-price p.fs-5').textContent.replace('€', '').trim().replace(',', '.');
            const price = parseFloat(priceText);

            totalPrice += quantity * price;
        });

        document.getElementById('price').innerText = `Totale: €${totalPrice.toFixed(2)}`;
    }

    function updateCartVisibility() {
        const cartItems = document.querySelectorAll('.event-card');
        const divCart = document.getElementById('noEvents');
        const checkoutButton = document.getElementById('checkoutButton');
        const divBg = document.getElementById('divBg');

        if (cartItems.length === 0) {
            divCart.classList.remove('d-none');
            divBg.classList.remove('bg');
            checkoutButton.disabled = true; // Disabilita il pulsante di checkout
        } else {
            divCart.classList.add('d-none');
            divBg.classList.add('bg');
            checkoutButton.disabled = false; // Abilita il pulsante di checkout
        }
    }


    // Gestisci il pulsante per rimuovere un biglietto
    document.querySelectorAll(".remove-item-btn").forEach(function (button) {
        button.addEventListener("click", function (event) {
            event.preventDefault();

            var eventId = this.getAttribute("data-event-id");
            var ticketTypeId = this.getAttribute("data-ticket-type-id");
            var row = document.getElementById(`cart-item-${eventId}-${ticketTypeId}`);
            var quantityDisplay = row.querySelector('.event-quantity p.fs-4');
            var currentQuantity = parseInt(quantityDisplay.textContent.split('X ')[1]);

            if (currentQuantity > 0) {
                var url = `/Cart/RemoveFromCart?eventId=${eventId}&ticketTypeId=${ticketTypeId}`;
                fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Network response was not ok: ' + response.statusText);
                        }
                        return response.json();
                    })
                    .then(data => {
                        if (data.success) {
                            currentQuantity--;
                            quantityDisplay.textContent = `X ${currentQuantity}`;

                            if (currentQuantity === 0) {
                                row.remove(); // Rimuove l'intera riga se la quantità è zero
                            }
                            calculateTotalPrice(); // Ricalcola il totale subito dopo la rimozione
                            updateCartVisibility();
                        } else {
                            alert(data.message);
                        }
                    })
                    .catch(error => {
                        console.error("Errore:", error);
                        alert("Si è verificato un errore durante la rimozione del biglietto.");
                    });
            }
        });
    });

    // Gestisci il pulsante per aggiungere un biglietto
    document.querySelectorAll(".add-item-btn").forEach(function (button) {
        button.addEventListener("click", function (event) {
            event.preventDefault();

            var eventId = this.getAttribute("data-event-id");
            var ticketTypeId = this.getAttribute("data-ticket-type-id");
            var row = document.getElementById(`cart-item-${eventId}-${ticketTypeId}`);
            var quantityDisplay = row.querySelector('.event-quantity p.fs-4');
            var currentQuantity = parseInt(quantityDisplay.textContent.split('X ')[1]);

            var url = `/Cart/UpdateCart?eventId=${eventId}&ticketTypeId=${ticketTypeId}`;
            fetch(url, {
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok: ' + response.statusText);
                    }
                    return response.json();
                })
                .then(data => {
                    console.log(data);
                    if (data.success) {
                        currentQuantity++;
                        quantityDisplay.textContent = `X ${currentQuantity}`;
                        calculateTotalPrice(); // Ricalcola il totale subito dopo l'aggiunta
                        updateCartVisibility();
                    } else {
                        alert(data.message);
                    }
                })
                .catch(error => {
                    console.error("Errore:", error);
                });
        });
    });

    // Calcola il prezzo totale all'apertura della pagina
    calculateTotalPrice();
    updateCartVisibility();
});
