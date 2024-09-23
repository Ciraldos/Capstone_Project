// Carica la libreria Google Charts
google.charts.load('current', { packages: ['corechart'] });

// Imposta una callback quando Google Charts è pronto
google.charts.setOnLoadCallback(drawCharts);

// Funzione che disegna tutti i grafici
function drawCharts() {
    drawTicketsPerDayChart();
    drawReviewsPerEventChart();
    drawLikesPerCommentChart();
    drawEventsPerDjChart();
    drawSalesByTypeChart();
    drawReviewsPerEventTypeChart();
    drawDjPopularityChart();
}

// Funzione per disegnare il grafico dei biglietti venduti per giorno
function drawTicketsPerDayChart() {
    var data = google.visualization.arrayToDataTable([
        ['Giorno', 'Biglietti Venduti'],
        // Inserisci i dati dal server qui
        ['2024-09-01', 100],
        ['2024-09-02', 150],
        ['2024-09-03', 200],
    ]);

    var options = {
        title: 'Biglietti Venduti per Giorno',
        legend: { position: 'bottom' },
        hAxis: { title: 'Giorno' },
        vAxis: { title: 'Biglietti Venduti' }
    };

    var chart = new google.visualization.LineChart(document.getElementById('chart_tickets_per_day'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico delle recensioni per evento
function drawReviewsPerEventChart() {
    var data = google.visualization.arrayToDataTable([
        ['Evento', 'Recensioni'],
        // Inserisci i dati dal server qui
        ['Evento A', 50],
        ['Evento B', 75],
        ['Evento C', 100],
    ]);

    var options = {
        title: 'Recensioni per Evento',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.PieChart(document.getElementById('chart_reviews_per_event'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico dei mi piace per commento
function drawLikesPerCommentChart() {
    var data = google.visualization.arrayToDataTable([
        ['Commento', 'Mi Piace'],
        // Inserisci i dati dal server qui
        ['Commento 1', 30],
        ['Commento 2', 45],
        ['Commento 3', 60],
    ]);

    var options = {
        title: 'Mi Piace per Commento',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.ColumnChart(document.getElementById('chart_likes_per_comment'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico degli eventi per DJ
function drawEventsPerDjChart() {
    var data = google.visualization.arrayToDataTable([
        ['DJ', 'Eventi'],
        // Inserisci i dati dal server qui
        ['DJ A', 10],
        ['DJ B', 15],
        ['DJ C', 20],
    ]);

    var options = {
        title: 'Eventi per DJ',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.BarChart(document.getElementById('chart_events_per_dj'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico delle vendite per tipo di biglietto
function drawSalesByTypeChart() {
    var data = google.visualization.arrayToDataTable([
        ['Tipo di Biglietto', 'Biglietti Venduti'],
        // Inserisci i dati dal server qui
        ['VIP', 100],
        ['Standard', 200],
        ['Economico', 150],
    ]);

    var options = {
        title: 'Vendite per Tipo di Biglietto',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.PieChart(document.getElementById('chart_sales_by_type'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico delle recensioni per tipo di evento
function drawReviewsPerEventTypeChart() {
    var data = google.visualization.arrayToDataTable([
        ['Tipo di Evento', 'Recensioni'],
        // Inserisci i dati dal server qui
        ['Techno', 120],
        ['House', 150],
        ['EDM', 100],
    ]);

    var options = {
        title: 'Recensioni per Tipo di Evento',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.ColumnChart(document.getElementById('chart_reviews_per_event_type'));
    chart.draw(data, options);
}

// Funzione per disegnare il grafico della popolarità dei DJ
function drawDjPopularityChart() {
    var data = google.visualization.arrayToDataTable([
        ['DJ', 'Eventi', 'Recensioni'],
        // Inserisci i dati dal server qui
        ['DJ A', 10, 50],
        ['DJ B', 15, 75],
        ['DJ C', 20, 100],
    ]);

    var options = {
        title: 'Popolarità dei DJ',
        hAxis: { title: 'DJ' },
        vAxis: { title: 'Numero' },
        seriesType: 'bars',
        series: { 1: { type: 'line' } } // Linea per le recensioni
    };

    var chart = new google.visualization.ComboChart(document.getElementById('chart_dj_popularity'));
    chart.draw(data, options);
}
