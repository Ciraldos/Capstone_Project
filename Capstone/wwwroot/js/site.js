function toggleEditMode(reviewId) {
    var reviewContentDiv = document.getElementById('review-content-' + reviewId);
    var editFormDiv = document.getElementById('edit-review-form-' + reviewId);

    if (editFormDiv.style.display === 'none') {
        editFormDiv.style.display = 'block';   // Show the edit form
        reviewContentDiv.style.display = 'none'; // Hide the review content
    } else {
        editFormDiv.style.display = 'none';    // Hide the edit form
        reviewContentDiv.style.display = 'block';  // Show the review content
    }
}

// Function to handle AJAX form submission
function submitEditForm(reviewId) {
    var form = document.querySelector(`#edit-review-form-${reviewId} form`);
    var formData = new FormData(form);

    // Send the form data via AJAX (fetch)
    fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest' // To distinguish between AJAX and normal requests
        }
    })
        .then(response => response.json())  // Assuming the server returns JSON
        .then(data => {
            if (data.success) {
                // Update the UI with the new review data
                document.getElementById('review-title-' + reviewId).innerText = data.review.title;
                document.getElementById('review-description-' + reviewId).innerText = data.review.description;
                document.getElementById('review-rating-' + reviewId).innerText = data.review.rating + '/5';

                // Toggle back to view mode
                toggleEditMode(reviewId);
            } else {
                alert('Error updating review: ' + data.error);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while trying to update the review.');
        });

    // Prevent the form from submitting the traditional way
    return false;
}
