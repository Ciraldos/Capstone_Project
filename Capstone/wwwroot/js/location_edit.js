let map;
let marker;
let autocomplete;

function initMap() {
    const defaultLocation = { lat: 41.9028, lng: 12.4964 }; // Default to Rome if no location is provided

    map = new google.maps.Map(document.getElementById("map"), {
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

        // Update map and marker position
        map.setCenter(place.geometry.location);
        map.setZoom(15);
        marker.setPosition(place.geometry.location);

        // Populate the address field
        document.getElementById("addressGoogleApi").value = place.formatted_address;
    });

    // Retrieve values from data attributes
    const locationInput = document.getElementById('locationName');
    const locationName = locationInput.dataset.locationName;
    const addressGoogleApi = document.getElementById('addressGoogleApi').dataset.addressGoogleApi;

    if (locationName && addressGoogleApi) {
        locationInput.value = locationName;
        document.getElementById('addressGoogleApi').value = addressGoogleApi;

        // Geocode the existing address to update the map
        const geocoder = new google.maps.Geocoder();
        geocoder.geocode({ address: addressGoogleApi }, function (results, status) {
            if (status === 'OK') {
                map.setCenter(results[0].geometry.location);
                marker.setPosition(results[0].geometry.location);
            } else {
                console.error('Geocode was not successful for the following reason: ' + status);
            }
        });
    }
}

window.onload = function () {
    if (typeof google !== 'undefined') {
        initMap();
    } else {
        console.error('Google Maps API is not loaded');
    }
};
