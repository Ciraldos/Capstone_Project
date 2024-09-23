// Trigger the search as the user types in the search input
document.getElementById("searchArtist").addEventListener("input", function () {
    let query = this.value.trim();
    if (query.length > 2) { // Trigger search only if input is at least 3 characters
        searchSpotifyArtist(query);
    } else {
        document.getElementById("searchResults").innerHTML = ""; // Clear search results for empty input
        document.getElementById("searchResults").style.display = "none"; // Hide results if input is less than 3 characters
    }
});

function searchSpotifyArtist(query) {
    fetch(`/Dj/SearchSpotifyArtist?query=${encodeURIComponent(query)}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then(data => {
            let searchResults = document.getElementById("searchResults");
            searchResults.style.display = "block"; // Show results list
            searchResults.innerHTML = ""; // Clear previous results
            if (data.artists && data.artists.items && data.artists.items.length > 0) {
                data.artists.items.forEach(artist => {
                    // Create the list item
                    let li = document.createElement("li");
                    li.classList.add("list-group-item", "d-flex", "align-items-center", "bg-black", "px-4");
                    li.style.cursor = "pointer";


                    // Create the artist image (if available)
                    let img = document.createElement("img");
                    if (artist.images && artist.images.length > 0) {
                        img.src = artist.images[0].url;
                    } else {
                        img.src = "https://via.placeholder.com/50"; // Fallback image if artist has no image
                    }
                    img.alt = artist.name;
                    img.classList.add("img-thumbnail", "mr-3");
                    img.style.width = "70px";
                    img.style.height = "70px";

                    // Create the artist name element
                    let artistName = document.createElement("span");
                    artistName.textContent = artist.name;
                    artistName.classList.add("text-light", "ms-4");


                    // Add event listener to populate input fields when clicked and hide results
                    li.addEventListener("click", function () {
                        document.getElementById("artistName").value = artist.name;
                        document.getElementById("artistSpotifyId").value = artist.id;

                        // Hide the search results after artist is selected
                        searchResults.innerHTML = "";
                        searchResults.style.display = "none";
                    });

                    // Append image and name to list item
                    li.appendChild(img);
                    li.appendChild(artistName);
                    searchResults.appendChild(li);
                });
            } else {
                searchResults.innerHTML = '<li class="list-group-item">Nessun artista trovato.</li>';
            }
        })
        .catch(error => {
            document.getElementById("searchResults").innerHTML = '<li class="list-group-item text-danger">Error: ' + error.message + '</li>';
            console.error('Error:', error);
        });
}