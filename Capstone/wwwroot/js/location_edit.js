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

        // Update map and marker location
        map.setCenter(place.geometry.location);
        map.setZoom(15);
        marker.setPosition(place.geometry.location);

        // Populate address field
        document.getElementById("addressGoogleApi").value = place.formatted_address;
    });

    // If the location is already set, initialize with that location
    const locationName = '@Model.LocationName';
    const addressGoogleApi = '@Model.AddressGoogleApi';

    if (locationName && addressGoogleApi) {
        autocomplete.setBounds(map.getBounds());
        document.getElementById('locationName').value = locationName;
        document.getElementById('addressGoogleApi').value = addressGoogleApi;

        // Perform a search to update the map and marker
        const geocoder = new google.maps.Geocoder();
        geocoder.geocode({ address: addressGoogleApi }, function (results, status) {
            if (status === 'OK') {
                map.setCenter(results[0].geometry.location);
                marker.setPosition(results[0].geometry.location);
            }
        });
    }
}

window.onload = initMap;