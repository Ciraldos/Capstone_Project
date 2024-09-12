let map;
let marker;
let autocomplete;

function initMap() {
    const defaultLocation = { lat: 41.9028, lng: 12.4964 }; // Rome

    const mapElement = document.getElementById("map");
    if (mapElement) {
        map = new google.maps.Map(mapElement, {
            center: defaultLocation,
            zoom: 13,
        });

        marker = new google.maps.Marker({
            map: map,
            draggable: true,
            position: defaultLocation,
        });

        autocomplete = new google.maps.places.Autocomplete(document.getElementById('locationName'));
        autocomplete.bindTo('bounds', map);

        autocomplete.addListener('place_changed', function () {
            let place = autocomplete.getPlace();
            if (!place.geometry) {
                alert("No details available for this location.");
                return;
            }

            // Update map and marker location
            map.setCenter(place.geometry.location);
            map.setZoom(15);
            marker.setPosition(place.geometry.location);

            // Populate address field
            document.getElementById("addressGoogleApi").value = place.formatted_address;
        });
    } else {
        console.error("Map element not found");
    }
}

window.addEventListener('DOMContentLoaded', initMap);