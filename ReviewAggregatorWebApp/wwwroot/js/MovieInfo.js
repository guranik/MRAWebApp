let selectedRating = 0;
let currentPage = 1;
const movieId = document.getElementById('movie').getAttribute('data-movie-id');

document.querySelectorAll('.star').forEach(star => {
    star.addEventListener('click', function () {
        selectedRating = this.getAttribute('data-value');
        document.querySelectorAll('.star').forEach(s => {
            s.classList.remove('selected');
        });
        for (let i = 0; i < selectedRating; i++) {
            document.querySelectorAll('.star')[i].classList.add('selected');
        }
    });
});

function submitReview() {
    const reviewText = $('#review-text').val();

    if (selectedRating === 0 || reviewText.trim() === "") {
        alert("Пожалуйста, выберите рейтинг и введите текст отзыва.");
        return;
    }

    const reviewData = {
        MovieId: movieId,
        Rating: selectedRating,
        ReviewText: reviewText
    };

    const createReviewUrl = document.getElementById('review-url').getAttribute('data-url');

    $.ajax({
        type: 'POST',
        url: createReviewUrl,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(reviewData),
        dataType: "json",
        success: function (data) {
            if (data.success) {
                alert("Отзыв успешно добавлен!");
                loadReviews(movieId, 1); // Reload reviews
                $('#review-text').val(""); // Clear the text area
                selectedRating = 0; // Reset rating
                $('.star').removeClass('selected'); // Reset stars
            } else {
                alert("Ошибка при добавлении отзыва.");
            }
        },
        error: function () {
            alert("Ошибка при отправке запроса.");
        }
    });
}

function loadReviews(movieId, page) {
    currentPage = page;
    fetch(`/Reviews/GetReviews?movieId=${movieId}&page=${currentPage}`)
        .then(response => response.json())
        .then(data => {
            const reviewsContainer = document.getElementById('reviews-container');
            reviewsContainer.innerHTML = '';

            data.reviews.forEach(review => {
                const reviewTemplate = document.getElementById('review-template');
                const reviewElement = reviewTemplate.cloneNode(true);
                reviewElement.style.display = '';

                reviewElement.querySelector('.review-user-name').innerText = review.userName;
                reviewElement.querySelector('.review-text').innerText = review.reviewText;
                reviewElement.querySelector('.review-rating').innerText = `Рейтинг: ${review.rating}`;
                reviewElement.querySelector('.review-date').innerText = `Дата: ${new Date(review.postDate).toLocaleDateString()}`;
                const innerReviewElement = reviewElement.querySelector('.review');
                innerReviewElement.setAttribute('data-review-id', review.id);

                reviewsContainer.appendChild(reviewElement);
            });

            updatePagination(data.totalPages);
        });
}

function updatePagination(totalPages) {
    const prevButton = document.getElementById('prev-page');
    const nextButton = document.getElementById('next-page');

    prevButton.disabled = currentPage === 1;
    nextButton.disabled = currentPage === totalPages;
}

// При загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    currentPage = 1;
    loadReviews(movieId, currentPage);
});