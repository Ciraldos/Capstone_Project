// Toggle review edit mode
function toggleEditMode(reviewId) {
    var reviewContentDiv = $('#review-content-' + reviewId);
    var editFormDiv = $('#edit-review-form-' + reviewId);

    if (editFormDiv.css('display') === 'none') {
        editFormDiv.css('display', 'block');   // Show the edit form
        reviewContentDiv.css('display', 'none'); // Hide the review content
    } else {
        editFormDiv.css('display', 'none');    // Hide the edit form
        reviewContentDiv.css('display', 'block');  // Show the review content
    }
}

// Submit edit form via AJAX
function submitEditForm(reviewId) {
    var form = $('#edit-review-form-' + reviewId + ' form');
    var formData = new FormData(form[0]);

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (data) {
            if (data.success) {
                $('#review-title-' + reviewId).text(data.review.title);
                $('#review-description-' + reviewId).text(data.review.description);
                $('#review-rating-' + reviewId).text(data.review.rating);

                toggleEditMode(reviewId); // Switch back to view mode
            } else {
                alert('Error updating review: ' + data.error);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('An error occurred while updating the review.');
        }
    });

    return false; // Prevent default form submission
}

// GIF section toggle and search
$(document).ready(function () {
    $('#addGifSectionBtn').click(function () {
        $('#gifSection').toggle();
        $('#gifSearch').val(''); // Clear the search input
        $('#gifResults').empty(); // Clear the search results
        $('#selectedGifContainer').hide(); // Hide the selected GIF container
    });

    $('#gifSearch').on('input', function () {
        var query = $('#gifSearch').val().trim();
        if (query === '') {
            $('#gifResults').empty(); // Clear results if query is empty
            return;
        }

        searchGifs(query).done(function (gifs) {
            var $gifResults = $('#gifResults');
            $gifResults.empty(); // Clear previous results

            gifs.forEach(function (gif) {
                var gifUrl = gif.images.fixed_height.url;
                var $imgElement = $('<img>').attr('src', gifUrl)
                    .addClass('img-thumbnail m-2')
                    .css('cursor', 'pointer')
                    .click(function () {
                        $('#selectedGif').attr('src', gifUrl);
                        $('#hiddenGifUrl').val(gifUrl);
                        $('#selectedGifContainer').show(); // Show the selected GIF container
                        $('#gifResults').empty(); // Clear the GIF search results
                    });

                $gifResults.append($imgElement);
            });
        }).fail(function (error) {
            console.error('Error fetching GIFs:', error);
            alert('Failed to fetch GIFs. Please try again.');
        });
    });

    function searchGifs(query) {
        var url = `https://api.giphy.com/v1/gifs/search?api_key=${giphyApiKey}&q=${query}&limit=9`;
        return $.ajax({
            url: url,
            method: 'GET'
        }).then(function (response) {
            return response.data;
        });
    }
});

// Like button functionality
$(document).ready(function () {
    $('.like-button').click(function () {
        var button = $(this);
        var commentId = button.data('comment-id');

        $.ajax({
            url: toggleLikeUrl,
            type: 'POST',
            data: { id: commentId },
            success: function (response) {
                var likeCountSpan = $('.like-count[data-comment-id="' + commentId + '"]');
                var likedByList = $('ul[data-comment-id="' + commentId + '"]');

                likeCountSpan.text(response.likeCount);

                likedByList.empty();
                response.likes.forEach(function (username) {
                    likedByList.append('<li class="small text-danger">' + username + '</li>');
                });
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                alert('An error occurred during the like/unlike operation.');
            }
        });
    });

    // Load initial like counts and lists
    $('.like-count').each(function () {
        var commentId = $(this).data('comment-id');
        $.ajax({
            url: getCommentLikesUrl,
            type: 'GET',
            data: { commentId: commentId },
            success: function (response) {
                var likeCountSpan = $('.like-count[data-comment-id="' + commentId + '"]');
                var likedByList = $('ul[data-comment-id="' + commentId + '"]');

                likeCountSpan.text(response.likeCount);

                likedByList.empty();
                response.likes.forEach(function (username) {
                    likedByList.append('<li class="small text-danger">' + username + '</li>');
                });
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
            }
        });
    });
});

    document.addEventListener('DOMContentLoaded', function () {
        const replyButtons = document.querySelectorAll('.reply-button');

        replyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const commentId = this.getAttribute('data-comment-id');
            const replyForm = document.getElementById('replyForm-' + commentId);

            // Toggle visibility of the reply form
            if (replyForm.style.display === 'none' || replyForm.style.display === '') {
                replyForm.style.display = 'block';
            } else {
                replyForm.style.display = 'none';
            }
        });
        });
    });


// Google Maps functionality

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

