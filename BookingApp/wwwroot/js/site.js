let currentIndex = 0;
let carouselItems = document.getElementsByClassName("carousel-item");
let indicators = document.getElementsByClassName("indicator-dot");
let autoSlideTimer;

function setupCarousel() {

    carouselItems = document.getElementsByClassName("carousel-item");
    indicators = document.getElementsByClassName("indicator-dot");

    if (carouselItems.length === 0) {
        console.log("Carousel items not found, retrying...");
        setTimeout(setupCarousel, 100);
        return;
    }

    const slideTotal = document.getElementById("slide-total");
    if (slideTotal) {
        slideTotal.innerText = carouselItems.length;
    }

  
    if (autoSlideTimer) {
        clearInterval(autoSlideTimer);
    }


    setupTouchEvents();

    beginAutoSlide();
    console.log("Carousel initialized and autoplay started");
}


function displaySlide(index) {
    if (index >= carouselItems.length) { currentIndex = 0; }
    if (index < 0) { currentIndex = carouselItems.length - 1; }

    for (let i = 0; i < carouselItems.length; i++) {
        carouselItems[i].classList.remove("show", "smooth-transition");
    }

    for (let i = 0; i < indicators.length; i++) {
        indicators[i].classList.remove("current");
    }

 
    setTimeout(() => {
        if (carouselItems[currentIndex]) {
            carouselItems[currentIndex].classList.add("show", "smooth-transition");
        }
        if (indicators[currentIndex]) {
            indicators[currentIndex].classList.add("current");
        }
        const activeSlide = document.getElementById("active-slide");
        if (activeSlide) {
            activeSlide.innerText = currentIndex + 1;
        }
    }, 50);
}


function navigateSlide(direction) {
    currentIndex += direction;
    displaySlide(currentIndex);
}


function jumpToSlide(slideNumber) {
    currentIndex = slideNumber - 1;
    displaySlide(currentIndex);
}


function advanceSlide() {
    currentIndex++;
    displaySlide(currentIndex);
}


function beginAutoSlide() {
    autoSlideTimer = setInterval(function () {
        console.log("Auto-advancing to next slide");
        advanceSlide();
    }, 4000); 
}


document.addEventListener('keydown', (event) => {
    if (event.key === 'ArrowLeft') navigateSlide(-1);
    if (event.key === 'ArrowRight') navigateSlide(1);
});


let touchStartX = null;

function setupTouchEvents() {
    const carousel = document.querySelector('.image-carousel');
    if (!carousel) return;

    carousel.addEventListener('touchstart', (event) => {
        touchStartX = event.touches[0].clientX;
    });

    carousel.addEventListener('touchend', (event) => {
        if (!touchStartX) return;

        const touchEndX = event.changedTouches[0].clientX;
        const swipeDistance = touchStartX - touchEndX;

        if (Math.abs(swipeDistance) > 50) { 
            if (swipeDistance > 0) {
                navigateSlide(1);
            } else {
                navigateSlide(-1); 
            }
        }

        touchStartX = null;
    });
}


document.addEventListener('DOMContentLoaded', setupCarousel);


window.addEventListener('load', function () {
   
    if (autoSlideTimer) {
        clearInterval(autoSlideTimer);
    }
    setupCarousel();
});