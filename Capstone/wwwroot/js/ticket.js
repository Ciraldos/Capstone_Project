function confirmDelete(ticketNum, ticketId) {
    if (confirm(`Sei sicuro di voler eliminare il biglietto N° ${ticketNum}?`)) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value; // Ottieni il token antiforgery
        const url = '/Ticket/Delete/' + ticketId; // URL per la richiesta di eliminazione

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token // Aggiungi il token antiforgery nell'intestazione
            },
            body: JSON.stringify({ id: ticketId }) // Invia l'ID del ticket nel corpo della richiesta
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Errore durante la richiesta di eliminazione.');
                }
                return response.json();
            })
            .then(result => {
                if (result.success) {
                    // Rimuovi la card visivamente
                    document.getElementById('ticket-' + ticketId).remove();
                } else {
                    alert('Errore durante l\'eliminazione del biglietto.');
                }
            })
            .catch(error => {
                alert(error.message);
            });
    }
}

document.querySelectorAll('[id^="delete-button-"]').forEach(button => {
    button.addEventListener('click', function (event) {
        event.preventDefault(); // Previene il comportamento predefinito del form
        const ticketId = this.id.split('-')[2]; // Estrai l'ID del ticket dall'ID del bottone
        const ticketNum = this.getAttribute('data-ticket-num'); // Ottieni il numero del ticket da un attributo personalizzato

        confirmDelete(ticketNum, ticketId);
    });
});