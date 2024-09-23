let map;
let marker;
let geocoder;

function initMap() {
    // Default location (Rome)
    const defaultLocation = { lat: 41.9028, lng: 12.4964 };

    // Initialize the map
    map = new google.maps.Map(document.getElementById("map"), {
        center: defaultLocation,
        zoom: 13,
    });

    // Create a geocoder instance
    geocoder = new google.maps.Geocoder();

    // Use the address passed from Razor
    if (locationAddress) {
        geocodeAddress(locationAddress);
    }
}

function geocodeAddress(address) {
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            // Center the map on the geocoded location
            map.setCenter(results[0].geometry.location);
            map.setZoom(15); // Zoom in closer to the location

            // Place the marker on the geocoded location
            marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location,
            });
        } else {
            console.error('Geocode was not successful for the following reason: ' + status);
        }
    });
}

// Ensure the map initializes when the page is fully loaded
window.addEventListener('DOMContentLoaded', initMap);
